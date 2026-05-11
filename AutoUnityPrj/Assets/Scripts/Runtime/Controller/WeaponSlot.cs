using UnityEngine;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// 武器槽位组件 - 管理武器安装和射击
/// 作者：AI
/// 最后修改时间：2026-04-03
/// </summary>
namespace Game.Runtime.Controller
{
    public class WeaponSlot : MonoBehaviour
    {
        [Header("槽位设置")]
        [SerializeField] private int _slotIndex;
        [SerializeField] private Transform _muzzlePosition;

        [Header("武器数据 (ScriptableObject)")]
        [SerializeField] private WeaponDataSO _weaponDataSO;  // 武器数据SO
        [SerializeField] private GameObject _weaponPrefab;    // 武器预制体

        [Header("武器预制体")]
        [SerializeField] private GameObject _defaultWeaponPrefab;  // 默认武器预制体
        [SerializeField] private GameObject _bulletPrefab;        // 子弹预制体

        private WeaponDataValue _weaponData;
        private GameObject _weaponInstance;
        private Transform _currentTarget;
        private TankController _cachedTankController;  // 缓存战车控制器

        public WeaponDataValue WeaponData => _weaponData;
        public bool HasWeapon => _weaponData != null;
        public Transform MuzzlePosition => _muzzlePosition != null ? _muzzlePosition : transform;
        public int SlotIndex => _slotIndex;

        /// <summary>
        /// 获取缓存的战车控制器（延迟初始化）
        /// </summary>
        private TankController TankControllerRef
        {
            get
            {
                if (_cachedTankController == null)
                {
                    _cachedTankController = GetComponentInParent<TankController>();
                }
                return _cachedTankController;
            }
        }

        private void Start()
        {
            // 优先使用配置的SO武器
            if (_weaponDataSO != null && _weaponPrefab != null && !HasWeapon)
            {
                InstallWeapon(_weaponDataSO.ToDataValue(), _weaponPrefab);
            }
            // 如果没配置SO，使用默认武器
            else if (_slotIndex == 0 && _defaultWeaponPrefab != null && !HasWeapon)
            {
                var defaultData = new WeaponDataValue("default_blaster", "默认机关炮", WeaponType.Gatling, 10f, 2f, 8f);
                InstallWeapon(defaultData, _defaultWeaponPrefab);
            }
        }

        private void Update()
        {
            if (!HasWeapon) return;
            if (!_weaponData.CanAttack()) return;

            FindTarget();
            if (_currentTarget != null)
            {
                AimAndShoot();
            }
        }

        public bool InstallWeapon(WeaponDataValue weaponData, GameObject weaponPrefab)
        {
            if (HasWeapon) return false;

            _weaponData = weaponData;
            if (weaponPrefab != null)
            {
                _weaponInstance = Instantiate(weaponPrefab, transform);
                _weaponInstance.transform.localPosition = Vector3.zero;
                _weaponInstance.transform.localRotation = Quaternion.identity;
            }
            return true;
        }

        public void RemoveWeapon()
        {
            _weaponData = null;
            if (_weaponInstance != null)
            {
                Destroy(_weaponInstance);
                _weaponInstance = null;
            }
        }

        private void FindTarget()
        {
            TankDataValue tankData = TankControllerRef != null ? TankControllerRef.TankData : null;
            float range = _weaponData.GetFinalRange(tankData);
            Collider[] hits = Physics.OverlapSphere(transform.position, range);

            float closestDistance = float.MaxValue;
            _currentTarget = null;

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        _currentTarget = hit.transform;
                    }
                }
            }
        }

        private void AimAndShoot()
        {
            // 瞄准目标
            Vector3 direction = (_currentTarget.position - transform.position);
            direction.y = 0;

            // 如果距离极近（怪物重合），直接朝向怪物并射击
            if (direction.sqrMagnitude < 0.1f)
            {
                _weaponData.ExecuteAttack();
                SpawnProjectile();
                return;
            }

            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);

            // 检查是否对准目标（点乘 > 阈值）
            if (!IsAimedAtTarget()) return;

            _weaponData.ExecuteAttack();
            SpawnProjectile();
        }

        /// <summary>
        /// 检查是否对准目标
        /// 战车向前方向与到目标方向的点乘 > 阈值
        /// </summary>
        private bool IsAimedAtTarget()
        {
            if (_currentTarget == null) return false;

            // 获取战车向前方向
            Vector3 forward = transform.forward;
            forward.y = 0;
            //if (forward.sqrMagnitude < 0.01f) return false;
            //forward.Normalize();

            // 获取到目标的方向
            Vector3 toTarget = (_currentTarget.position - transform.position).normalized;
            toTarget.y = 0;

            // 点乘判断：越接近1表示越对准
            float dot = Vector3.Dot(forward, toTarget);

            // 从缓存的战车控制器获取瞄准精度阈值，默认0.85
            float aimThreshold = 0.85f;
            if (TankControllerRef != null && TankControllerRef.TankData != null)
            {
                aimThreshold = TankControllerRef.TankData.AimAccuracy;
            }

            return dot >= aimThreshold;
        }

        private void SpawnProjectile()
        {
            if (_currentTarget == null) return;

            // 子弹发射方向 = 战车的向前方向
            Vector3 fireDirection = transform.forward;
            fireDirection.y = 0;
            if (fireDirection.sqrMagnitude < 0.01f) return;
            fireDirection.Normalize();

            // 使用缓存的战车控制器获取数据
            TankDataValue tankData = TankControllerRef != null ? TankControllerRef.TankData : null;

            int damage = Mathf.RoundToInt(_weaponData.GetFinalDamage(tankData));

            // 使用Projectile统一控制器
            if (_bulletPrefab != null)
            {
                ProjectileFactory.CreateFromPrefab(
                    _bulletPrefab,
                    MuzzlePosition.position,
                    damage,
                    10f,
                    attackerData: tankData,
                    target: _currentTarget,
                    targetTag: "Enemy"
                );
            }
            else
            {
                ProjectileFactory.CreateSimple(
                    MuzzlePosition.position,
                    fireDirection,
                    damage,
                    10f,
                    targetTag: "Enemy"
                );
            }
        }
    }
}
