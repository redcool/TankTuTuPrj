using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 能量块掉落数据 ScriptableObject - 可在Inspector中配置掉落属性
    /// 作者：AI
    /// 最后修改时间：2026-04-03
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

        public int DefaultAmount => _defaultAmount;
        public float CollectRange => _collectRange;
        public float Lifetime => _lifetime;
        public float MagnetRange => _magnetRange;
        public float MagnetSpeed => _magnetSpeed;
        public bool UseMagnet => _useMagnet;
    }
}
