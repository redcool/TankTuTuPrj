using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 小怪AI - 继承自EnemyBase，用于普通小怪（海狸/奶牛）
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class EnemySmall : EnemyBase
    {
        /// <summary>
        /// 初始化为海狸
        /// </summary>
        public void InitializeAsBeaver()
        {
            SetEnemyData(EnemyDataValue.CreateBeaver());
        }

        /// <summary>
        /// 初始化为奶牛
        /// </summary>
        public void InitializeAsCow()
        {
            SetEnemyData(EnemyDataValue.CreateCow());
        }

        /// <summary>
        /// 初始化为指定类型
        /// </summary>
        public void InitializeAs(string enemyType)
        {
            switch (enemyType.ToLower())
            {
                case "beaver":
                    InitializeAsBeaver();
                    break;
                case "cow":
                    InitializeAsCow();
                    break;
                default:
                    InitializeAsBeaver();
                    break;
            }
        }

        protected override void Attack()
        {
            // 小怪普通攻击
            base.Attack();
        }

        protected override void OnDeath()
        {
            base.OnDeath();
        }
    }
}
