using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 能量块掉落 - 小怪掉落的资源
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class EnergyDrop : ResourceDrop
    {
        [Header("能量块特效")]
        [SerializeField] private GameObject _collectEffect;

        protected override void OnCollected()
        {
            // 播放收集特效
            if (_collectEffect != null)
            {
                Instantiate(_collectEffect, transform.position, Quaternion.identity);
            }

            base.OnCollected();
        }
    }
}
