using UnityEngine;
using UnityEngine.UIElements;
using Game.Runtime.Controller;

namespace Game.Runtime.UI
{
    /// <summary>
    /// 战斗 HUD Presenter — 400×120 四角玩家信息面板
    /// 显示 HP、EXP、等级徽章、宝箱数量
    /// 使用独立的 BattleHud.uxml，直接绑定到场景中的 TankController
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class BattleHudPresenter : MonoBehaviour
    {
        private const int MAX_PLAYERS = 4;
        private const int CHEST_TYPES = 3;

        private VisualElement _root;

        // 四个玩家面板容器
        private VisualElement[] _playerPanels = new VisualElement[MAX_PLAYERS];

        // HP
        private Label[] _hpValues = new Label[MAX_PLAYERS];
        private VisualElement[] _hpBars = new VisualElement[MAX_PLAYERS];

        // EXP
        private Label[] _expValues = new Label[MAX_PLAYERS];
        private VisualElement[] _expBars = new VisualElement[MAX_PLAYERS];

        // 等级徽章
        private Label[] _levelCounts = new Label[MAX_PLAYERS];

        // 宝箱 (3种: bronze, silver, gold)
        private Label[][] _chestCounts = new Label[MAX_PLAYERS][];

        // 头像 / 名字
        private VisualElement[] _avatars = new VisualElement[MAX_PLAYERS];
        private Label[] _names = new Label[MAX_PLAYERS];

        private TankController[] _tanks;
        private bool _initialized;

        private void Start()
        {
            var doc = GetComponent<UIDocument>();
            if (doc == null || doc.rootVisualElement == null)
            {
                Debug.LogWarning("[BattleHudPresenter] 未找到 UIDocument");
                return;
            }
            _root = doc.rootVisualElement;

            QueryUIElements();
        }

        private void QueryUIElements()
        {
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                string p = $"p{i + 1}";

                _playerPanels[i] = _root.Q<VisualElement>($"player-{i + 1}-panel");
                _avatars[i] = _root.Q<VisualElement>($"{p}-avatar");
                _names[i] = _root.Q<Label>($"{p}-name");
                _hpValues[i] = _root.Q<Label>($"{p}-hp-value");
                _hpBars[i] = _root.Q<VisualElement>($"{p}-hp-bar");
                _expValues[i] = _root.Q<Label>($"{p}-exp-value");
                _expBars[i] = _root.Q<VisualElement>($"{p}-exp-bar");
                _levelCounts[i] = _root.Q<Label>($"{p}-level-count");

                // 宝箱 (3种)
                _chestCounts[i] = new Label[CHEST_TYPES];
                for (int j = 0; j < CHEST_TYPES; j++)
                {
                    _chestCounts[i][j] = _root.Q<Label>($"{p}-chest-{j + 1}-count");
                }
            }
        }

        private void FindTanks()
        {
            _tanks = FindObjectsOfType<TankController>();

            int tankCount = _tanks?.Length ?? 0;
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if (_playerPanels[i] == null) continue;
                bool show = i < tankCount;
                _playerPanels[i].style.display = show ? DisplayStyle.Flex : DisplayStyle.None;

                if (show && _names[i] != null)
                    _names[i].text = $"玩家 {i + 1}";
            }

            Debug.Log($"[BattleHudPresenter] 找到 {tankCount} 个 TankController");
        }

        private void Update()
        {
            if (!_initialized)
            {
                FindTanks();
                if (_tanks == null || _tanks.Length == 0)
                    return;
                _initialized = true;
            }

            int count = Mathf.Min(_tanks.Length, MAX_PLAYERS);
            for (int i = 0; i < count; i++)
            {
                if (_tanks[i] != null && _tanks[i].IsAlive)
                    UpdatePlayerPanel(i, _tanks[i]);
            }
        }

        private void UpdatePlayerPanel(int index, TankController tank)
        {
            var data = tank.TankData;
            if (data == null) return;

            // ── HP ──
            int currentHp = data.CurrentHealth;
            int maxHp = data.MaxHealth;

            if (_hpValues[index] != null)
                _hpValues[index].text = $"{currentHp} / {maxHp}";

            if (_hpBars[index] != null && maxHp > 0)
            {
                float hpRatio = Mathf.Clamp01((float)currentHp / maxHp);
                _hpBars[index].style.width = Length.Percent(hpRatio * 100f);

                _hpBars[index].RemoveFromClassList("danger");
                _hpBars[index].RemoveFromClassList("warning");
                if (hpRatio <= 0.3f)
                    _hpBars[index].AddToClassList("danger");
                else if (hpRatio <= 0.6f)
                    _hpBars[index].AddToClassList("warning");
            }

            // ── EXP ──
            if (_expValues[index] != null)
                _expValues[index].text = $"EXP 0 / 100";

            if (_expBars[index] != null)
                _expBars[index].style.width = Length.Percent(0f);

            // ── 等级徽章 ──
            if (_levelCounts[index] != null)
                _levelCounts[index].text = "1";

            // ── 宝箱（TODO: 接入实际 Chest 系统后显示真实数据） ──
            for (int j = 0; j < CHEST_TYPES; j++)
            {
                // 查找宝箱 slot 父元素
                var slot = _root.Q<VisualElement>($"p{index + 1}-chest-{j + 1}");
                if (slot != null)
                {
                    // 暂时隐藏 — 有真实数据时改为 DisplayStyle.Flex
                    slot.style.display = DisplayStyle.None;
                }
                if (_chestCounts[index]?[j] != null)
                    _chestCounts[index][j].text = "0";
            }
        }

        /// <summary>
        /// 刷新坦克引用（再生/重连后调用）
        /// </summary>
        public void RefreshTanks()
        {
            FindTanks();
        }
    }
}
