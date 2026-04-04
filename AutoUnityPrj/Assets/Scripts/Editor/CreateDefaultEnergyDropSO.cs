#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Game.Runtime.Editor
{
    public static class CreateDefaultEnergyDropSO
    {
        [MenuItem("铁皮突突/创建默认能量块掉落数据", false, 2)]
        public static void Create()
        {
            var so = ScriptableObject.CreateInstance<Game.Runtime.ValueObject.ScriptableObjects.EnergyDropDataSO>();
            if (so != null)
            {
                so.name = "DefaultEnergyDrop";
                AssetDatabase.CreateAsset(so, "Assets/ScriptableObjects/EnergyDrop/DefaultEnergyDrop.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("[CreateDefaultEnergyDropSO] 创建 DefaultEnergyDrop.asset 成功");
            }
        }
    }
}
#endif
