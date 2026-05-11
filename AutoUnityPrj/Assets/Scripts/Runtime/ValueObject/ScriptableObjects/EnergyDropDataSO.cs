using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 能量块掉落数据 ScriptableObject - 可在Inspector中配置掉落属性
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnergyDropData", menuName = "铁皮突突/能量块掉落数据")]
    public class EnergyDropDataSO : ScriptableObject
    {
        [Header("基础设置")]
        [SerializeField] private int _defaultAmount = 1;
        [SerializeField] private float _collectRange = 1.5f;
        [SerializeField] private float _lifetime = 30f;

        [Header("磁铁效果")]
        [SerializeField] private float _magnetRange = 3f;
        [SerializeField] private float _magnetSpeed = 5f;
        [SerializeField] private bool _useMagnet = true;

        #region Properties

        public int DefaultAmount
        {
            get => _defaultAmount;
            set => _defaultAmount = Mathf.Max(1, value);
        }

        public float CollectRange
        {
            get => _collectRange;
            set => _collectRange = Mathf.Max(0, value);
        }

        public float Lifetime
        {
            get => _lifetime;
            set => _lifetime = Mathf.Max(0, value);
        }

        public float MagnetRange
        {
            get => _magnetRange;
            set => _magnetRange = Mathf.Max(0, value);
        }

        public float MagnetSpeed
        {
            get => _magnetSpeed;
            set => _magnetSpeed = Mathf.Max(0, value);
        }

        public bool UseMagnet
        {
            get => _useMagnet;
            set => _useMagnet = value;
        }

        #endregion
    }
}