using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// Boss AI - 继承自EnemyBase，用于Boss敌人（大象）
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class EnemyBoss : EnemyBase
    {
        // Boss特有属性
        [SerializeField] private int _currentPhase = 1;
        [SerializeField] private int _maxPhase = 2;
        [SerializeField] private float _phase2Threshold = 0.5f;  // 50%血量进入第二阶段

        // 特效
        [SerializeField] private GameObject _phaseChangeEffect;

        /// <summary>
        /// 初始化为大象Boss
        /// </summary>
        public void InitializeAsElephant()
        {
            SetEnemyData(EnemyDataValue.CreateElephantBoss());
            _maxPhase = 2;
            _currentPhase = 1;
        }

        protected override void Update()
        {
            base.Update();
            CheckPhaseChange();
        }

        /// <summary>
        /// 检查阶段转换
        /// </summary>
        private void CheckPhaseChange()
        {
            if (_currentPhase >= _maxPhase) return;
            if (_enemyData == null) return;

            float healthPercent = (float)_enemyData.CurrentHealth / _enemyData.MaxHealth;
            if (healthPercent <= _phase2Threshold && _currentPhase == 1)
            {
                EnterPhase2();
            }
        }

        /// <summary>
        /// 进入第二阶段
        /// </summary>
        private void EnterPhase2()
        {
            _currentPhase = 2;

            // Boss属性增强
            _enemyData.MoveSpeed *= 1.3f;
            _enemyData.AttackDamage *= 1.5f;
            _enemyData.AttackInterval *= 0.8f;

            // 更新NavMeshAgent
            if (_navAgent != null)
            {
                _navAgent.speed = _enemyData.MoveSpeed;
            }

            // 播放特效
            if (_phaseChangeEffect != null)
            {
                Instantiate(_phaseChangeEffect, transform.position, Quaternion.identity);
            }

            Debug.Log("[EnemyBoss] Boss进入第二阶段！属性大幅增强");
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }

        protected override void OnDeath()
        {
            // Boss死亡时必定掉落宝箱
            if (_enemyData != null)
            {
                GameManager.Instance?.SpawnTreasureBox(transform.position);
            }
            base.OnDeath();
        }

        /// <summary>
        /// 获取当前阶段
        /// </summary>
        public int GetCurrentPhase() => _currentPhase;

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => _enemyData?.IsAlive ?? false;
    }
}
