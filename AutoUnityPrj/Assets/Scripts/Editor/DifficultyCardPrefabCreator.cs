#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.Editor
{
    public class DifficultyCardPrefabCreator
    {
        [MenuItem("铁皮突突/创建UI/创建难度卡片 Prefab", false, 4)]
        public static void CreateDifficultyCardPrefab()
        {
            // 根对象
            GameObject cardObj = new GameObject("DifficultyCardPrefab");
            
            // 添加 RectTransform (UI元素必须有)
            RectTransform cardRect = cardObj.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(120, 120);
            
            // Image (图标背景)
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(cardObj.transform, false);
            Image icon = iconObj.AddComponent<Image>();
            icon.color = Color.gray;
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.7f);
            iconRect.anchorMax = new Vector2(0.5f, 0.7f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(80, 60);

            // Name Text
            GameObject nameObj = new GameObject("Name");
            nameObj.transform.SetParent(cardObj.transform, false);
            Text nameText = nameObj.AddComponent<Text>();
            nameText.text = "难度";
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 24;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.MiddleCenter;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.5f, 0.2f);
            nameRect.anchorMax = new Vector2(0.5f, 0.2f);
            nameRect.pivot = new Vector2(0.5f, 0.5f);
            nameRect.anchoredPosition = Vector2.zero;
            nameRect.sizeDelta = new Vector2(100, 30);

            // Selected Highlight (选中高亮)
            GameObject highlightObj = new GameObject("SelectedHighlight");
            highlightObj.transform.SetParent(cardObj.transform, false);
            Image highlight = highlightObj.AddComponent<Image>();
            highlight.color = new Color(1f, 1f, 0f, 0.3f); // 黄色半透明
            highlight.gameObject.SetActive(false);
            RectTransform highlightRect = highlightObj.GetComponent<RectTransform>();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.offsetMin = Vector2.zero;
            highlightRect.offsetMax = Vector2.zero;

            // Locked Overlay
            GameObject lockedObj = new GameObject("LockedOverlay");
            lockedObj.transform.SetParent(cardObj.transform, false);
            Image locked = lockedObj.AddComponent<Image>();
            locked.color = new Color(0, 0, 0, 0.5f);
            locked.gameObject.SetActive(false);
            RectTransform lockedRect = lockedObj.GetComponent<RectTransform>();
            lockedRect.anchorMin = Vector2.zero;
            lockedRect.anchorMax = Vector2.one;
            lockedRect.offsetMin = Vector2.zero;
            lockedRect.offsetMax = Vector2.zero;

            // Button 组件
            Button btn = cardObj.AddComponent<Button>();
            btn.targetGraphic = icon;

            // DifficultyCardView 脚本
            DifficultyCardView card = cardObj.AddComponent<DifficultyCardView>();
            card._iconImage = icon;
            card._nameText = nameText;
            card._selectedHighlight = highlight;
            card._lockedOverlay = locked;

            // 保存为Prefab
            string path = "Assets/Resources/Prefabs/UI/DifficultyCardPrefab.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(cardObj, path);
            
            Debug.Log("[DifficultyCardPrefabCreator] Created: " + path);
            
            Object.DestroyImmediate(cardObj);
        }
    }
}
#endif