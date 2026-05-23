using System.Collections.Generic;

/// <summary>
/// 角色 ID → kenney_car 车辆模型名映射
/// 运行时根据角色 ID 获取要加载的模型名称
/// 模型须位于 Assets/Resources/Prefabs/Cars/ 下
/// </summary>
namespace Game.Runtime.Controller
{
    public static class CharacterModelMapper
    {
        private static readonly Dictionary<string, string> _characterToModel = new Dictionary<string, string>
        {
            { "mbt",       "suv" },
            { "scout",     "race-future" },
            { "spg",       "truck-flat" },
            { "apc",       "delivery" },
            { "td",        "police" },
            { "ifv",       "hatchback-sports" },
            { "aa",        "race" },
            { "flame",     "garbage-truck" },
            { "jeep",      "kart-oobi" },
            { "engineer",  "tractor-shovel" },
        };

        /// <summary>
        /// 根据角色 ID 获取对应 kenney_car 模型名称
        /// </summary>
        public static string GetModelName(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return "suv";

            if (_characterToModel.TryGetValue(characterId.ToLower(), out var model))
                return model;

            // 回退：如果 ID 本身就是模型名（如 "suv"），直接返回
            return characterId.ToLower();
        }

        /// <summary>
        /// 注册或覆盖角色→模型映射（供编辑器/扩展使用）
        /// </summary>
        public static void RegisterMapping(string characterId, string modelName)
        {
            _characterToModel[characterId.ToLower()] = modelName;
        }
    }
}
