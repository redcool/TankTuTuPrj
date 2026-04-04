using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 宝箱掉落 - Boss/精英怪掉落的宝箱
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class TreasureBox : ResourceDrop
    {
        [Header("宝箱设置")]
        [SerializeField] private int _minReward = 50;
        [SerializeField] private int _maxReward = 100;

        [Header("宝箱特效")]
        [SerializeField] private GameObject _openEffect;
        [SerializeField] private GameObject _glowEffect;

        private bool _isOpen;

        /// <summary>
        /// 计算奖励数量
        /// </summary>
        private int CalculateReward()
        {
            return Random.Range(_minReward, _maxReward + 1);
        }

        protected override void OnCollected()
        {
            if (_isOpen) return;
            _isOpen = true;

            // 播放开箱特效
            if (_openEffect != null)
            {
                Instantiate(_openEffect, transform.position, Quaternion.identity);
            }

            base.OnCollected();
        }

        /// <summary>
        /// 重写收集，宝箱奖励随机
        /// </summary>
        protected override void Collect(TankController collector)
        {
            if (_isCollected) return;

            _amount = CalculateReward();
            base.Collect(collector);
        }
    }
}
