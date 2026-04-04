using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 投射物工厂 - 用于创建投射物
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public static class ProjectileFactory
    {
        /// <summary>
        /// 从预制体创建投射物
        /// </summary>
        public static Projectile CreateFromPrefab(GameObject prefab, Vector3 position, int damage, float speed, TankDataValue attackerData = null, Transform target = null, string targetTag = "Enemy")
        {
            GameObject projectileObj = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = projectileObj.AddComponent<Projectile>();
            }

            projectile.Initialize(damage, speed, 3f, attackerData, target, targetTag);
            return projectile;
        }

        /// <summary>
        /// 创建简单投射物（无预制体，使用球体）
        /// </summary>
        public static Projectile CreateSimple(Vector3 position, Vector3 direction, int damage, float speed, string targetTag = "Enemy")
        {
            GameObject projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObj.name = "Projectile";
            projectileObj.transform.position = position;
            projectileObj.transform.localScale = Vector3.one * 0.3f;

            // 添加投射物组件
            Projectile projectile = projectileObj.AddComponent<Projectile>();
            projectile.InitializeSimple(damage, speed, direction, targetTag);

            return projectile;
        }
    }
}
