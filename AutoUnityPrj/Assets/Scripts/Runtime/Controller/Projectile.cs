using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 投射物控制器 — 直线飞行，碰撞即爆炸
    /// 发出时武器已对准目标，子弹直线前进，不追踪
    /// 碰到任何物体（敌人/障碍物）立即爆炸销毁
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("投射物属性")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 3f;
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private int _pierce = 1;
        [SerializeField] private float _areaDamage = 0f;  // >0 时命中后产生范围伤害

        [Header("特效")]
        [SerializeField] private GameObject _hitEffect;

        // 运行时状态
        private Vector3 _direction;
        private TankDataValue _attackerData;
        private int _pierceCount;
        private string _targetTag = "Enemy";
        private bool _isInitialized;
        private bool _isDestroyed;

        public int Damage => _damage;
        public float Speed => _speed;

        /// <summary>
        /// 完整初始化（用于预制体弹体）
        /// target 仅用于计算初始发射方向，飞行中不追踪
        /// </summary>
        public void Initialize(int damage, float speed, float lifetime,
            TankDataValue attackerData = null, Transform target = null,
            string targetTag = "Enemy")
        {
            _damage = damage;
            _speed = speed;
            _lifeTime = lifetime;
            _attackerData = attackerData;
            _targetTag = targetTag;
            _direction = target != null
                ? (target.position - transform.position).normalized
                : transform.forward;
            _isInitialized = true;

            SetupCollider();
            Destroy(gameObject, _lifeTime);
        }

        /// <summary>
        /// 简化初始化（WeaponSlot 动态创建时使用）
        /// </summary>
        public void InitializeSimple(int damage, float speed, Vector3 direction,
            string targetTag = "Enemy")
        {
            _damage = damage;
            _speed = speed;
            _direction = direction.normalized;
            _targetTag = targetTag;
            _isInitialized = true;

            SetupCollider();
            Destroy(gameObject, _lifeTime);
        }

        /// <summary>
        /// 配置碰撞体：设为 Trigger + Kinematic Rigidbody 以触发物理事件
        /// </summary>
        private void SetupCollider()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            var rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction.normalized;
        }

        public void SetTargetTag(string tag)
        {
            _targetTag = tag;
        }

        private void Update()
        {
            if (!_isInitialized) return;
            Move();
        }

        private void Move()
        {
            // 直线前进，不追踪
            transform.position += _direction * _speed * Time.deltaTime;

            // 朝向运动方向
            if (_direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_direction);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isDestroyed || !_isInitialized) return;
            if (other.gameObject == gameObject) return;

            // 跳过玩家自己的战车（持有 TankController 但不属于敌方）
            if (other.GetComponent<TankController>() != null) return;

            _isDestroyed = true;

            // 命中敌人 → 造成伤害
            if (other.CompareTag(_targetTag))
            {
                var result = DamageSystem.CalculateDamage(_damage, _attackerData, null);

                var enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                    enemy.TakeDamage(result.finalDamage);
            }

            // 碰到任何物体（墙/障碍/敌人）→ 爆炸
            Explode();
        }

        /// <summary>
        /// 爆炸：特效 + 范围伤害 + 销毁
        /// </summary>
        private void Explode()
        {
            // 命中特效
            if (_hitEffect != null)
                Instantiate(_hitEffect, transform.position, Quaternion.identity);

            // 范围伤害（爆炸半径内的敌人）
            if (_areaDamage > 0)
            {
                var hits = Physics.OverlapSphere(transform.position, _areaDamage);
                foreach (var hit in hits)
                {
                    if (!hit.CompareTag(_targetTag)) continue;
                    var result = DamageSystem.CalculateDamage(
                        Mathf.RoundToInt(_damage * 0.5f), _attackerData, null);
                    var enemy = hit.GetComponent<EnemyBase>();
                    if (enemy != null) enemy.TakeDamage(result.finalDamage);
                }
            }

            Destroy(gameObject);
        }
    }
}
