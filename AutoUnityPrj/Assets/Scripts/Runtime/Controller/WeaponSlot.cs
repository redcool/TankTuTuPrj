using UnityEngine;
using Game.Runtime.ValueObject;

/// <summary>
/// 武器槽位组件 - 管理武器安装和射击
/// 作者：AI
/// 最后修改时间：2026-04-03
/// </summary>
namespace Game.Runtime.Controller
{
    public class WeaponSlot : MonoBehaviour
    {
        [SerializeField] private int _slotIndex;
        [SerializeField] private Transform _muzzlePosition;

        private WeaponDataValue _weaponData;
        private GameObject _weaponInstance;
        private Transform _currentTarget;

        public WeaponDataValue WeaponData => _weaponData;
        public bool HasWeapon => _weaponData != null;
        public Transform MuzzlePosition => _muzzlePosition != null ? _muzzlePosition : transform;

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
            float range = _weaponData.GetFinalRange(null);
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
            Vector3 direction = (_currentTarget.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            }

            _weaponData.ExecuteAttack();
            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.transform.position = MuzzlePosition.position;
            projectile.transform.localScale = Vector3.one * 0.2f;

            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;

            Vector3 direction = (_currentTarget.position - transform.position).normalized;
            rb.velocity = direction * 10f;

            Destroy(projectile, 3f);
        }
    }
}