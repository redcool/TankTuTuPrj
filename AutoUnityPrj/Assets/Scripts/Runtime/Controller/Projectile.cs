using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 投射物控制器 - 统一处理飞行、瞄准、碰撞、伤害
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        // 序列化字段
        [Header("投射物属性")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 3f;
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private int _pierce = 1;  // 穿透敌人数量
        [SerializeField] private float _areaDamage = 0f;  // 范围伤害半径

        [Header("特效")]
        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private TrailRenderer _trail;

        // 私有字段
        private Transform _target;
        private Vector3 _direction;
        private TankDataValue _attackerData;
        private int _pierceCount = 0;
        private string _targetTag = "Enemy";
        private bool _isInitialized;

        // 公有属性
        public int Damage => _damage;
        public float Speed => _speed;

        /// <summary>
        /// 初始化投射物（完整参数）
        /// </summary>
        public void Initialize(int damage, float speed, float lifetime, TankDataValue attackerData = null, Transform target = null, string targetTag = "Enemy")
        {
            _damage = damage;
            _speed = speed;
            _lifeTime = lifetime;
            _attackerData = attackerData;
            _target = target;
            _targetTag = targetTag;
            _direction = target != null ? (target.position - transform.position).normalized : transform.forward;
            _isInitialized = true;

            // 自动销毁
            Destroy(gameObject, _lifeTime);
        }

        /// <summary>
        /// 初始化投射物（简化版 - 用于WeaponSlot直接调用）
        /// </summary>
        public void InitializeSimple(int damage, float speed, Vector3 direction, string targetTag = "Enemy")
        {
            _damage = damage;
            _speed = speed;
            _direction = direction.normalized;
            _targetTag = targetTag;
            _isInitialized = true;

            // 自动销毁
            Destroy(gameObject, _lifeTime);
        }

        /// <summary>
        /// 设置投射物朝向
        /// </summary>
        public void SetDirection(Vector3 direction)
        {
            _direction = direction.normalized;
        }

        /// <summary>
        /// 设置目标标签
        /// </summary>
        public void SetTargetTag(string tag)
        {
            _targetTag = tag;
        }

        private void Update()
        {
            if (!_isInitialized) return;
            Move();
            CheckCollision();
        }

        /// <summary>
        /// 移动投射物
        /// </summary>
        private void Move()
        {
            // 如果有目标，跟踪目标
            if (_target != null)
            {
                Vector3 directionToTarget = (_target.position - transform.position).normalized;
                _direction = Vector3.Lerp(_direction, directionToTarget, 5f * Time.deltaTime);
            }

            transform.position += _direction * _speed * Time.deltaTime;

            // 朝向运动方向
            if (_direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_direction);
            }
        }

        /// <summary>
        /// 检测碰撞
        /// </summary>
        private void CheckCollision()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(_targetTag))
                {
                    HitTarget(hit);
                    break;
                }
            }
        }

        /// <summary>
        /// 命中目标
        /// </summary>
        private void HitTarget(Collider target)
        {
            // 计算伤害
            var result = DamageSystem.CalculateDamage(_damage, _attackerData, null);

            // 对敌人造成伤害
            var enemyBase = target.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.TakeDamage(result.finalDamage);
            }

            // 对战车造成伤害
            var tankController = target.GetComponent<TankController>();
            if (tankController != null)
            {
                tankController.TakeDamage(result.finalDamage);
            }

            // 播放特效
            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, transform.position, Quaternion.identity);
            }

            // 处理穿透
            _pierceCount++;
            if (_pierceCount >= _pierce)
            {
                Destroy(gameObject);
            }
            else
            {
                // 暂时禁用碰撞避免重复命中
                StartCoroutine(DisableCollisionBriefly());
            }
        }

        /// <summary>
        /// 短暂禁用碰撞
        /// </summary>
        private System.Collections.IEnumerator DisableCollisionBriefly()
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            yield return new WaitForSeconds(0.1f);
            if (col != null) col.enabled = true;
        }

        /// <summary>
        /// 触发器碰撞（备用）
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_targetTag))
            {
                HitTarget(other);
            }
        }
    }
}
