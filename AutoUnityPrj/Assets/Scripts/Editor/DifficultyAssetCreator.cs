#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Editor
{
    public class DifficultyAssetCreator
    {
        [MenuItem("铁皮突突/创建资源/创建难度配置文件 (0-6)", false, 10)]
        public static void CreateDifficultyAssets()
        {
            string[] names = { "新手", "简单", "普通", "困难", "专家", "大师", "梦魇" };
            string[] descs = {
                "适合初次接触游戏的新手，敌人较弱",
                "适合放松娱乐，敌人强度较低",
                "标准难度，平衡体验",
                "需要一定技巧和反应",
                "富有挑战性，适合熟练玩家",
                "只有高手能通过",
                "极限挑战，超越自我"
            };
            
            Color[] colors = {
                Color.gray,
                Color.green,
                Color.blue,
                new Color(1f, 0.5f, 0f),
                Color.red,
                new Color(0.5f, 0f, 0.5f),
                new Color(1f, 0f, 1f)
            };
            
            float[,] multipliers = {
                // enemyCount, enemyHp, enemySpeed, enemyDamage, spawnInterval, dropRate, exp
                { 0.8f, 1.0f, 0.9f, 0.8f, 1.2f, 1.2f, 1.0f },  // 0: 新手
                { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },  // 1: 简单
                { 1.2f, 1.3f, 1.1f, 1.2f, 0.9f, 1.0f, 1.1f },  // 2: 普通
                { 1.5f, 1.6f, 1.2f, 1.5f, 0.8f, 0.9f, 1.3f },  // 3: 困难
                { 2.0f, 2.0f, 1.3f, 2.0f, 0.7f, 0.8f, 1.5f },  // 4: 专家
                { 2.5f, 2.5f, 1.4f, 2.5f, 0.6f, 0.7f, 1.8f },  // 5: 大师
                { 3.0f, 3.0f, 1.5f, 3.0f, 0.5f, 0.6f, 2.0f }   // 6: 梦魇
            };
            
            bool[,] features = {
                // showHealthBar, hasElite, hasBoss
                { false, false, false },  // 0: 新手
                { false, false, false },  // 1: 简单
                { true, false, false },  // 2: 普通
                { true, true, false },  // 3: 困难
                { true, true, false },  // 4: 专家
                { true, true, false },  // 5: 大师
                { true, true, true }    // 6: 梦魇
            };
            
            string folder = "Assets/Resources/ScriptableObjects/Difficulties";
            System.IO.Directory.CreateDirectory(folder);
            
            for (int i = 0; i < 7; i++)
            {
                var difficulty = ScriptableObject.CreateInstance<DifficultyDataSO>();
                
                // 使用反射设置私有字段
                SetField(difficulty, "_difficultyName", names[i]);
                SetField(difficulty, "_description", descs[i]);
                SetField(difficulty, "_difficultyLevel", i);
                SetField(difficulty, "_enemyCountMultiplier", multipliers[i, 0]);
                SetField(difficulty, "_enemyHpMultiplier", multipliers[i, 1]);
                SetField(difficulty, "_enemySpeedMultiplier", multipliers[i, 2]);
                SetField(difficulty, "_enemyDamageMultiplier", multipliers[i, 3]);
                SetField(difficulty, "_spawnIntervalMultiplier", multipliers[i, 4]);
                SetField(difficulty, "_dropRateMultiplier", multipliers[i, 5]);
                SetField(difficulty, "_expMultiplier", multipliers[i, 6]);
                SetField(difficulty, "_showEnemyHealthBar", features[i, 0]);
                SetField(difficulty, "_hasEliteEnemies", features[i, 1]);
                SetField(difficulty, "_hasBoss", features[i, 2]);
                
                string path = $"{folder}/Difficulty_{i}_{names[i]}.asset";
                AssetDatabase.CreateAsset(difficulty, path);
                
                Debug.Log($"[DifficultyAssetCreator] Created: Difficulty_{i}_{names[i]}.asset");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[DifficultyAssetCreator] 已创建 7 个难度配置文件");
        }
        
        private static void SetField(ScriptableObject obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
#endif