#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 为武器数据SO设置图标
    /// 使用 weaponIcons.png 中的 sprite，按顺序分配给武器
    /// </summary>
    public class AssignWeaponIcons
    {
        [MenuItem("铁皮突突/创建数据/设置武器图标", false, 2)]
        public static void AssignIcons()
        {
            string folder = "Assets/Resources/ScriptableObjects/Weapons";
            
            if (!System.IO.Directory.Exists(folder))
            {
                Debug.LogError($"[AssignWeaponIcons] 武器文件夹不存在: {folder}");
                return;
            }

            // 获取所有 weapon asset 文件
            string[] assetPaths = System.IO.Directory.GetFiles(folder, "*.asset");
            
            // 按名称排序以确保顺序一致
            System.Array.Sort(assetPaths);

            Debug.Log($"[AssignWeaponIcons] 找到 {assetPaths.Length} 个武器资产");

            // sprite 0-4 是箱子,武器从 sprite 5 开始
            const int spriteOffset = 5;

            // 为每个武器分配图标
            for (int i = 0; i < assetPaths.Length; i++)
            {
                string assetPath = assetPaths[i];
                WeaponDataSO weapon = AssetDatabase.LoadAssetAtPath<WeaponDataSO>(assetPath);
                
                if (weapon != null)
                {
                    // 加载对应索引的 sprite (加上偏移)
                    int spriteIndex = i + spriteOffset;
                    string spriteName = $"weaponIcons_{spriteIndex}";
                    Sprite sprite = LoadSpriteByName(spriteName);
                    
                    if (sprite != null)
                    {
                        weapon.IconSetter = sprite;
                        EditorUtility.SetDirty(weapon);
                        Debug.Log($"[AssignWeaponIcons] 设置图标: {weapon.WeaponName} -> {spriteName}");
                    }
                    else
                    {
                        Debug.LogWarning($"[AssignWeaponIcons] 无法加载 sprite: {spriteName}");
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[AssignWeaponIcons] 完成!");
        }

        private static Sprite LoadSpriteByName(string spriteName)
        {
            // 通过 GUID 查找 sprite
            string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite", new string[] { "Assets/Arts/Icons" });
            
            if (guids.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            
            return null;
        }
    }
}
#endif