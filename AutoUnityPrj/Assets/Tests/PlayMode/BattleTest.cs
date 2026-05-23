using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Game.Runtime.Controller;
using Game.Runtime.UI;

/// <summary>
/// 战斗场景 PlayMode 测试
/// 直接加载 Level_0 场景，验证战车初始化、HUD 显示、战斗循环
/// </summary>
public class BattleTest
{
    private const string LEVEL_0_SCENE = "Level_0";

    /// <summary>
    /// 加载 Level_0 场景并等待初始化完成
    /// </summary>
    [UnityTest]
    public IEnumerator SceneInitializes_Correctly()
    {
        // 加载场景
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null; // 等待一帧让 Awake 执行
        yield return null; // 再等一帧让 Start 执行

        // 验证场景关键对象存在
        var sceneInitializer = Object.FindObjectOfType<SceneInitializer>();
        Assert.IsNotNull(sceneInitializer, "SceneInitializer 应在场景中");

        var gameManager = GameManager.Instance;
        Assert.IsNotNull(gameManager, "GameManager 应存在（Editor 模式下自动创建）");

        var tank = Object.FindObjectOfType<TankController>();
        Assert.IsNotNull(tank, "TankController 应在场景中");
    }

    /// <summary>
    /// 验证 TankController 能正确初始化默认数据
    /// </summary>
    [UnityTest]
    public IEnumerator TankController_Initializes_WithDefaultData()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null;
        yield return null;

        var tank = Object.FindObjectOfType<TankController>();
        Assert.IsNotNull(tank, "TankController 应在场景中");

        // 战车数据应已加载（有有效血量）
        Assert.IsTrue(tank.TankData != null, "TankData 不应为 null");
        Assert.Greater(tank.TankData.MaxHealth, 0, "最大血量应大于 0");
        Assert.Greater(tank.TankData.CurrentHealth, 0, "当前血量应大于 0");
        Assert.Greater(tank.TankData.MoveSpeed, 0, "移动速度应大于 0");
        Assert.IsTrue(tank.IsAlive, "战车应处于存活状态");
    }

    /// <summary>
    /// 验证 LevelManager 在场景初始化后启动
    /// </summary>
    [UnityTest]
    public IEnumerator LevelManager_Starts_Level()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null;
        yield return null;

        var levelManager = Object.FindObjectOfType<LevelManager>();
        Assert.IsNotNull(levelManager, "LevelManager 应在场景中");

        // 关卡应已激活
        Assert.IsTrue(levelManager.IsLevelActive, "关卡应在初始化后激活");

        // 倒计时应正在运行（剩余时间 < 总时长）
        Assert.Less(levelManager.RemainingTime, LevelManager.LEVEL_DURATION,
            "倒计时应已开始减少");
    }

    /// <summary>
    /// 验证 BattleHudPresenter 正确挂载并连接到 UIDocument
    /// </summary>
    [UnityTest]
    public IEnumerator BattleHudPresenter_Finds_UIDocument()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null;
        yield return null;

        // BattleHudPresenter 应在 UIDocument 所在物体上
        var doc = Object.FindObjectOfType<UIDocument>();
        Assert.IsNotNull(doc, "UIDocument 应在场景中");

        var presenter = doc.GetComponent<BattleHudPresenter>();
        Assert.IsNotNull(presenter, "BattleHudPresenter 应挂载在 UIDocument 同一物体上");

        // 验证 UXML 根节点可用
        Assert.IsNotNull(doc.rootVisualElement, "UIDocument rootVisualElement 不应为 null");

        // 验证 HUD 面板已加载
        var panel = doc.rootVisualElement.Q<VisualElement>("panel-battle-hud");
        Assert.IsNotNull(panel, "panel-battle-hud 应在 UXML 中");
        Assert.AreEqual(DisplayStyle.Flex, panel.style.display.value,
            "panel-battle-hud 应可见（BattleHud.uxml 中的默认状态）");
    }

    /// <summary>
    /// 验证 HUD 面板正确显示战车数据（HP、EXP 等元素存在）
    /// </summary>
    [UnityTest]
    public IEnumerator BattleHud_Displays_PlayerPanels()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null; // Awake
        yield return null; // Start
        yield return null; // Update (BattleHudPresenter 延迟初始化)
        yield return null; // 额外帧确保安全

        var doc = Object.FindObjectOfType<UIDocument>();
        Assert.IsNotNull(doc);

        var root = doc.rootVisualElement;

        // 验证四个玩家面板都已加载
        for (int i = 1; i <= 4; i++)
        {
            var panel = root.Q<VisualElement>($"player-{i}-panel");
            Assert.IsNotNull(panel, $"player-{i}-panel 应在 UXML 中");

            var hpLabel = root.Q<Label>($"p{i}-hp-value");
            Assert.IsNotNull(hpLabel, $"p{i}-hp-value 应在 UXML 中");

            var hpBar = root.Q<VisualElement>($"p{i}-hp-bar");
            Assert.IsNotNull(hpBar, $"p{i}-hp-bar 应在 UXML 中");
        }

        // 至少玩家 1 的面板应可见（场景中有 1 个 TankController）
        var player1Panel = root.Q<VisualElement>("player-1-panel");
        Assert.AreEqual(DisplayStyle.Flex, player1Panel.style.display.value,
            "player-1-panel 应可见");
    }

    /// <summary>
    /// 验证 EnemySpawner 存在并能开始生成敌人
    /// </summary>
    [UnityTest]
    public IEnumerator EnemySpawner_Exists_AndSpawnsEnemies()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null;
        yield return null;

        var spawner = Object.FindObjectOfType<EnemySpawner>();
        Assert.IsNotNull(spawner, "EnemySpawner 应在场景中");

        // 等待几帧让生成器开始工作
        yield return new WaitForSeconds(2.0f);

        // 场景中应有敌人被生成（Enemy 标签的对象）
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Assert.Greater(enemies.Length, 0, "应已生成至少一个敌人");
    }

    /// <summary>
    /// 验证坦克受到伤害后血量正确减少
    /// </summary>
    [UnityTest]
    public IEnumerator TankController_TakesDamage()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null;
        yield return null;

        var tank = Object.FindObjectOfType<TankController>();
        Assert.IsNotNull(tank);
        Assert.IsTrue(tank.IsAlive, "战车初始应存活");

        int initialHp = tank.TankData.CurrentHealth;

        // 手动造成伤害
        tank.TakeDamage(20);

        // 血量应减少
        Assert.AreEqual(initialHp - 20, tank.TankData.CurrentHealth, "伤害后血量应减少 20");

        // 战车应仍存活
        Assert.IsTrue(tank.IsAlive, "受到 20 点伤害后战车应仍存活");
    }

    /// <summary>
    /// 验证坦克死亡逻辑
    /// </summary>
    [UnityTest]
    public IEnumerator TankController_Dies_WhenHpReachesZero()
    {
        SceneManager.LoadScene(LEVEL_0_SCENE);
        yield return null;
        yield return null;

        var tank = Object.FindObjectOfType<TankController>();
        Assert.IsNotNull(tank);

        // 施加致命伤害
        int maxHp = tank.TankData.MaxHealth;
        tank.TakeDamage(maxHp + 50);

        // 血量应为 0（被 clamp）
        Assert.AreEqual(0, tank.TankData.CurrentHealth, "死亡后血量应为 0");
        Assert.IsFalse(tank.IsAlive, "战车应已死亡");
    }
}
