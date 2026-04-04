using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 伤害系统 - 负责伤害计算、护甲、闪避、暴击
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class DamageSystem
    {
        /// <summary>
        /// 伤害结果结构
        /// </summary>
        public struct DamageResult
        {
            public int finalDamage;      // 最终伤害
            public bool isCritical;       // 是否暴击
            public bool isDodged;        // 是否闪避
            public bool isBlocked;       // 是否被格挡
            public float damageReduction; // 伤害减少百分比
        }

        /// <summary>
        /// 计算对目标的伤害
        /// </summary>
        /// <param name="baseDamage">基础伤害</param>
        /// <param name="attackerData">攻击者数据（战车属性）</param>
        /// <param name="defenderData">防御者数据（敌人/战车属性）</param>
        /// <returns>伤害结果</returns>
        public static DamageResult CalculateDamage(int baseDamage, TankDataValue attackerData, object defenderData)
        {
            DamageResult result = new DamageResult
            {
                isCritical = false,
                isDodged = false,
                isBlocked = false,
                damageReduction = 0
            };

            // 1. 检查是否闪避
            float dodgeChance = 0;
            if (defenderData is TankDataValue tankDefender)
            {
                dodgeChance = tankDefender.Dodge;
            }
            else if (defenderData is EnemyDataValue enemyDefender)
            {
                // 敌人默认闪避率为0
                dodgeChance = 0;
            }

            if (dodgeChance > 0 && Random.Range(0f, 100f) < dodgeChance)
            {
                result.isDodged = true;
                result.finalDamage = 0;
                return result;
            }

            // 2. 计算暴击
            float critChance = attackerData != null ? attackerData.CritRate : 5f;
            bool isCritical = Random.Range(0f, 100f) < critChance;

            float finalDamage = baseDamage;

            // 暴击伤害倍率
            if (isCritical)
            {
                finalDamage *= 1.5f;  // 暴击150%伤害
                result.isCritical = true;
            }

            // 3. 应用百分比伤害加成
            if (attackerData != null)
            {
                finalDamage *= (1 + attackerData.PercentDamage / 100f);
            }

            // 4. 计算护甲减伤
            int armor = 0;
            if (defenderData is TankDataValue tankWithArmor)
            {
                armor = tankWithArmor.Armor;
            }
            else if (defenderData is EnemyDataValue enemyWithArmor)
            {
                armor = enemyWithArmor.Armor;
            }

            // 护甲减伤公式：减少伤害 = 护甲值，但有上限
            if (armor > 0)
            {
                int armorReduction = Mathf.Min(armor, (int)(finalDamage * 0.8f));  // 最多减80%伤害
                finalDamage -= armorReduction;
                result.damageReduction = (float)armorReduction / baseDamage;
            }

            // 5. 确保最小伤害为1
            result.finalDamage = Mathf.Max(1, (int)finalDamage);

            return result;
        }

        /// <summary>
        /// 计算武器伤害（考虑战车属性加成）
        /// </summary>
        public static int CalculateWeaponDamage(WeaponDataValue weaponData, TankDataValue tankData)
        {
            if (weaponData == null) return 0;

            float damage = weaponData.GetFinalDamage(tankData);

            // 应用暴击
            float critChance = tankData != null ? tankData.CritRate : 5f;
            if (Random.Range(0f, 100f) < critChance)
            {
                damage *= 1.5f;
            }

            return Mathf.RoundToInt(damage);
        }

        /// <summary>
        /// 造成范围伤害
        /// </summary>
        public static void ApplyAreaDamage(Vector3 center, float radius, int baseDamage, TankDataValue attackerData, string targetTag)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(targetTag))
                {
                    // 对每个目标造成伤害
                    var enemyBase = hit.GetComponent<EnemyBase>();
                    if (enemyBase != null)
                    {
                        var result = CalculateDamage(baseDamage, attackerData, enemyBase.EnemyData);
                        enemyBase.TakeDamage(result.finalDamage);
                    }
                }
            }
        }

        /// <summary>
        /// 造成持续伤害（DOT）
        /// </summary>
        public static void ApplyDoTDamage(object target, float damagePerSecond, float duration, TankDataValue attackerData)
        {
            // TODO: 实现持续伤害协程
            Debug.Log($"[DamageSystem] 造成持续伤害: {damagePerSecond}/秒, 持续{duration}秒");
        }
    }
}