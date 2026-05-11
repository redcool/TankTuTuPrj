#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 创建难度选择 Canvas
    /// 只包含难度选择部分，供游戏选择流程使用
    /// </summary>
    public class DifficultySelectionCanvasCreator
    {
        [MenuItem("铁皮突突/创建UI/创建难度选择 Canvas Prefab", false, 6)]
        public static void CreateDifficultySelectionCanvas()
        {
            // Canvas
            GameObject canvasObj = new GameObject("DifficultySelectionCanvas");
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
            titleText.text = "选择难度";
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
            
            // 难度选项容器 (水平排列)
            GameObject difficultyContainer = new GameObject("DifficultyContainer");
            difficultyContainer.transform.SetParent(canvasObj.transform, false);
            HorizontalLayoutGroup hLayout = difficultyContainer.AddComponent<HorizontalLayoutGroup>();
            hLayout.childAlignment = TextAnchor.MiddleCenter;
            hLayout.childForceExpandWidth = true;
            hLayout.childForceExpandHeight = true;
            hLayout.spacing = 30;
            hLayout.padding.left = 50;
            hLayout.padding.right = 50;
            // 获取已有的 RectTransform (新建 GameObject 时已自动添加)
            RectTransform containerRect = difficultyContainer.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(800, 150);
            
            // 难度选项配置
            string[] diffNames = { "简单", "普通", "困难", "梦魇" };
            Color[] diffColors = { Color.green, Color.blue, new Color(1f, 0.5f, 0f), Color.red };
            string[] diffDescs = { "适合新手", "标准挑战", "高难度", "极限模式" };
            
            for (int i = 0; i < 4; i++)
            {
                CreateDifficultyButton(diffNames[i], diffColors[i], diffDescs[i], difficultyContainer);
            }
            
            // DifficultySelectionView 脚本 (已处理全部逻辑)
            DifficultySelectionView view = canvasObj.AddComponent<DifficultySelectionView>();
            // _backButton 会通过 FindUIElements 自动获取
            
            // 保存Prefab
            string prefabPath = "Assets/Resources/Prefabs/UI/DifficultySelectionCanvas.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
            
            Debug.Log("[DifficultySelectionCanvasCreator] Created: " + prefabPath);
            
            Object.DestroyImmediate(canvasObj);
        }
        
        private static void CreateDifficultyButton(string name, Color color, string desc, GameObject parent)
        {
            // 按钮容器
            GameObject btnContainer = new GameObject(name);
            btnContainer.transform.SetParent(parent.transform, false);
            
            // 背景图片
            Image bg = btnContainer.AddComponent<Image>();
            bg.color = new Color(color.r, color.g, color.b, 0.3f);
            
            RectTransform rect = btnContainer.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 120);
            
            // 按钮组件
            Button btn = btnContainer.AddComponent<Button>();
            btn.targetGraphic = bg;
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(color.r, color.g, color.b, 0.3f);
            colors.highlightedColor = new Color(color.r, color.g, color.b, 0.5f);
            colors.pressedColor = new Color(color.r, color.g, color.b, 0.7f);
            btn.colors = colors;
            
            // 垂直布局
            VerticalLayoutGroup vLayout = btnContainer.AddComponent<VerticalLayoutGroup>();
            vLayout.childAlignment = TextAnchor.MiddleCenter;
            vLayout.childForceExpandWidth = true;
            vLayout.childForceExpandHeight = true;
            vLayout.spacing = 10;
            
            // 名称文本
            GameObject nameObj = new GameObject("Name");
            nameObj.transform.SetParent(btnContainer.transform, false);
            Text nameText = nameObj.AddComponent<Text>();
            nameText.text = name;
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 28;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.MiddleCenter;
            
            // 描述文本
            GameObject descObj = new GameObject("Desc");
            descObj.transform.SetParent(btnContainer.transform, false);
            Text descText = descObj.AddComponent<Text>();
            descText.text = desc;
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 16;
            descText.color = new Color(1f, 1f, 1f, 0.7f);
            descText.alignment = TextAnchor.MiddleCenter;
            
            // 添加 SelectionItem 组件
            btnContainer.AddComponent<SelectionItem>();
        }
    }
}
#endif