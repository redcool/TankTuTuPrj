using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 场景初始化器 - 从Resources加载预制体并通过属性setter连接管理器引用
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        private void Awake()
        {
            InitializeEnemySpawner();
            InitializeLevelManager();

            Debug.Log("[SceneInitializer] 场景初始化完成");
        }

        private void Start()
        {
            // GameManager.Instance 在 Awake 中设置，Start 中调用确保实例已就绪
            InitializeGameManager();

            // 所有引用连接完成后开始关卡
            var levelManager = FindObjectOfType<LevelManager>();
            levelManager?.StartLevel();
        }

        /// <summary>
        /// 初始化EnemySpawner - 从Resources加载敌人预制体
        /// </summary>
        private void InitializeEnemySpawner()
        {
            var spawner = FindObjectOfType<EnemySpawner>();
            if (spawner == null)
            {
                Debug.LogWarning("[SceneInitializer] 未找到EnemySpawner");
                return;
            }

            // 从Resources加载敌人预制体
            var beaverPrefab = Resources.Load<GameObject>("Prefabs/Monsters/Common/animal-beaver");
            var cowPrefab = Resources.Load<GameObject>("Prefabs/Monsters/Common/animal-cow");
            var elephantPrefab = Resources.Load<GameObject>("Prefabs/Monsters/Boss/animal-elephant");

            if (beaverPrefab != null)
            {
                spawner.BeaverPrefab = beaverPrefab;
            }
            else
            {
                Debug.LogWarning("[SceneInitializer] 未找到 animal-beaver 预制体");
            }

            if (cowPrefab != null)
            {
                spawner.CowPrefab = cowPrefab;
            }
            else
            {
                Debug.LogWarning("[SceneInitializer] 未找到 animal-cow 预制体");
            }

            if (elephantPrefab != null)
            {
                spawner.ElephantBossPrefab = elephantPrefab;
            }
            else
            {
                Debug.LogWarning("[SceneInitializer] 未找到 animal-elephant 预制体");
            }

            Debug.Log("[SceneInitializer] EnemySpawner 初始化完成");
        }

        /// <summary>
        /// 初始化LevelManager
        /// </summary>
        private void InitializeLevelManager()
        {
            var levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogWarning("[SceneInitializer] 未找到LevelManager");
                return;
            }

            var spawner = FindObjectOfType<EnemySpawner>();
            if (spawner != null)
            {
                levelManager.EnemySpawnerRef = spawner;
            }

            var hudView = FindObjectOfType<Game.Runtime.View.HUDView>();
            if (hudView != null)
            {
                levelManager.HUDViewRef = hudView;
            }

            var tanks = FindObjectsOfType<TankController>();
            if (tanks.Length > 0)
            {
                levelManager.PlayerTanks = tanks;
            }

            // 查找并设置ResultView
            var resultView = FindObjectOfType<Game.Runtime.View.ResultView>();
            if (resultView != null)
            {
                levelManager.ResultViewRef = resultView;
            }

            Debug.Log("[SceneInitializer] LevelManager 初始化完成");
        }

        /// <summary>
        /// 初始化GameManager
        /// </summary>
        private void InitializeGameManager()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning("[SceneInitializer] 未找到GameManager");
                return;
            }

            // 从Resources加载资源预制体
            var energyDropPrefab = Resources.Load<GameObject>("Prefabs/Items/Block/goods1");
            var treasureBoxPrefab = Resources.Load<GameObject>("Prefabs/Items/Box/TreasureBox1");

            if (energyDropPrefab != null)
            {
                gm.EnergyDropPrefab = energyDropPrefab;
            }
            else
            {
                Debug.LogWarning("[SceneInitializer] 未找到 goods1 预制体");
            }

            if (treasureBoxPrefab != null)
            {
                gm.TreasureBoxPrefab = treasureBoxPrefab;
            }
            else
            {
                Debug.LogWarning("[SceneInitializer] 未找到 TreasureBox1 预制体");
            }

            // 加载能量块掉落数据 SO
            var energyDropData = Resources.Load<Game.Runtime.ValueObject.ScriptableObjects.EnergyDropDataSO>("ScriptableObjects/EnergyDrop/DefaultEnergyDrop");
            if (energyDropData != null)
            {
                gm.EnergyDropData = energyDropData;
            }
            else
            {
                Debug.LogWarning("[SceneInitializer] 未找到 DefaultEnergyDrop SO数据");
            }

            Debug.Log("[SceneInitializer] GameManager 初始化完成");
        }
    }
}
