using System.Collections.Generic;
using Game.Runtime.ValueObject;

/// <summary>
/// 武器类别(WeaponCategory) → kenney_blaster 模型名映射
/// 运行时根据武器数据查询要加载的武器模型
/// 模型须位于 Assets/Resources/Prefabs/Weapons/ 下
/// </summary>
namespace Game.Runtime.Controller
{
    public static class WeaponModelMapper
    {
        private static readonly Dictionary<WeaponCategory, string> _categoryToModel = new Dictionary<WeaponCategory, string>
        {
            { WeaponCategory.MainCannon,  "blaster-d" },
            { WeaponCategory.MachineGun,  "blaster-a" },
            { WeaponCategory.Missile,     "blaster-b" },
            { WeaponCategory.Sprayer,     "scope-large-a" },
            { WeaponCategory.Melee,       "blaster-c" },
        };

        /// <summary>
        /// 根据武器类别获取对应 kenney_blaster 模型名称
        /// </summary>
        public static string GetModelName(WeaponCategory category)
        {
            if (_categoryToModel.TryGetValue(category, out var model))
                return model;

            return "blaster-a"; // 默认回退
        }

        /// <summary>
        /// 注册或覆盖武器类别→模型映射（供编辑器/扩展使用）
        /// </summary>
        public static void RegisterMapping(WeaponCategory category, string modelName)
        {
            _categoryToModel[category] = modelName;
        }
    }
}
