using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 武器数据 ScriptableObject - 可在Inspector中配置
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "铁皮突突/武器数据")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("武器标识")]
        [SerializeField] private string _weaponId = "";
        [SerializeField] private string _weaponName = "";
        [SerializeField] private WeaponType _weaponType = WeaponType.Ranged;

        [Header("基础属性")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _maxLevel = 5;

        [Header("特殊属性")]
        [SerializeField] private float _pierce = 1f;
        [SerializeField] private float _area = 0f;
        [SerializeField] private float _duration = 0f;

        [Header("升级")]
        [SerializeField] private int _upgradeCost = 100;

        /// <summary>
        /// 转换为 WeaponDataValue
        /// </summary>
        public WeaponDataValue ToDataValue()
        {
            var data = new WeaponDataValue(_weaponId, _weaponName, _weaponType, _damage, _attackSpeed, _range);
            data.Pierce = _pierce;
            data.Area = _area;
            data.Duration = _duration;
            return data;
        }
    }
}
