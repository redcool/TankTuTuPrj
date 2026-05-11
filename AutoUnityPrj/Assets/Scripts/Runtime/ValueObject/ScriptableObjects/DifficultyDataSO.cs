using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 难度数据 ScriptableObject - 定义游戏难度等级的属性
    /// </summary>
    [CreateAssetMenu(fileName = "NewDifficulty", menuName = "铁皮突突/难度数据")]
    public class DifficultyDataSO : ScriptableObject
    {
        [Header("基础信息")]
        [SerializeField] private string _difficultyName = "";
        [SerializeField] private Sprite _icon;
        [TextArea(2, 3)]
        [SerializeField] private string _description = "";

        [Header("难度属性")]
        [Tooltip("难度等级 (0-6)")]
        [SerializeField, Range(0, 6)] private int _difficultyLevel = 0;
        
        [Tooltip("敌人数量倍率")]
        [SerializeField] private float _enemyCountMultiplier = 1f;
        
        [Tooltip("敌人生命倍率")]
        [SerializeField] private float _enemyHpMultiplier = 1f;
        
        [Tooltip("敌人移速倍率")]
        [SerializeField] private float _enemySpeedMultiplier = 1f;
        
        [Tooltip("敌人攻击倍率")]
        [SerializeField] private float _enemyDamageMultiplier = 1f;
        
        [Tooltip("刷怪间隔百分比 (越小越快)")]
        [SerializeField, Range(0.1f, 2f)] private float _spawnIntervalMultiplier = 1f;
        
        [Tooltip("道具掉落率")]
        [SerializeField, Range(0f, 2f)] private float _dropRateMultiplier = 1f;
        
        [Tooltip("经验倍率")]
        [SerializeField, Range(0.5f, 3f)] private float _expMultiplier = 1f;

        [Header("特殊效果")]
        [Tooltip("是否显示敌人血条")]
        [SerializeField] private bool _showEnemyHealthBar = true;
        
        [Tooltip("是否有精英敌人")]
        [SerializeField] private bool _hasEliteEnemies = true;
        
        [Tooltip("是否有Boss")]
        [SerializeField] private bool _hasBoss = false;

        #region Properties

        public string DifficultyName
        {
            get => _difficultyName;
            set => _difficultyName = value;
        }

        public Sprite Icon
        {
            get => _icon;
            set => _icon = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int DifficultyLevel
        {
            get => _difficultyLevel;
            set => _difficultyLevel = Mathf.Clamp(value, 0, 6);
        }

        public float EnemyCountMultiplier => _enemyCountMultiplier;
        public float EnemyHpMultiplier => _enemyHpMultiplier;
        public float EnemySpeedMultiplier => _enemySpeedMultiplier;
        public float EnemyDamageMultiplier => _enemyDamageMultiplier;
        public float SpawnIntervalMultiplier => _spawnIntervalMultiplier;
        public float DropRateMultiplier => _dropRateMultiplier;
        public float ExpMultiplier => _expMultiplier;
        public bool ShowEnemyHealthBar => _showEnemyHealthBar;
        public bool HasEliteEnemies => _hasEliteEnemies;
        public bool HasBoss => _hasBoss;

        #endregion

        /// <summary>
        /// 获取完整的属性描述文本（中文显示）
        /// </summary>
        public string GetStatsDescription()
        {
            var sb = new System.Text.StringBuilder();
            
            sb.AppendLine($"等级: {_difficultyLevel}");
            sb.AppendLine($"敌人: x{_enemyCountMultiplier:F1}");
            sb.AppendLine($"生命: x{_enemyHpMultiplier:F1}");
            sb.AppendLine($"移速: x{_enemySpeedMultiplier:F1}");
            sb.AppendLine($"攻击: x{_enemyDamageMultiplier:F1}");
            sb.AppendLine($"刷怪: x{_spawnIntervalMultiplier:F1}");
            sb.AppendLine($"掉落: x{_dropRateMultiplier:F1}");
            sb.AppendLine($"经验: x{_expMultiplier:F1}");
            
            if (!string.IsNullOrEmpty(_description))
            {
                sb.AppendLine();
                sb.AppendLine(_description);
            }

            return sb.ToString();
        }
    }
}