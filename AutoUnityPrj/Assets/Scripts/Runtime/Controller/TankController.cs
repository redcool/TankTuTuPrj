using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// Tank Controller - manages tank movement, rotation and attributes
/// Data source: loads CharacterDataSO from GameManager.SelectedCharacterId
/// Model source: dynamically loads kenney_car model based on character via CharacterModelMapper
/// Weapon slots: dynamically created in 360° around the tank based on SelectedWeaponDatas
/// </summary>
namespace Game.Runtime.Controller
{
    public class TankController : MonoBehaviour
    {
        private const string TAG_ENEMY = "Enemy";
        private const string TAG_RESOURCE = "Resource";
        private const string CHARACTERS_RESOURCE_PATH = "ScriptableObjects/Characters/Character_";
        private const string CARS_RESOURCE_PATH = "Prefabs/Cars/";
        private const string WEAPONS_RESOURCE_PATH = "Prefabs/Weapons/";
        private const float WEAPON_SLOT_RADIUS = 1.5f;
        private const float WEAPON_SLOT_HEIGHT = 1.0f;
        private const int MAX_WEAPON_SLOTS = 6;

        [Header("Input Settings")]
        [SerializeField] private int _playerIndex = 0;

        [Header("Tank Prefab (fallback)")]
        [SerializeField] private GameObject _tankPrefab = null!;

        [Header("Default Character ID (used when GameManager not set)")]
        [SerializeField] private string _defaultCharacterId = "mbt";

        [Header("Component Cache")]
        [SerializeField] private Transform _weaponSlotsRoot;
        [SerializeField] private Transform _modelContainer;

        private GameObject _tankInstance;
        private Rigidbody _rigidbody;
        private TankDataValue _tankData;
        private Transform[] _weaponSlots;
        private Vector2 _moveInput;
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private Camera _mainCamera;

        public TankDataValue TankData => _tankData;
        public int PlayerIndex => _playerIndex;
        public bool IsAlive => _tankData.CurrentHealth > 0;

        private void Awake()
        {
            InitializeData();
            CacheComponents();
            InstantiateTank();
            SetupWeaponSlots();
            SetupInput();
        }

        private void OnEnable()
        {
            if (_moveAction != null) _moveAction.Enable();
        }

        private void OnDisable()
        {
            if (_moveAction != null) _moveAction.Disable();
        }

        private void Start()
        {
            _tankData.CurrentHealth = _tankData.MaxHealth;
        }

        private void Update()
        {
            if (!IsAlive) return;
            ProcessInput();
            UpdateVisuals();
        }

