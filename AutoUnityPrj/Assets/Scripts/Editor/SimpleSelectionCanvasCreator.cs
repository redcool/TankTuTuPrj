#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.Editor
{
    public class SimpleSelectionCanvasCreator
    {
        [MenuItem("铁皮突突/创建UI/创建选择流程 Canvas", false, 3)]
        public static void CreateSelectionCanvas()
        {
            GameObject canvasObj = new GameObject("SelectionCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Character Panel
            GameObject charPanel = CreatePanel("CharacterPanel", canvasObj);
            
            GameObject charTitle = CreateText("Title", charPanel, "选择角色", 40, Color.white);
            var charTitleRect = charTitle.GetComponent<RectTransform>();
            charTitleRect.anchorMin = new Vector2(0.5f, 1);
            charTitleRect.anchorMax = new Vector2(0.5f, 1);
            charTitleRect.pivot = new Vector2(0.5f, 1);
            charTitleRect.anchoredPosition = new Vector2(0, -50);
            charTitleRect.sizeDelta = new Vector2(600, 50);
            
            GameObject charGrid = CreateGrid("CharacterGrid", charPanel, new Vector2(100, 120), new Vector2(20, 20));
            charGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
            charGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 300);
            
            // Add view controller - 使用新的 View 架构
            // CharacterSelectionController 已移除，改用 CharacterSelectView + PlayerSelectionControl
            
            // Weapon Panel (hidden)
            GameObject weaponPanel = CreatePanel("WeaponPanel", canvasObj);
            weaponPanel.SetActive(false);
            
            GameObject weaponTitle = CreateText("Title", weaponPanel, "选择武器", 40, Color.white);
            var weaponTitleRect = weaponTitle.GetComponent<RectTransform>();
            weaponTitleRect.anchorMin = new Vector2(0.5f, 1);
            weaponTitleRect.anchorMax = new Vector2(0.5f, 1);
            weaponTitleRect.pivot = new Vector2(0.5f, 1);
            weaponTitleRect.anchoredPosition = new Vector2(0, -50);
            weaponTitleRect.sizeDelta = new Vector2(600, 50);
            
            GameObject weaponGrid = CreateGrid("WeaponGrid", weaponPanel, new Vector2(100, 120), new Vector2(20, 20));
            weaponGrid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
            weaponGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 300);
            
            weaponPanel.AddComponent<WeaponSelectionController>();
            
            // Difficulty Panel (hidden)
            GameObject diffPanel = CreatePanel("DifficultyPanel", canvasObj);
            diffPanel.SetActive(false);
            
            GameObject diffTitle = CreateText("Title", diffPanel, "选择难度", 40, Color.white);
            var diffTitleRect = diffTitle.GetComponent<RectTransform>();
            diffTitleRect.anchorMin = new Vector2(0.5f, 1);
            diffTitleRect.anchorMax = new Vector2(0.5f, 1);
            diffTitleRect.pivot = new Vector2(0.5f, 1);
            diffTitleRect.anchoredPosition = new Vector2(0, -50);
            diffTitleRect.sizeDelta = new Vector2(600, 50);
            
            GameObject diffGrid = CreateHorizontalGrid("DifficultyGrid", diffPanel, 30);
            diffGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            diffGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 120);
            
            string[] diffNames = { "简单", "普通", "困难", "梦魇" };
            Color[] diffColors = { Color.green, Color.blue, new Color(1f, 0.5f, 0f), Color.red };
            
            for (int i = 0; i < 4; i++)
            {
                CreateDifficultyButton(diffNames[i], diffColors[i], diffGrid);
            }
            
            // 不再添加 DifficultySelectionController (已由 DifficultySelectionView 处理)
            
            // Save
            string path = "Assets/Resources/Prefabs/UI/SelectionCanvas.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(canvasObj, path);
            
            Debug.Log("[SimpleSelectionCanvasCreator] Created: " + path);
            Object.DestroyImmediate(canvasObj);
        }
        
        static GameObject CreatePanel(string name, GameObject parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent.transform, false);
            panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            return panel;
        }
        
        static GameObject CreateText(string name, GameObject parent, string content, int size, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent.transform, false);
            Text text = obj.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            obj.AddComponent<RectTransform>();
            return obj;
        }
        
        static GameObject CreateGrid(string name, GameObject parent, Vector2 cellSize, Vector2 spacing)
        {
            GameObject grid = new GameObject(name);
            grid.transform.SetParent(parent.transform, false);
            GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
            layout.cellSize = cellSize;
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            grid.AddComponent<RectTransform>();
            return grid;
        }
        
        static GameObject CreateHorizontalGrid(string name, GameObject parent, float spacing)
        {
            GameObject grid = new GameObject(name);
            grid.transform.SetParent(parent.transform, false);
            HorizontalLayoutGroup layout = grid.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.spacing = spacing;
            grid.AddComponent<RectTransform>();
            return grid;
        }
        
        static void CreateDifficultyButton(string name, Color color, GameObject parent)
        {
            GameObject btn = new GameObject(name);
            btn.transform.SetParent(parent.transform, false);
            btn.AddComponent<Image>().color = new Color(color.r, color.g, color.b, 0.3f);
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 100);
            
            GameObject label = new GameObject("Label");
            label.transform.SetParent(btn.transform, false);
            Text text = label.AddComponent<Text>();
            text.text = name;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
        }
    }
}
#endif