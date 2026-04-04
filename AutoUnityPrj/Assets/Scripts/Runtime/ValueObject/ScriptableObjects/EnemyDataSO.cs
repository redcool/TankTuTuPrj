using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 敌人数据 ScriptableObject - 可在Inspector中配置
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "铁皮突突/敌人数据")]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("敌人标识")]
        [SerializeField] private string _enemyId = "";
        [SerializeField] private string _enemyName = "";
        [SerializeField] private EnemyType _enemyType = EnemyType.Normal;

        [Header("基础属性")]
        [SerializeField] private int _maxHealth = 50;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackInterval = 1f;

        [Header("战斗属性")]
        [SerializeField] private float _critRate = 5f;
        [SerializeField] private int _armor = 0;

        [Header("掉落属性")]
        [SerializeField] private int _energyDrop = 1;
        [SerializeField] private float _dropChance = 1f;
        [SerializeField] private bool _dropTreasureBox = false;
        [SerializeField] private int _treasureBoxDropChance = 0;

        /// <summary>
        /// 转换为 EnemyDataValue
        /// </summary>
        public EnemyDataValue ToDataValue()
        {
            return new EnemyDataValue(_enemyId, _enemyName, _enemyType, _maxHealth, _moveSpeed)
            {
                AttackDamage = _attackDamage,
                AttackRange = _attackRange,
                AttackInterval = _attackInterval,
                CritRate = _critRate,
                Armor = _armor,
                EnergyDrop = _energyDrop,
                DropChance = _dropChance,
                DropTreasureBox = _dropTreasureBox,
                TreasureBoxDropChance = _treasureBoxDropChance
            };
        }
    }
}
