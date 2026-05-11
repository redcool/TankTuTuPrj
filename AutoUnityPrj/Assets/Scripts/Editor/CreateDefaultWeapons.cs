#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 创建默认武器数据 - 战车专用武器
    /// </summary>
    public class CreateDefaultWeapons
    {
        [MenuItem("铁皮突突/创建数据/创建默认武器数据", false, 1)]
        public static void CreateWeapons()
        {
            string folder = "Assets/Resources/ScriptableObjects/Weapons";
            System.IO.Directory.CreateDirectory(folder);

            // 1. 主炮 (MainCannon) - 高伤害单发，默认武器
            CreateWeaponAsset(folder, "MainCannon", "主炮", WeaponType.MainCannon, 40f, 0.8f, 15f, 0, "高伤害单发炮弹，精准打击", true);

            // 2. 榴弹炮 (Howitzer) - 范围伤害
            CreateWeaponAsset(folder, "Howitzer", "榴弹炮", WeaponType.Howitzer, 30f, 0.5f, 12f, 200, "曲射弹道，造成范围伤害", false);

            // 3. 加农炮 (Cannon) - 均衡输出
            CreateWeaponAsset(folder, "Cannon", "加农炮", WeaponType.Cannon, 25f, 1f, 14f, 150, "均衡的火炮，伤害与攻速兼顾", false);

            // 4. 机关炮 (Gatling) - 快速连射
            CreateWeaponAsset(folder, "Gatling", "机关炮", WeaponType.Gatling, 8f, 5f, 8f, 180, "极速射击，适合清理大量敌人", false);

            // 5. 导弹 (Missile) - 追踪
            CreateWeaponAsset(folder, "Missile", "导弹", WeaponType.Missile, 50f, 0.4f, 18f, 350, "高精度追踪弹，自动锁定目标", false);

            // 6. 火箭弹 (Rocket) - 弹幕
            CreateWeaponAsset(folder, "Rocket", "火箭弹", WeaponType.Rocket, 20f, 0.6f, 10f, 280, "多发火箭弹，形成弹幕覆盖", false);

            // 7. 电磁炮 (Tesla) - 链式伤害
            CreateWeaponAsset(folder, "Tesla", "电磁炮", WeaponType.Tesla, 35f, 0.7f, 12f, 320, "放电攻击，可链式传导多个目标", false);

            // 8. 激光炮 (Laser) - 持续伤害
            CreateWeaponAsset(folder, "Laser", "激光炮", WeaponType.Laser, 15f, 1.5f, 14f, 250, "持续激光束，穿透目标", false);

            // 9. 穿甲弹 (AP) - 高穿深
            CreateWeaponAsset(folder, "AP", "穿甲弹", WeaponType.Cannon, 45f, 0.6f, 16f, 300, "高穿深穿甲弹，对重甲敌人特效", false);

            // 10. 燃烧弹 (Incendiary) - 持续燃烧
            CreateWeaponAsset(folder, "Incendiary", "燃烧弹", WeaponType.Howitzer, 25f, 0.5f, 11f, 220, "命中后持续燃烧，造成额外伤害", false);

            AssetDatabase.Refresh();
            Debug.Log("[CreateDefaultWeapons] 已创建10个战车武器数据");
        }

        private static void CreateWeaponAsset(string folder, string id, string name, WeaponType type, 
            float damage, float attackSpeed, float range, int price, string desc, bool isDefault)
        {
            string path = $"{folder}/{name}.asset";
            
            // 检查是否已存在
            if (System.IO.File.Exists(path))
            {
                Debug.Log($"[CreateDefaultWeapons] {name} 已存在,跳过");
                return;
            }

            var weapon = ScriptableObject.CreateInstance<WeaponDataSO>();
            
            // 使用 setter 直接设置属性
            weapon.WeaponIdSetter = id;
            weapon.WeaponNameSetter = name;
            weapon.WeaponTypeSetter = type;
            weapon.DamageSetter = damage;
            weapon.AttackSpeedSetter = attackSpeed;
            weapon.RangeSetter = range;
            weapon.PriceSetter = price;
            weapon.DescriptionSetter = desc;
            weapon.IsDefaultSetter = isDefault;
            
            // 根据类型设置特殊属性
            float pierce = 1f;
            float area = 0f;
            
            switch (type)
            {
                case WeaponType.Howitzer:
                case WeaponType.Rocket:
                    area = 3f;  // 范围伤害
                    break;
                case WeaponType.Missile:
                    pierce = 3f;  // 高穿深
                    break;
                case WeaponType.Gatling:
                    pierce = 1.5f;
                    break;
                case WeaponType.Tesla:
                    pierce = 2f;  // 链式
                    break;
                case WeaponType.Laser:
                    pierce = 5f;  // 持续穿透
                    area = 0.5f;
                    break;
                default:
                    pierce = 1f;
                    break;
            }
            
            weapon.PierceSetter = pierce;
            weapon.AreaSetter = area;

            AssetDatabase.CreateAsset(weapon, path);
            Debug.Log($"[CreateDefaultWeapons] 创建: {name}");
        }
    }
}
#endif