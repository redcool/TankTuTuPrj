using UnityEngine;
using UnityEngine.InputSystem;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// 战车控制器 - 负责战车移动、转向和属性管理
/// 作者：AI
/// 最后修改时间：2026-04-03
/// </summary>
namespace Game.Runtime.Controller
{
    public class TankController : MonoBehaviour
    {
        // 常量
        private const string TAG_ENEMY = "Enemy";
        private const string TAG_RESOURCE = "Resource";

        // 序列化字段
        [Header("输入设置")]
        [SerializeField] private int _playerIndex = 0;

        [Header("坦克预制体")]
        [SerializeField] private GameObject _tankPrefab = null!;

        [Header("战车数据 (ScriptableObject)")]
        [SerializeField] private TankDataSO _tankDataSO;
        
        /// <summary>
        /// 设置战车数据SO (供外部代码使用,避免反射)
        /// </summary>
        public TankDataSO TankDataSOSetter
        {
            get => _tankDataSO;
            set => _tankDataSO = value;
        }

        [Header("组件缓存")]
        [SerializeField] private Transform _weaponSlotsRoot;
        [SerializeField] private Transform _modelContainer;

        // 私有字段
        private GameObject _tankInstance;
        private Rigidbody _rigidbody;
        private TankDataValue _tankData;
        private Transform[] _weaponSlots;
        private Vector2 _moveInput;
        private PlayerInput _playerInput;
        private InputAction _moveAction;

        // 摄像机缓存
        private Camera _mainCamera;

        // 公有属性
        public TankDataValue TankData => _tankData;
        public int PlayerIndex => _playerIndex;
        public bool IsAlive => _tankData.CurrentHealth > 0;

        // 生命周期
        private void Awake()
        {
            // 先初始化数据，再缓存组件（确保数据可用）
            InitializeData();
            CacheComponents();
            // 流程更新：先实例化坦克，再查找slots，再初始化其他
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

        // 私有方法
        private void CacheComponents()
        {
            // 物理组件从 PlayerTank (当前 gameObject) 获取
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                _rigidbody.useGravity = false;
            }

            // 缓存主摄像机
            _mainCamera = Camera.main;
        }

        private void InstantiateTank()
        {
            // 确保 Model_Container 存在
            if (_modelContainer == null)
            {
                _modelContainer = transform.Find("Model_Container");
            }
            
            if (_modelContainer == null)
            {
                Debug.LogWarning("[TankController] 未找到 Model_Container 子物体");
                return;
            }

            // 实例化坦克模型到 Model_Container 下
            if (_tankPrefab != null)
            {
                _tankInstance = Instantiate(_tankPrefab, _modelContainer.position, Quaternion.identity, _modelContainer);
                
                // 保持原始旋转
                _tankInstance.transform.localRotation = Quaternion.identity;
                
                Debug.Log($"[TankController] 已生成坦克模型到 Model_Container: {_tankInstance.name}");

                // 从实例化的坦克中查找武器槽位
                FindWeaponSlotsFromInstance();
            }
            else
            {
                Debug.LogWarning("[TankController] _tankPrefab 为 null，请拖入坦克模型 prefab");
            }
        }

        /// <summary>
        /// 从实例化的坦克中查找武器槽位 (Slot0, Slot1, ...)
        /// </summary>
        private void FindWeaponSlotsFromInstance()
        {
            if (_tankInstance == null) return;

            // 尝试在实例化坦克下查找名为 "WeaponSlots" 或 "Slots" 的父节点
            Transform slotsRoot = _tankInstance.transform.Find("WeaponSlots");
            if (slotsRoot == null)
            {
                slotsRoot = _tankInstance.transform.Find("Slots");
            }

            // 如果没找到父节点，尝试查找直接子级中包含 "Slot" 的
            if (slotsRoot == null)
            {
                for (int i = 0; i < _tankInstance.transform.childCount; i++)
                {
                    Transform child = _tankInstance.transform.GetChild(i);
                    if (child.name.Contains("Slot"))
                    {
                        // 找到第一个Slot，假设同级的都是Slot
                        Transform parent = child.parent;
                        if (parent != null && parent.childCount >= 6)
                        {
                            slotsRoot = parent;
                            break;
                        }
                    }
                }
            }

            if (slotsRoot != null)
            {
                _weaponSlotsRoot = slotsRoot;
                Debug.Log($"[TankController] 找到武器槽位根节点: {slotsRoot.name}");
            }
            else
            {
                Debug.LogWarning("[TankController] 未找到武器槽位，请检查预制体配置");
            }
        }

        private void InitializeData()
        {
            if (_tankData == null)
            {
                // 优先使用SO，如果没有则创建默认的
                if (_tankDataSO != null)
                {
                    _tankData = _tankDataSO.ToDataValue();
                    Debug.Log($"[TankController] 从SO加载战车数据: {_tankDataSO.name}");
                }
                else
                {
                    _tankData = new TankDataValue();
                    Debug.LogWarning("[TankController] 未配置TankDataSO，使用默认数据");
                }
            }
        }

        /// <summary>
        /// 重新初始化数据 (供外部代码调用)
        /// </summary>
        public void ReinitializeData()
        {
            _tankData = null;
            InitializeData();
        }

        private void SetupWeaponSlots()
        {
            if (_weaponSlotsRoot != null)
            {
                _weaponSlots = new Transform[_weaponSlotsRoot.childCount];
                for (int i = 0; i < _weaponSlotsRoot.childCount; i++)
                {
                    _weaponSlots[i] = _weaponSlotsRoot.GetChild(i);
                }
            }
        }

        private void SetupInput()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null && _playerInput.actions != null)
            {
                _moveAction = _playerInput.actions["Move"];
            }
            
            // Fallback: Create simple input if PlayerInput not configured
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

            // 获取摄像机的前后左右方向（忽略Y轴）
            Vector3 cameraForward = _mainCamera != null ? _mainCamera.transform.forward : Vector3.forward;
            Vector3 cameraRight = _mainCamera != null ? _mainCamera.transform.right : Vector3.right;
            
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // W = 摄像机前方向, S = 反方向, D = 右方向, A = 反方向
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
