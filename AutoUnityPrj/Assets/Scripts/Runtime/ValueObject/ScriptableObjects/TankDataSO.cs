using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 战车数据 ScriptableObject - 可在Inspector中配置
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    [CreateAssetMenu(fileName = "NewTankData", menuName = "铁皮突突/战车数据")]
    public class TankDataSO : ScriptableObject
    {
        [Header("生命属性")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _healthRegen = 0.5f;
        [SerializeField] private float _lifesteal = 0f;

        [Header("伤害属性")]
        [SerializeField] private float _percentDamage = 0f;
        [SerializeField] private float _rangedDamage = 0f;
        [SerializeField] private float _meleeDamage = 0f;
        [SerializeField] private float _elementDamage = 0f;
        [SerializeField] private float _engineering = 0f;

        [Header("战斗属性")]
        [SerializeField] private float _attackSpeed = 5f;
        [SerializeField] private float _critRate = 5f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private float _aimAccuracy = 0.85f;  // 瞄准精度阈值

        [Header("防御属性")]
        [SerializeField] private int _armor = 0;
        [SerializeField] private float _dodge = 0f;

        [Header("移动属性")]
        [SerializeField] private float _moveSpeed = 3f;

        [Header("成长属性")]
        [SerializeField] private float _luck = 0f;
        [SerializeField] private float _harvest = 1f;

        /// <summary>
        /// 转换为 TankDataValue
        /// </summary>
        public TankDataValue ToDataValue()
        {
            var data = new TankDataValue();
            data.MaxHealth = _maxHealth;
            data.HealthRegen = _healthRegen;
            data.Lifesteal = _lifesteal;
            data.PercentDamage = _percentDamage;
            data.RangedDamage = _rangedDamage;
            data.MeleeDamage = _meleeDamage;
            data.ElementDamage = _elementDamage;
            data.Engineering = _engineering;
            data.AttackSpeed = _attackSpeed;
            data.CritRate = _critRate;
            data.Range = _range;
            data.AimAccuracy = _aimAccuracy;
            data.Armor = _armor;
            data.Dodge = _dodge;
            data.MoveSpeed = _moveSpeed;
            data.Luck = _luck;
            data.Harvest = _harvest;
            return data;
        }
    }
}
