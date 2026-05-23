using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 游戏设置 ScriptableObject - 可在 Inspector 中配置默认值
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "铁皮突突/游戏设置")]
    public class GameSettingsSO : ScriptableObject
    {
        [Header("音频")]
        [SerializeField] [Range(0, 1)] private float _masterVolume = 0.8f;
        [SerializeField] [Range(0, 1)] private float _musicVolume = 0.7f;
        [SerializeField] [Range(0, 1)] private float _sfxVolume = 1.0f;
        [SerializeField] private bool _muteAll = false;

        [Header("画面")]
        [SerializeField] private int _qualityLevel = 2;
        [SerializeField] private bool _fullscreen = true;
        [SerializeField] private int _resolutionIndex = 0;
        [SerializeField] private bool _vSync = true;

        [Header("游戏")]
        [SerializeField] private string _language = "zh-CN";
        [SerializeField] private bool _cameraShake = true;
        [SerializeField] private bool _showDamageNumbers = true;
        [SerializeField] [Range(0, 10)] private float _autoCollectRange = 3f;

        [Header("控制")]
        [SerializeField] private bool _invertY = false;
        [SerializeField] [Range(0.1f, 5f)] private float _sensitivityX = 1f;
        [SerializeField] [Range(0.1f, 5f)] private float _sensitivityY = 1f;

        #region Properties

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public bool MuteAll => _muteAll;
        public int QualityLevel => _qualityLevel;
        public bool Fullscreen => _fullscreen;
        public int ResolutionIndex => _resolutionIndex;
        public bool VSync => _vSync;
        public string Language => _language;
        public bool CameraShake => _cameraShake;
        public bool ShowDamageNumbers => _showDamageNumbers;
        public float AutoCollectRange => _autoCollectRange;
        public bool InvertY => _invertY;
        public float SensitivityX => _sensitivityX;
        public float SensitivityY => _sensitivityY;

        #endregion

        /// <summary>
        /// 转换为 GameSettingsValue
        /// </summary>
        public GameSettingsValue ToDataValue()
        {
            return new GameSettingsValue
            {
                MasterVolume = _masterVolume,
                MusicVolume = _musicVolume,
                SfxVolume = _sfxVolume,
                MuteAll = _muteAll,
                QualityLevel = _qualityLevel,
                Fullscreen = _fullscreen,
                ResolutionIndex = _resolutionIndex,
                VSync = _vSync,
                Language = _language,
                CameraShake = _cameraShake,
                ShowDamageNumbers = _showDamageNumbers,
                AutoCollectRange = _autoCollectRange,
                InvertY = _invertY,
                SensitivityX = _sensitivityX,
                SensitivityY = _sensitivityY
            };
        }
    }
}
