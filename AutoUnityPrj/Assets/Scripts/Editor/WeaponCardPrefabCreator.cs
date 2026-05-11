#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.Editor
{
    public class WeaponCardPrefabCreator
    {
        [MenuItem("铁皮突突/创建UI/创建武器卡片 Prefab", false, 4)]
        public static void CreateWeaponCardPrefab()
        {
            // 根对象
            GameObject cardObj = new GameObject("WeaponCardPrefab");
            
            // 添加 RectTransform (UI元素必须有)
            RectTransform cardRect = cardObj.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(80, 80);
            
            // Image (图标)
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(cardObj.transform, false);
            Image icon = iconObj.AddComponent<Image>();
            icon.color = Color.gray;
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

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

            // WeaponCard 脚本
            WeaponCard card = cardObj.AddComponent<WeaponCard>();
            card._iconImage = icon;
            card._selectedHighlight = highlight;
            card._lockedOverlay = locked;

            // 保存为Prefab
            string path = "Assets/Resources/Prefabs/UI/WeaponCardPrefab.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(cardObj, path);
            
            Debug.Log("[WeaponCardPrefabCreator] Created: " + path);
            
            Object.DestroyImmediate(cardObj);
        }

        [MenuItem("铁皮突突/创建UI/创建武器选择 Canvas Prefab", false, 5)]
        public static void CreateWeaponSelectionCanvas()
        {
            // Canvas
            GameObject canvasObj = new GameObject("WeaponSelectionCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 背景
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvasObj.transform, false);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.85f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = Vector2.zero;
            
            // 标题
            GameObject title = new GameObject("Title");
            title.transform.SetParent(canvasObj.transform, false);
            Text titleText = title.AddComponent<Text>();
            titleText.text = "选择武器";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 40;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -50);
            titleRect.sizeDelta = new Vector2(600, 50);
            
            // 返回按钮
            GameObject backBtn = new GameObject("BackButton");
            backBtn.transform.SetParent(canvasObj.transform, false);
            Image backImg = backBtn.AddComponent<Image>();
            backImg.color = Color.gray;
            Button backButton = backBtn.AddComponent<Button>();
            RectTransform backRect = backBtn.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0, 1);
            backRect.anchorMax = new Vector2(0, 1);
            backRect.pivot = new Vector2(0, 1);
            backRect.anchoredPosition = new Vector2(50, -50);
            backRect.sizeDelta = new Vector2(150, 50);
            
            GameObject backLabel = new GameObject("Label");
            backLabel.transform.SetParent(backBtn.transform, false);
            Text backText = backLabel.AddComponent<Text>();
            backText.text = "返回";
            backText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            backText.fontSize = 24;
            backText.color = Color.white;
            backText.alignment = TextAnchor.MiddleCenter;
            RectTransform backLabelRect = backLabel.GetComponent<RectTransform>();
            backLabelRect.anchorMin = Vector2.zero;
            backLabelRect.anchorMax = Vector2.one;
            backLabelRect.offsetMin = Vector2.zero;
            backLabelRect.offsetMax = Vector2.zero;
            
            // 武器网格容器
            GameObject weaponGrid = new GameObject("WeaponGrid");
            weaponGrid.transform.SetParent(canvasObj.transform, false);
            RectTransform gridRect = weaponGrid.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.anchoredPosition = new Vector2(0, 0);
            gridRect.sizeDelta = new Vector2(900, 400);
            
            // GridLayoutGroup - 每行10个
            GridLayoutGroup gridLayout = weaponGrid.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(80, 80);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 10;
            
            // WeaponSelectionView 脚本
            WeaponSelectionView view = canvasObj.AddComponent<WeaponSelectionView>();
            view._weaponGridPath = "WeaponGrid";
            view._backButtonPath = "BackButton";
            view._titleTextPath = "Title";
            
            // 保存Prefab
            string prefabPath = "Assets/Resources/Prefabs/UI/WeaponSelectionCanvas.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
            
            Debug.Log("[WeaponSelectionCanvasCreator] Created: " + prefabPath);
            
            Object.DestroyImmediate(canvasObj);
        }
    }
}
#endif