#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 编辑器工具 - 一键创建 HUD Canvas Prefab
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    public class HUDCanvasCreator
    {
        [MenuItem("铁皮突突/创建UI/创建 HUD Canvas Prefab", false, 1)]
        public static void CreateHUDCanvas()
        {
            // 1. 创建 Canvas
            GameObject canvasObj = new GameObject("HUDCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 2. 添加 CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // 3. 添加 GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 4. 添加 HUDView
            canvasObj.AddComponent<HUDView>();
            
            // 5. 创建 Timer Text (右上角)
            GameObject timerObj = new GameObject("TimerText");
            timerObj.transform.SetParent(canvasObj.transform, false);
            RectTransform timerRect = timerObj.AddComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0, 1);
            timerRect.anchorMax = new Vector2(0, 1);
            timerRect.pivot = new Vector2(0, 1);
            timerRect.anchoredPosition = new Vector2(20, -20);
            timerRect.sizeDelta = new Vector2(200, 50);
            Text timerText = timerObj.AddComponent<Text>();
            timerText.text = "00:00";
            timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            timerText.fontSize = 32;
            timerText.color = Color.white;
            
            // 6. 创建 Health Text (左上角)
            GameObject healthObj = new GameObject("HealthText");
            healthObj.transform.SetParent(canvasObj.transform, false);
            RectTransform healthRect = healthObj.AddComponent<RectTransform>();
            healthRect.anchorMin = new Vector2(1, 1);
            healthRect.anchorMax = new Vector2(1, 1);
            healthRect.pivot = new Vector2(1, 1);
            healthRect.anchoredPosition = new Vector2(-20, -20);
            healthRect.sizeDelta = new Vector2(200, 50);
            Text healthText = healthObj.AddComponent<Text>();
            healthText.text = "100/100";
            healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            healthText.fontSize = 28;
            healthText.color = Color.green;
            
            // 7. 创建 Resource Text (左下角)
            GameObject resourceObj = new GameObject("ResourceText");
            resourceObj.transform.SetParent(canvasObj.transform, false);
            RectTransform resourceRect = resourceObj.AddComponent<RectTransform>();
            resourceRect.anchorMin = new Vector2(0, 0);
            resourceRect.anchorMax = new Vector2(0, 0);
            resourceRect.pivot = new Vector2(0, 0);
            resourceRect.anchoredPosition = new Vector2(20, 20);
            resourceRect.sizeDelta = new Vector2(200, 50);
            Text resourceText = resourceObj.AddComponent<Text>();
            resourceText.text = "0";
            resourceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            resourceText.fontSize = 28;
            resourceText.color = Color.yellow;
            
            // 8. 创建 Wave Text
            GameObject waveObj = new GameObject("WaveText");
            waveObj.transform.SetParent(canvasObj.transform, false);
            RectTransform waveRect = waveObj.AddComponent<RectTransform>();
            waveRect.anchorMin = new Vector2(1, 0.5f);
            waveRect.anchorMax = new Vector2(1, 0.5f);
            waveRect.pivot = new Vector2(1, 0.5f);
            waveRect.anchoredPosition = new Vector2(-20, 0);
            waveRect.sizeDelta = new Vector2(150, 40);
            Text waveText = waveObj.AddComponent<Text>();
            waveText.text = "Wave 1/10";
            waveText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            waveText.fontSize = 24;
            waveText.color = Color.cyan;
            
            // 9. 保存为 Prefab
            string prefabPath = "Assets/Resources/Prefabs/UI/HUDCanvas.prefab";
            System.IO.Directory.CreateDirectory("Assets/Resources/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(canvasObj, prefabPath);
            
            Debug.Log($"[HUDCanvasCreator] Prefab 已创建: {prefabPath}");
            
            // 10. 销毁场景中的临时对象
            Object.DestroyImmediate(canvasObj);
        }
    }
}
#endif