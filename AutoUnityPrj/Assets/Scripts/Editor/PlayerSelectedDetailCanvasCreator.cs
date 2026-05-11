#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 编辑器工具 - 创建玩家选择详情 Canvas Prefab
    /// 显示已选择的角色、武器、难度详情
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    public class PlayerSelectedDetailCanvasCreator
    {
        [MenuItem("铁皮突突/创建UI/创建玩家选择详情 Canvas Prefab", false, 2)]
        public static void CreatePlayerSelectedDetailCanvas()
        {
            // 1. 创建 Canvas
            GameObject canvasObj = new GameObject("PlayerSelectedDetailCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 2. 添加 CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // 3. 添加 GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 4. 添加 PlayerSelectedDetailView 脚本
            PlayerSelectedDetailView view = canvasObj.AddComponent<PlayerSelectedDetailView>();
            
            // 5. 创建主容器 (垂直布局)
            GameObject mainContainer = CreateUIObject("MainContainer", canvasObj);
            RectTransform mainRect = mainContainer.GetComponent<RectTransform>();
            mainRect.anchorMin = new Vector2(0.5f, 0.5f);
            mainRect.anchorMax = new Vector2(0.5f, 0.5f);
            mainRect.pivot = new Vector2(0.5f, 0.5f);
            mainRect.anchoredPosition = Vector2.zero;
            mainRect.sizeDelta = new Vector2(800, 500);
            
            VerticalLayoutGroup mainLayout = mainContainer.AddComponent<VerticalLayoutGroup>();
            mainLayout.childAlignment = TextAnchor.UpperCenter;
            mainLayout.childForceExpandWidth = true;
            mainLayout.childForceExpandHeight = false;
            mainLayout.spacing = 20;
            mainLayout.padding = new RectOffset(20, 20, 20, 20);
            
            // ========== 角色详情区域 ==========
            GameObject characterSection = CreateUIObject("CharacterSection", mainContainer);
            SetupSection(characterSection, "角色详情", new Color(0.2f, 0.6f, 1f));
            
            // 角色内容容器 (水平布局)
            GameObject characterContent = CreateUIObject("CharacterContent", characterSection);
            HorizontalLayoutGroup charContentLayout = characterContent.AddComponent<HorizontalLayoutGroup>();
            charContentLayout.childAlignment = TextAnchor.MiddleLeft;
            charContentLayout.childForceExpandWidth = true;
            charContentLayout.childForceExpandHeight = true;
            charContentLayout.spacing = 20;
            
            // 角色图标
            GameObject characterIconObj = CreateUIObject("Icon", characterContent);
            // RectTransform 已在 CreateUIObject 中添加
            RectTransform charIconRect = characterIconObj.GetComponent<RectTransform>();
            charIconRect.sizeDelta = new Vector2(100, 100);
            Image charIcon = characterIconObj.AddComponent<Image>();
            charIcon.color = Color.gray;
            
            // 角色信息容器 (垂直)
            GameObject characterInfo = CreateUIObject("CharacterInfo", characterContent);
            VerticalLayoutGroup charInfoLayout = characterInfo.AddComponent<VerticalLayoutGroup>();
            charInfoLayout.childAlignment = TextAnchor.UpperLeft;
            charInfoLayout.childForceExpandWidth = true;
            charInfoLayout.spacing = 10;
            
            // 角色名字
            GameObject charNameObj = CreateUIObject("NameText", characterInfo);
            Text charName = charNameObj.AddComponent<Text>();
            charName.text = "角色名称";
            charName.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            charName.fontSize = 28;
            charName.color = Color.white;
            charName.alignment = TextAnchor.MiddleLeft;
            SetupRectTransform(charNameObj, 300, 40);
            
            // 角色类型
            GameObject charTypeObj = CreateUIObject("TypeText", characterInfo);
            Text charType = charTypeObj.AddComponent<Text>();
            charType.text = "类型";
            charType.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            charType.fontSize = 20;
            charType.color = Color.cyan;
            charType.alignment = TextAnchor.MiddleLeft;
            SetupRectTransform(charTypeObj, 300, 30);
            
            // 角色描述
            GameObject charDescObj = CreateUIObject("DescriptionText", characterInfo);
            Text charDesc = charDescObj.AddComponent<Text>();
            charDesc.text = "角色详情描述...";
            charDesc.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            charDesc.fontSize = 18;
            charDesc.color = Color.gray;
            charDesc.alignment = TextAnchor.UpperLeft;
            charDesc.supportRichText = true;
            SetupRectTransform(charDescObj, 400, 120);
            
            // ========== 武器详情区域 ==========
            GameObject weaponSection = CreateUIObject("WeaponSection", mainContainer);
            SetupSection(weaponSection, "武器详情", new Color(1f, 0.6f, 0.2f));
            
            // 武器内容容器
            GameObject weaponContent = CreateUIObject("WeaponContent", weaponSection);
            HorizontalLayoutGroup weaponContentLayout = weaponContent.AddComponent<HorizontalLayoutGroup>();
            weaponContentLayout.childAlignment = TextAnchor.MiddleLeft;
            weaponContentLayout.childForceExpandWidth = true;
            weaponContentLayout.childForceExpandHeight = true;
            weaponContentLayout.spacing = 20;
            
            // 武器图标
            GameObject weaponIconObj = CreateUIObject("Icon", weaponContent);
            // RectTransform 已在 CreateUIObject 中添加
            RectTransform weaponIconRect = weaponIconObj.GetComponent<RectTransform>();
            weaponIconRect.sizeDelta = new Vector2(80, 80);
            Image weaponIcon = weaponIconObj.AddComponent<Image>();
            weaponIcon.color = new Color(1f, 0.6f, 0.2f, 0.5f);
            
            // 武器信息容器
            GameObject weaponInfo = CreateUIObject("WeaponInfo", weaponContent);
            VerticalLayoutGroup weaponInfoLayout = weaponInfo.AddComponent<VerticalLayoutGroup>();
            weaponInfoLayout.childAlignment = TextAnchor.UpperLeft;
            weaponInfoLayout.childForceExpandWidth = true;
            weaponInfoLayout.spacing = 8;
            
            // 武器名字
            GameObject weaponNameObj = CreateUIObject("NameText", weaponInfo);
            Text weaponName = weaponNameObj.AddComponent<Text>();
            weaponName.text = "武器名称";
            weaponName.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            weaponName.fontSize = 24;
            weaponName.color = Color.white;
            weaponName.alignment = TextAnchor.MiddleLeft;
            SetupRectTransform(weaponNameObj, 250, 35);
            
            // 武器类型
            GameObject weaponTypeObj = CreateUIObject("TypeText", weaponInfo);
            Text weaponType = weaponTypeObj.AddComponent<Text>();
            weaponType.text = "远程";
            weaponType.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            weaponType.fontSize = 18;
            weaponType.color = Color.yellow;
            weaponType.alignment = TextAnchor.MiddleLeft;
            SetupRectTransform(weaponTypeObj, 250, 28);
            
            // 武器描述
            GameObject weaponDescObj = CreateUIObject("DescriptionText", weaponInfo);
            Text weaponDesc = weaponDescObj.AddComponent<Text>();
            weaponDesc.text = "伤害: 10.0\n攻速: 1.0/s\n范围: 5.0\n等级: 1/5";
            weaponDesc.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            weaponDesc.fontSize = 16;
            weaponDesc.color = Color.gray;
            weaponDesc.alignment = TextAnchor.UpperLeft;
            SetupRectTransform(weaponDescObj, 300, 100);
            
            // ========== 难度详情区域 ==========
            GameObject difficultySection = CreateUIObject("DifficultySection", mainContainer);
            SetupSection(difficultySection, "难度详情", new Color(0.8f, 0.2f, 0.2f));
            
            // 难度内容容器
            GameObject difficultyContent = CreateUIObject("DifficultyContent", difficultySection);
            HorizontalLayoutGroup diffContentLayout = difficultyContent.AddComponent<HorizontalLayoutGroup>();
            diffContentLayout.childAlignment = TextAnchor.MiddleLeft;
            diffContentLayout.childForceExpandWidth = true;
            diffContentLayout.childForceExpandHeight = true;
            diffContentLayout.spacing = 20;
            
            // 难度图标
            GameObject diffIconObj = CreateUIObject("Icon", difficultyContent);
            // RectTransform 已在 CreateUIObject 中添加
            RectTransform diffIconRect = diffIconObj.GetComponent<RectTransform>();
            diffIconRect.sizeDelta = new Vector2(60, 60);
            Image diffIcon = diffIconObj.AddComponent<Image>();
            diffIcon.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
            
            // 难度信息容器
            GameObject diffInfo = CreateUIObject("DifficultyInfo", difficultyContent);
            VerticalLayoutGroup diffInfoLayout = diffInfo.AddComponent<VerticalLayoutGroup>();
            diffInfoLayout.childAlignment = TextAnchor.UpperLeft;
            diffInfoLayout.childForceExpandWidth = true;
            diffInfoLayout.spacing = 8;
            
            // 难度名字
            GameObject diffNameObj = CreateUIObject("NameText", diffInfo);
            Text diffName = diffNameObj.AddComponent<Text>();
            diffName.text = "普通";
            diffName.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            diffName.fontSize = 24;
            diffName.color = Color.white;
            diffName.alignment = TextAnchor.MiddleLeft;
            SetupRectTransform(diffNameObj, 200, 35);
            
            // 难度类型
            GameObject diffTypeObj = CreateUIObject("TypeText", diffInfo);
            Text diffType = diffTypeObj.AddComponent<Text>();
            diffType.text = "难度等级";
            diffType.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            diffType.fontSize = 18;
            diffType.color = Color.red;
            diffType.alignment = TextAnchor.MiddleLeft;
            SetupRectTransform(diffTypeObj, 200, 28);
            
            // 难度描述
            GameObject diffDescObj = CreateUIObject("DescriptionText", diffInfo);
            Text diffDesc = diffDescObj.AddComponent<Text>();
            diffDesc.text = "标准难度，平衡的挑战";
            diffDesc.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            diffDesc.fontSize = 16;
            diffDesc.color = Color.gray;
            diffDesc.alignment = TextAnchor.UpperLeft;
            SetupRectTransform(diffDescObj, 400, 60);
            
            // 6. 保存为 Prefab
            string prefabPath = "Assets/Resources/Prefabs/UI/PlayerSelectedDetailCanvas.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
            
            Debug.Log($"[PlayerSelectedDetailCanvasCreator] Prefab 已创建: {prefabPath}");
            
            // 7. 销毁场景中的临时对象
            Object.DestroyImmediate(canvasObj);
        }
        
        /// <summary>
        /// 创建UI对象并添加RectTransform
        /// </summary>
        private static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent.transform, false);
            obj.AddComponent<RectTransform>();
            return obj;
        }
        
        /// <summary>
        /// 设置区域标题
        /// </summary>
        private static void SetupSection(GameObject section, string title, Color color)
        {
            // 背景
            Image bg = section.AddComponent<Image>();
            bg.color = new Color(color.r, color.g, color.b, 0.15f);
            
            RectTransform rect = section.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 120);
            
            // 标题
            GameObject titleObj = CreateUIObject("SectionTitle", section);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = title;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 22;
            titleText.color = color;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -10);
            titleRect.sizeDelta = new Vector2(0, 30);
        }
        
        /// <summary>
        /// 设置RectTransform尺寸
        /// </summary>
        private static void SetupRectTransform(GameObject obj, float width, float height)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}
#endif