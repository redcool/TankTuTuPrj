using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 资源掉落基类 - 能量块、宝箱等掉落物的基础控制器
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class ResourceDrop : MonoBehaviour
    {
        // 常量
        private const string TAG_PLAYER = "Player";

        // 序列化字段
        [Header("掉落设置")]
        [SerializeField] protected int _amount = 1;
        [SerializeField] protected float _collectRange = 1.5f;
        [SerializeField] protected float _lifetime = 30f;

        [Header("磁铁效果")]
        [SerializeField] protected float _magnetRange = 3f;
        [SerializeField] protected float _magnetSpeed = 5f;
        [SerializeField] protected bool _useMagnet = true;

        // 私有字段
        protected bool _isCollected;
        protected float _elapsedTime;
        protected Transform _nearestPlayer;

        // 公有属性
        public int Amount => _amount;
        public bool IsCollected => _isCollected;

        protected virtual void Start()
        {
            // 自动销毁
            Destroy(gameObject, _lifetime);
        }

        protected virtual void Update()
        {
            if (_isCollected) return;

            _elapsedTime += Time.deltaTime;
            FindNearestPlayer();

            // 磁铁效果
            if (_useMagnet && _nearestPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, _nearestPlayer.position);
                if (distance < _magnetRange)
                {
                    MoveTowardsPlayer();
                }
            }
        }

        /// <summary>
        /// 查找最近的玩家
        /// </summary>
        private void FindNearestPlayer()
        {
            var tanks = FindObjectsOfType<TankController>();
            float closestDistance = float.MaxValue;
            _nearestPlayer = null;

            foreach (var tank in tanks)
            {
                if (!tank.IsAlive) continue;

                float distance = Vector3.Distance(transform.position, tank.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _nearestPlayer = tank.transform;
                }
            }
        }

        /// <summary>
        /// 向玩家移动（磁铁效果）
        /// </summary>
        private void MoveTowardsPlayer()
        {
            if (_nearestPlayer == null) return;

            Vector3 direction = (_nearestPlayer.position - transform.position).normalized;
            transform.position += direction * _magnetSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 触发器收集
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            var tank = other.GetComponent<TankController>();
            if (tank != null && tank.IsAlive)
            {
                Collect(tank);
            }
        }

        /// <summary>
        /// 收集资源
        /// </summary>
        protected virtual void Collect(TankController collector)
        {
            if (_isCollected) return;
            _isCollected = true;

            // 添加资源到对应玩家
            GameManager.Instance?.AddResource(collector.PlayerIndex, _amount);

            OnCollected();
        }

        /// <summary>
        /// 收集后的效果（子类可重写）
        /// </summary>
        protected virtual void OnCollected()
        {
            // 播放收集动画/特效
            Destroy(gameObject, 0.1f);
        }

        /// <summary>
        /// 设置掉落数量
        /// </summary>
        public void SetAmount(int amount)
        {
            _amount = Mathf.Max(1, amount);
        }

        /// <summary>
        /// 设置收集范围
        /// </summary>
        public void SetCollectRange(float range)
        {
            _collectRange = Mathf.Max(0.5f, range);
        }

        /// <summary>
        /// 设置生命周期
        /// </summary>
        public void SetLifetime(float lifetime)
        {
            _lifetime = Mathf.Max(1f, lifetime);
        }

        /// <summary>
        /// 设置磁铁效果参数
        /// </summary>
        public void SetMagnetSettings(float range, float speed, bool enabled)
        {
            _magnetRange = Mathf.Max(0.5f, range);
            _magnetSpeed = Mathf.Max(0.1f, speed);
            _useMagnet = enabled;
        }
    }
}
