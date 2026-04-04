using UnityEngine;
using UnityEngine.AI;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 敌人基类 - 负责敌人AI、追踪、攻击
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class EnemyBase : MonoBehaviour
    {
        // 常量
        private const string TAG_PLAYER = "Player";
        private const float TARGET_UPDATE_INTERVAL = 0.5f;

        // 序列化字段
        [Header("敌人数据 (ScriptableObject)")]
        [SerializeField] protected EnemyDataSO _enemyDataSO;

        [Header("组件")]
        [SerializeField] protected Transform _target;
        [SerializeField] protected NavMeshAgent _navAgent;

        // 私有字段
        protected EnemyDataValue _enemyData;
        private float _lastAttackTime;
        private float _targetUpdateTimer;

        // 公有属性
        public EnemyDataValue EnemyData => _enemyData;
        public bool IsAlive => _enemyData?.IsAlive ?? false;

        protected virtual void Awake()
        {
            // 先初始化数据，再缓存组件（组件配置依赖数据）
            InitializeData();
            CacheComponents();
        }

        protected virtual void Start()
        {
            FindNearestTarget();
        }

        protected virtual void Update()
        {
            if (!IsAlive) return;

            UpdateTarget();
            MoveTowardsTarget();
            TryAttack();
        }

        /// <summary>
        /// 缓存组件
        /// </summary>
        private void CacheComponents()
        {
            if (_navAgent == null)
            {
                _navAgent = GetComponent<NavMeshAgent>();
                if (_navAgent == null)
                {
                    _navAgent = gameObject.AddComponent<NavMeshAgent>();
                }
            }

            // 设置NavMeshAgent参数
            if (_enemyData != null)
            {
                _navAgent.speed = _enemyData.MoveSpeed;
                _navAgent.stoppingDistance = _enemyData.AttackRange - 0.5f;
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            if (_enemyData == null)
            {
                // 优先使用SO
                if (_enemyDataSO != null)
                {
                    _enemyData = _enemyDataSO.ToDataValue();
                    Debug.Log($"[EnemyBase] 从SO加载敌人数据: {_enemyDataSO.name}");
                }
                else
                {
                    _enemyData = new EnemyDataValue();
                    Debug.LogWarning("[EnemyBase] 未配置EnemyDataSO，使用默认数据");
                }
            }
        }

        /// <summary>
        /// 设置敌人数据（外部调用）
        /// </summary>
        public void SetEnemyData(EnemyDataValue data)
        {
            _enemyData = data;
            if (_navAgent != null && _enemyData != null)
            {
                _navAgent.speed = _enemyData.MoveSpeed;
                _navAgent.stoppingDistance = _enemyData.AttackRange - 0.5f;
            }
        }

        /// <summary>
        /// 更新目标（定时）
        /// </summary>
        private void UpdateTarget()
        {
            _targetUpdateTimer += Time.deltaTime;
            if (_targetUpdateTimer >= TARGET_UPDATE_INTERVAL)
            {
                _targetUpdateTimer = 0f;
                FindNearestTarget();
            }
        }

        /// <summary>
        /// 查找最近的目标
        /// </summary>
        private void FindNearestTarget()
        {
            // 查找所有战车
            var tanks = FindObjectsOfType<TankController>();
            if (tanks.Length == 0) return;

            float closestDistance = float.MaxValue;
            Transform closestTarget = null;

            foreach (var tank in tanks)
            {
                if (!tank.IsAlive) continue;

                float distance = Vector3.Distance(transform.position, tank.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = tank.transform;
                }
            }

            _target = closestTarget;
        }

        /// <summary>
        /// 移动向目标
        /// </summary>
        private void MoveTowardsTarget()
        {
            if (_target == null || _navAgent == null) return;

            _navAgent.SetDestination(_target.position);
        }

        /// <summary>
        /// 尝试攻击
        /// </summary>
        private void TryAttack()
        {
            if (_target == null || _enemyData == null) return;

            float distance = Vector3.Distance(transform.position, _target.position);
            if (distance > _enemyData.AttackRange) return;

            // 检查攻击间隔
            if (Time.time - _lastAttackTime < _enemyData.AttackInterval) return;

            _lastAttackTime = Time.time;
            Attack();
        }

        /// <summary>
        /// 攻击目标
        /// </summary>
        protected virtual void Attack()
        {
            if (_target == null) return;

            // 获取目标战车
            var tank = _target.GetComponent<TankController>();
            if (tank != null)
            {
                int damage = Mathf.RoundToInt(_enemyData.AttackDamage);
                tank.TakeDamage(damage);
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            _enemyData.TakeDamage(damage);

            if (!IsAlive)
            {
                OnDeath();
            }
        }

        /// <summary>
        /// 死亡处理
        /// </summary>
        protected virtual void OnDeath()
        {
            // 掉落资源
            DropResources();

            // 销毁
            Destroy(gameObject, 0.1f);
        }

        /// <summary>
        /// 掉落资源
        /// </summary>
        private void DropResources()
        {
            if (_enemyData == null) return;

            // 掉落能量块
            if (_enemyData.ShouldDropEnergy())
            {
                GameManager.Instance?.SpawnEnergyDrop(transform.position, _enemyData.EnergyDrop);
            }

            // 掉落宝箱
            if (_enemyData.ShouldDropTreasureBox())
            {
                GameManager.Instance?.SpawnTreasureBox(transform.position);
            }
        }

        /// <summary>
        /// 获取追踪目标
        /// </summary>
        public Transform GetTarget()
        {
            return _target;
        }
    }
}
