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

            // 初始化玩家战车（激活场景中 inactive 的 PlayerTank）
            InitializePlayerTank();

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

            var tanks = FindObjectsOfType<TankController>();
            if (tanks.Length > 0)
            {
                levelManager.PlayerTanks = tanks;
            }

            // 旧版 uGUI HUDView/ResultView 已删除
            // UI Toolkit HUD/Result 由 UIFlowManager 管理，后续对接

            Debug.Log("[SceneInitializer] LevelManager 初始化完成");
        }

        /// <summary>
        /// 初始化GameManager
        /// 当直接 Play Level_0 且没有 GameManager 时（Editor 模式），自动创建测试用 GameManager
        /// </summary>
        private void InitializeGameManager()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
#if UNITY_EDITOR
                gm = CreateTestGameManager();
#else
                Debug.LogWarning("[SceneInitializer] 未找到GameManager");
                return;
#endif
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

        /// <summary>
        /// 初始化玩家战车 - 激活场景中 inactive 的 PlayerTank
        /// 激活后 TankController.Awake() 自动运行，加载角色模型和武器槽
        /// </summary>
        private void InitializePlayerTank()
        {
            // 查找场景中所有 TankController（包含 inactive）
            var allTanks = FindObjectsOfType<TankController>(true);
            TankController playerTank = null;

            foreach (var tank in allTanks)
            {
                // PlayerTank 实例在场景中且当前 inactive（未激活）
                if (tank.gameObject.scene == gameObject.scene && !tank.gameObject.activeInHierarchy)
                {
                    playerTank = tank;
                    break;
                }
            }

            if (playerTank == null)
            {
                Debug.LogWarning("[SceneInitializer] 未找到 inactive 的 PlayerTank，尝试使用 active 的 TankController");
                foreach (var tank in allTanks)
                {
                    if (tank.gameObject.scene == gameObject.scene)
                    {
                        playerTank = tank;
                        break;
                    }
                }
            }

            if (playerTank == null)
            {
                Debug.LogError("[SceneInitializer] 场景中无 TankController，无法生成玩家战车");
                return;
            }

            // 激活战车 → 触发 TankController.Awake()
            playerTank.gameObject.SetActive(true);
            Debug.Log($"[SceneInitializer] PlayerTank 已激活: {playerTank.name}");

            // 连接到 LevelManager
            var levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.PlayerTanks = new TankController[] { playerTank };
            }

            // 设置摄影机跟随目标
            SetupCamera(playerTank);
        }

        /// <summary>
        /// 设置摄影机跟随目标
        /// 查找场景中的 FollowCamera 并传入所有激活的玩家战车
        /// </summary>
        private void SetupCamera(TankController primaryTank)
        {
            var cam = FindObjectOfType<FollowCamera>();
            if (cam == null)
            {
                Debug.LogWarning("[SceneInitializer] 未找到 FollowCamera");
                return;
            }

            // 收集场景中所有激活的玩家战车（支持多人）
            var allTanks = FindObjectsOfType<TankController>(false);
            if (allTanks.Length > 0)
            {
                cam.SetTankTargets(allTanks);
            }
            else
            {
                cam.SetTarget(primaryTank.transform);
            }

            Debug.Log($"[SceneInitializer] 摄影机已设置为跟随 {allTanks.Length} 辆战车");
        }

#if UNITY_EDITOR
        /// <summary>
        /// 创建测试用 GameManager（Level_0 直接 Play 时自动调用）
        /// 填充默认的角色/武器/难度数据，使战斗流程无需从 GameStart 场景启动
        /// </summary>
        private GameManager CreateTestGameManager()
        {
            var go = new GameObject("GameManager [Test]");
            var gm = go.AddComponent<GameManager>();

            // 标记为 DontDestroyOnLoad 以模拟 GameStart 场景行为
            UnityEngine.Object.DontDestroyOnLoad(go);

            // 加载资源预制体（和正常流程一致）
            var energyDropPrefab = Resources.Load<GameObject>("Prefabs/Items/Block/goods1");
            var treasureBoxPrefab = Resources.Load<GameObject>("Prefabs/Items/Box/TreasureBox1");
            var energyDropData = Resources.Load<Game.Runtime.ValueObject.ScriptableObjects.EnergyDropDataSO>(
                "ScriptableObjects/EnergyDrop/DefaultEnergyDrop");
            var defaultCharacter = Resources.Load<Game.Runtime.ValueObject.ScriptableObjects.CharacterDataSO>(
                "ScriptableObjects/Characters/Character_LightTank");
            var defaultWeapon = Resources.Load<Game.Runtime.ValueObject.ScriptableObjects.WeaponDataSO>(
                "ScriptableObjects/Weapons/Weapon_MachineGun_LightMG");
            var defaultDifficulty = Resources.Load<Game.Runtime.ValueObject.ScriptableObjects.DifficultyDataSO>(
                "ScriptableObjects/Difficulties/Difficulty_Easy");

            if (energyDropPrefab != null) gm.EnergyDropPrefab = energyDropPrefab;
            if (treasureBoxPrefab != null) gm.TreasureBoxPrefab = treasureBoxPrefab;
            if (energyDropData != null) gm.EnergyDropData = energyDropData;

            // 填充角色数据（TankController 会从 GameManager 读取）
            if (defaultCharacter != null)
            {
                gm.SelectedCharacterData = defaultCharacter;
                gm.SelectedCharacterId = defaultCharacter.name.Replace("Character_", "").ToLower();
            }
            else
            {
                gm.SelectedCharacterId = "mbt";
            }

            // 填充武器数据
            if (defaultWeapon != null)
            {
                gm.SelectedWeaponDatas = new System.Collections.Generic.List<Game.Runtime.ValueObject.ScriptableObjects.WeaponDataSO>
                    { defaultWeapon };
                gm.SelectedWeaponIdList = new System.Collections.Generic.List<string>
                    { defaultWeapon.name };
            }

            // 填充难度数据
            if (defaultDifficulty != null)
            {
                gm.SelectedDifficultyData = defaultDifficulty;
                gm.SelectedDifficultyLevel = defaultDifficulty.StarRating;
            }

            Debug.Log("[SceneInitializer] 已创建测试用 GameManager（Editor 模式），无需从 GameStart 场景启动");
            return gm;
        }
#endif
    }
}