        private void FixedUpdate()
        {
            if (!IsAlive) return;
            MoveTank();
            RegenerateHealth();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TAG_RESOURCE))
            {
                CollectResource(other);
            }
        }

        private void CacheComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                _rigidbody.useGravity = false;
            }
            _mainCamera = Camera.main;
        }

        private void InstantiateTank()
        {
            if (_modelContainer == null)
            {
                _modelContainer = transform.Find("Model_Container");
            }

            if (_modelContainer == null)
            {
                Debug.LogWarning("[TankController] Model_Container not found");
                return;
            }

            // ── 动态加载角色对应的 kenney_car 模型 ──
            string characterId = GetLoadedCharacterId();
            string modelName = CharacterModelMapper.GetModelName(characterId);
            // 车辆预制体统一以 car_ 前缀命名（见 CreateModelPrefabs.cs）
            var carPrefab = Resources.Load<GameObject>(CARS_RESOURCE_PATH + "car_" + modelName);

            if (carPrefab != null)
            {
                _tankInstance = Instantiate(carPrefab, _modelContainer.position, Quaternion.identity, _modelContainer);
                _tankInstance.transform.localRotation = Quaternion.identity;
                Debug.Log($"[TankController] Tank model loaded: {modelName} (character: {characterId})");
            }
            else
            {
                // ── 回退：使用 _tankPrefab（编辑器引用） ──
                Debug.LogWarning($"[TankController] Car prefab not found: {modelName}, fallback to _tankPrefab");
                if (_tankPrefab != null)
                {
                    _tankInstance = Instantiate(_tankPrefab, _modelContainer.position, Quaternion.identity, _modelContainer);
                    _tankInstance.transform.localRotation = Quaternion.identity;
                    Debug.Log($"[TankController] Fallback tank model: {_tankPrefab.name}");
                }
                else
                {
                    Debug.LogWarning("[TankController] _tankPrefab is null, no tank model spawned");
                    return;
                }
            }

            // 武器槽不再从模型中查找，由 CreateDynamicWeaponSlots 动态生成
        }

        /// <summary>
        /// 获取已加载的角色 ID（从 _tankData 反推）
        /// </summary>
        private string GetLoadedCharacterId()
        {
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.SelectedCharacterId))
                return GameManager.Instance.SelectedCharacterId;

            return _defaultCharacterId;
        }

        private void InitializeData()
        {
            if (_tankData != null) return;

            // 优先使用 GameManager 已缓存的 CharacterDataSO（CharacterSelectPresenter 已写入）
            if (GameManager.Instance?.SelectedCharacterData != null)
            {
                _tankData = GameManager.Instance.SelectedCharacterData.ToTankDataValue();
                Debug.Log($"[TankController] Loaded from GameManager cache: {GameManager.Instance.SelectedCharacterData.CharacterName}");
                return;
            }

            // 回退：按角色 ID 重新加载
            string characterId = "";
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.SelectedCharacterId))
            {
                characterId = GameManager.Instance.SelectedCharacterId;
                Debug.Log($"[TankController] Loading character by ID: {characterId}");
            }
            else
            {
                characterId = _defaultCharacterId;
                Debug.Log($"[TankController] Using default character: {characterId}");
            }

            string assetName = char.ToUpper(characterId[0]) + characterId.Substring(1);
            var charData = Resources.Load<CharacterDataSO>(CHARACTERS_RESOURCE_PATH + assetName);

            if (charData != null)
            {
                _tankData = charData.ToTankDataValue();
                Debug.Log($"[TankController] Loaded: {charData.CharacterName}");
            }
            else
            {
                charData = Resources.Load<CharacterDataSO>("ScriptableObjects/Characters/" + characterId);
                if (charData != null)
                {
                    _tankData = charData.ToTankDataValue();
                    Debug.Log($"[TankController] Loaded (direct): {charData.CharacterName}");
                }
                else
                {
                    _tankData = new TankDataValue();
                    Debug.LogWarning($"[TankController] Character data not found (ID: {characterId}), using defaults");
                }
            }
        }

        public void ReinitializeData()
        {
            _tankData = null;
            InitializeData();
        }

        private void SetupWeaponSlots()
        {
            // 不再从模型预制体读取武器槽，改为动态 360° 生成
            CreateDynamicWeaponSlots();
        }

        /// <summary>
        /// 动态创建 360° 武器槽位
        /// 根据 GameManager.SelectedWeaponDatas 数量在圆周上等分
        /// </summary>
        private void CreateDynamicWeaponSlots()
        {
            // 创建武器槽根节点
            GameObject slotsRootObj = new GameObject("DynamicWeaponSlots");
            slotsRootObj.transform.SetParent(transform, false);
            _weaponSlotsRoot = slotsRootObj.transform;

            // ── 收集武器数据并按名字配对 ──
            List<WeaponDataSO> weapons = GetWeaponDataList();

            if (weapons == null || weapons.Count == 0)
            {
                Debug.LogWarning("[TankController] 无武器数据，创建 1 个默认机关炮槽位");
                CreateSingleSlotWithDefaultWeapon(0);
                BuildWeaponSlotsArray();
                return;
            }

            // 按角色初始武器名字进行配对
            weapons = MatchWeaponsToCharacter(weapons);

            int count = Mathf.Clamp(weapons.Count, 1, MAX_WEAPON_SLOTS);

            for (int i = 0; i < count; i++)
            {
                GameObject slotObj = new GameObject($"WeaponSlot_{i}");
                slotObj.transform.SetParent(slotsRootObj.transform, false);

                // 360° 环绕定位
                float angleDeg = i * (360f / count);
                float angleRad = angleDeg * Mathf.Deg2Rad;
                float x = WEAPON_SLOT_RADIUS * Mathf.Sin(angleRad);
                float z = WEAPON_SLOT_RADIUS * Mathf.Cos(angleRad);
                slotObj.transform.localPosition = new Vector3(x, WEAPON_SLOT_HEIGHT, z);
                slotObj.transform.localRotation = Quaternion.Euler(0, angleDeg, 0);

                var slot = slotObj.AddComponent<WeaponSlot>();
                slot.SlotIndex = i;

                // 安装武器
                if (i < weapons.Count && weapons[i] != null)
                {
                    InstallWeaponIntoSlot(slot, weapons[i]);
                }
            }

            BuildWeaponSlotsArray();
            Debug.Log($"[TankController] 动态创建了 {_weaponSlots.Length} 个武器槽 (360°分布, {count}件武器)");
        }

        /// <summary>
        /// 从 GameManager 获取武器数据列表
        /// </summary>
        private List<WeaponDataSO> GetWeaponDataList()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.SelectedWeaponDatas != null &&
                GameManager.Instance.SelectedWeaponDatas.Count > 0)
            {
                return GameManager.Instance.SelectedWeaponDatas;
            }

            // 兼容旧版单武器数据
            if (GameManager.Instance != null && GameManager.Instance.SelectedWeaponData != null)
            {
                return new List<WeaponDataSO> { GameManager.Instance.SelectedWeaponData };
            }

            return null;
        }

        /// <summary>
        /// 按名字配对武器：将已选择的武器与角色的初始武器进行配对
        /// 先尝试按名字匹配（武器名/武器ID包含起始武器名），未匹配的武器按顺序补位
        /// </summary>
        private List<WeaponDataSO> MatchWeaponsToCharacter(List<WeaponDataSO> selectedWeapons)
        {
            // 优先使用 GameManager 已缓存的 CharacterDataSO
            CharacterDataSO charData = GameManager.Instance?.SelectedCharacterData;
            string charId = GetLoadedCharacterId();
            if (charData == null)
            {
                // 回退：按角色 ID 重新加载
                string assetName = char.ToUpper(charId[0]) + charId.Substring(1);
                charData = Resources.Load<CharacterDataSO>(CHARACTERS_RESOURCE_PATH + assetName);
            }

            if (charData?.StartingWeaponPaths == null || charData.StartingWeaponPaths.Length == 0)
            {
                Debug.Log($"[TankController] 角色 {charId} 无起始武器配置，武器按顺序安装");
                return selectedWeapons;
            }

            // 从路径中提取起始武器的名字片段（最后一个 _ 之后的部分，转小写）
            var startingWeaponTokens = new List<string>();
            foreach (var path in charData.StartingWeaponPaths)
            {
                int idx = path.LastIndexOf('_');
                string token = idx >= 0 ? path.Substring(idx + 1) : path;
                startingWeaponTokens.Add(token.ToLower());
                Debug.Log($"[TankController] 角色初始武器路径: {path} → 匹配Token: {token}");
            }

            // 配对：已匹配的放前面（按起始武器顺序），未匹配的放后面
            var matched = new List<WeaponDataSO>();
            var remaining = new List<WeaponDataSO>(selectedWeapons);

            foreach (var token in startingWeaponTokens)
            {
                // 在剩余武器中找名字或ID包含该Token的
                int matchIdx = -1;
                for (int i = 0; i < remaining.Count; i++)
                {
                    if (remaining[i] == null) continue;
                    string wName = (remaining[i].WeaponName ?? "").ToLower();
                    string wId = (remaining[i].WeaponId ?? "").ToLower();
                    if (wName.Contains(token) || wId.Contains(token))
                    {
                        matchIdx = i;
                        break;
                    }
                }

                if (matchIdx >= 0)
                {
                    matched.Add(remaining[matchIdx]);
                    Debug.Log($"[TankController] 武器配对成功: {remaining[matchIdx].WeaponName} ↔ {token}");
                    remaining.RemoveAt(matchIdx);
                }
                else
                {
                    matched.Add(null); // 该位置无匹配，使用占位符
                }
            }

            // 重新组装：配对武器占对应位置，未配对的依次补位
            var result = new List<WeaponDataSO>();
            int remainingPtr = 0;
            foreach (var m in matched)
            {
                if (m != null)
                {
                    result.Add(m);
                }
                else if (remainingPtr < remaining.Count)
                {
                    // 无配对武器，用剩余未匹配武器按顺序补位
                    result.Add(remaining[remainingPtr++]);
                }
            }
            // 剩余未匹配武器追加到末尾
            while (remainingPtr < remaining.Count)
            {
                result.Add(remaining[remainingPtr++]);
            }

            Debug.Log($"[TankController] 武器配对完成: {selectedWeapons.Count}把 → {result.Count}把 (按名字配对)");
            return result;
        }

        /// <summary>
        /// 将武器数据安装到指定槽位
        /// </summary>
        private void InstallWeaponIntoSlot(WeaponSlot slot, WeaponDataSO weaponData)
        {
            // 通过 WeaponModelMapper 获取武器模型名称
            string modelName = WeaponModelMapper.GetModelName(weaponData.WeaponCategory);
            GameObject weaponPrefab = Resources.Load<GameObject>(WEAPONS_RESOURCE_PATH + modelName);

            if (weaponPrefab == null)
            {
                Debug.LogWarning($"[TankController] 武器模型未找到: {modelName}，使用 blaster-a 回退");
                weaponPrefab = Resources.Load<GameObject>(WEAPONS_RESOURCE_PATH + "blaster-a");
            }

            slot.InstallWeapon(weaponData.ToDataValue(), weaponPrefab);
        }

        /// <summary>
        /// 创建单个默认机关炮槽位（无武器数据时使用）
        /// </summary>
        private void CreateSingleSlotWithDefaultWeapon(int index)
        {
            GameObject slotObj = new GameObject($"WeaponSlot_{index}");
            slotObj.transform.SetParent(_weaponSlotsRoot, false);
            slotObj.transform.localPosition = new Vector3(0, WEAPON_SLOT_HEIGHT, WEAPON_SLOT_RADIUS);
            slotObj.transform.localRotation = Quaternion.identity;

            var slot = slotObj.AddComponent<WeaponSlot>();
            slot.SlotIndex = index;

            // 加载默认武器（blaster-a）
            var defaultPrefab = Resources.Load<GameObject>(WEAPONS_RESOURCE_PATH + "blaster-a");
            var defaultData = new WeaponDataValue("default_blaster", "默认机关炮",
                WeaponCategory.MachineGun, WeaponType.Gatling, 10f, 2f, 8f);
            slot.InstallWeapon(defaultData, defaultPrefab);
        }

        /// <summary>
        /// 构建 _weaponSlots 数组
        /// </summary>
        private void BuildWeaponSlotsArray()
        {
            if (_weaponSlotsRoot == null) return;
            _weaponSlots = new Transform[_weaponSlotsRoot.childCount];
            for (int i = 0; i < _weaponSlotsRoot.childCount; i++)
            {
                _weaponSlots[i] = _weaponSlotsRoot.GetChild(i);
            }
        }

        private void SetupInput()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null && _playerInput.actions != null)
            {
                _moveAction = _playerInput.actions["Move"];
            }

            if (_moveAction == null)
            {
                Debug.Log("TankController: Using fallback keyboard input");
                CreateFallbackInput();
            }
        }

        private void CreateFallbackInput()
        {
            _moveAction = new InputAction("Move", InputActionType.Value, "<Keyboard>/w");
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            _moveAction.Enable();
        }

        private void ProcessInput()
        {
            if (_moveAction != null)
            {
                _moveInput = _moveAction.ReadValue<Vector2>();
            }
        }

        private void MoveTank()
        {
            if (_moveInput.sqrMagnitude < 0.01f) return;

            Vector3 cameraForward = _mainCamera != null ? _mainCamera.transform.forward : Vector3.forward;
            Vector3 cameraRight = _mainCamera != null ? _mainCamera.transform.right : Vector3.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * _moveInput.y + cameraRight * _moveInput.x).normalized;
            Debug.DrawRay(transform.position, moveDirection * 10, Color.green);
            Vector3 targetPosition = transform.position + moveDirection * _tankData.MoveSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(targetPosition);

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
            }
        }

        private void RegenerateHealth()
        {
            if (_tankData.CurrentHealth < _tankData.MaxHealth && _tankData.HealthRegen > 0)
            {
                _tankData.CurrentHealth += Mathf.RoundToInt(_tankData.HealthRegen * Time.fixedDeltaTime);
            }
        }

        private void UpdateVisuals()
        {
        }

        private void CollectResource(Collider resource)
        {
            int amount = 1;
            amount = Mathf.RoundToInt(amount * _tankData.Harvest);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddResource(_playerIndex, amount);
            }
            Destroy(resource.gameObject);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            int actualDamage = damage;
            if (_tankData.Armor > 0)
            {
                actualDamage = Mathf.Max(1, damage - _tankData.Armor);
            }

            if (Random.value < _tankData.Dodge / 100f)
            {
                return;
            }

            _tankData.CurrentHealth -= actualDamage;

            if (_tankData.CurrentHealth <= 0)
            {
                OnDeath();
            }
        }

        public void Heal(int amount)
        {
            _tankData.CurrentHealth += amount;
        }

        public void AddStat(string statName, float value)
        {
            switch (statName.ToLower())
            {
                case "maxhealth": _tankData.MaxHealth += (int)value; break;
                case "healthregen": _tankData.HealthRegen += value; break;
                case "damage": _tankData.PercentDamage += value; break;
                case "attackspeed": _tankData.AttackSpeed += value; break;
                case "movespeed": _tankData.MoveSpeed += value; break;
                case "critrate": _tankData.CritRate += value; break;
                case "armor": _tankData.Armor += (int)value; break;
                case "luck": _tankData.Luck += value; break;
                case "harvest": _tankData.Harvest += value; break;
            }
        }

        private void OnDeath()
        {
            _tankData.CurrentHealth = 0;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTankDeath(_playerIndex);
            }
        }

        public Transform GetWeaponSlot(int index)
        {
            if (_weaponSlots != null && index >= 0 && index < _weaponSlots.Length)
            {
                return _weaponSlots[index];
            }
            return null;
        }

        public int GetAvailableSlotCount()
        {
            if (_weaponSlots == null) return 0;
            int count = 0;
            foreach (Transform slot in _weaponSlots)
            {
                if (slot.childCount == 0) count++;
            }
            return count;
        }
    }
}
