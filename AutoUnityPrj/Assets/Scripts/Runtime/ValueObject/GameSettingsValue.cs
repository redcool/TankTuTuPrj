using UnityEngine;

namespace Game.Runtime.ValueObject
{
    /// <summary>
    /// 游戏设置值对象 - 运行时设置数据
    /// 持久化使用 PlayerPrefs
    /// </summary>
    [System.Serializable]
    public class GameSettingsValue
    {
        // 音频
        [SerializeField] private float _masterVolume = 0.8f;
        [SerializeField] private float _musicVolume = 0.7f;
        [SerializeField] private float _sfxVolume = 1.0f;
        [SerializeField] private bool _muteAll = false;

        // 画面
        [SerializeField] private int _qualityLevel = 2;
        [SerializeField] private bool _fullscreen = true;
        [SerializeField] private int _resolutionIndex = 0;
        [SerializeField] private bool _vSync = true;

        // 游戏
        [SerializeField] private string _language = "zh-CN";
        [SerializeField] private bool _cameraShake = true;
        [SerializeField] private bool _showDamageNumbers = true;
        [SerializeField] private float _autoCollectRange = 3f;

        // 控制
        [SerializeField] private bool _invertY = false;
        [SerializeField] private float _sensitivityX = 1f;
        [SerializeField] private float _sensitivityY = 1f;

        #region Properties

        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = Mathf.Clamp01(value);
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = Mathf.Clamp01(value);
        }

        public float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Mathf.Clamp01(value);
        }

        public bool MuteAll
        {
            get => _muteAll;
            set => _muteAll = value;
        }

        public int QualityLevel
        {
            get => _qualityLevel;
            set => _qualityLevel = Mathf.Clamp(value, 0, 6);
        }

        public bool Fullscreen
        {
            get => _fullscreen;
            set => _fullscreen = value;
        }

        public int ResolutionIndex
        {
            get => _resolutionIndex;
            set => _resolutionIndex = Mathf.Max(0, value);
        }

        public bool VSync
        {
            get => _vSync;
            set => _vSync = value;
        }

        public string Language
        {
            get => _language;
            set => _language = string.IsNullOrEmpty(value) ? "zh-CN" : value;
        }

        public bool CameraShake
        {
            get => _cameraShake;
            set => _cameraShake = value;
        }

        public bool ShowDamageNumbers
        {
            get => _showDamageNumbers;
            set => _showDamageNumbers = value;
        }

        public float AutoCollectRange
        {
            get => _autoCollectRange;
            set => _autoCollectRange = Mathf.Clamp(value, 0, 10);
        }

        public bool InvertY
        {
            get => _invertY;
            set => _invertY = value;
        }

        public float SensitivityX
        {
            get => _sensitivityX;
            set => _sensitivityX = Mathf.Clamp(value, 0.1f, 5f);
        }

        public float SensitivityY
        {
            get => _sensitivityY;
            set => _sensitivityY = Mathf.Clamp(value, 0.1f, 5f);
        }

        #endregion

        #region PlayerPrefs 持久化

        private const string PP_KEY_PREFIX = "TankTuTu_Settings_";

        /// <summary>
        /// 保存设置到 PlayerPrefs
        /// </summary>
        public void SaveToPlayerPrefs()
        {
            PlayerPrefs.SetFloat(PP_KEY_PREFIX + "MasterVolume", _masterVolume);
            PlayerPrefs.SetFloat(PP_KEY_PREFIX + "MusicVolume", _musicVolume);
            PlayerPrefs.SetFloat(PP_KEY_PREFIX + "SfxVolume", _sfxVolume);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "MuteAll", _muteAll ? 1 : 0);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "QualityLevel", _qualityLevel);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "Fullscreen", _fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "ResolutionIndex", _resolutionIndex);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "VSync", _vSync ? 1 : 0);
            PlayerPrefs.SetString(PP_KEY_PREFIX + "Language", _language);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "CameraShake", _cameraShake ? 1 : 0);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "ShowDamageNumbers", _showDamageNumbers ? 1 : 0);
            PlayerPrefs.SetFloat(PP_KEY_PREFIX + "AutoCollectRange", _autoCollectRange);
            PlayerPrefs.SetInt(PP_KEY_PREFIX + "InvertY", _invertY ? 1 : 0);
            PlayerPrefs.SetFloat(PP_KEY_PREFIX + "SensitivityX", _sensitivityX);
            PlayerPrefs.SetFloat(PP_KEY_PREFIX + "SensitivityY", _sensitivityY);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 从 PlayerPrefs 加载设置
        /// </summary>
        public static GameSettingsValue LoadFromPlayerPrefs()
        {
            var settings = new GameSettingsValue();
            settings._masterVolume = PlayerPrefs.GetFloat(PP_KEY_PREFIX + "MasterVolume", 0.8f);
            settings._musicVolume = PlayerPrefs.GetFloat(PP_KEY_PREFIX + "MusicVolume", 0.7f);
            settings._sfxVolume = PlayerPrefs.GetFloat(PP_KEY_PREFIX + "SfxVolume", 1.0f);
            settings._muteAll = PlayerPrefs.GetInt(PP_KEY_PREFIX + "MuteAll", 0) == 1;
            settings._qualityLevel = PlayerPrefs.GetInt(PP_KEY_PREFIX + "QualityLevel", 2);
            settings._fullscreen = PlayerPrefs.GetInt(PP_KEY_PREFIX + "Fullscreen", 1) == 1;
            settings._resolutionIndex = PlayerPrefs.GetInt(PP_KEY_PREFIX + "ResolutionIndex", 0);
            settings._vSync = PlayerPrefs.GetInt(PP_KEY_PREFIX + "VSync", 1) == 1;
            settings._language = PlayerPrefs.GetString(PP_KEY_PREFIX + "Language", "zh-CN");
            settings._cameraShake = PlayerPrefs.GetInt(PP_KEY_PREFIX + "CameraShake", 1) == 1;
            settings._showDamageNumbers = PlayerPrefs.GetInt(PP_KEY_PREFIX + "ShowDamageNumbers", 1) == 1;
            settings._autoCollectRange = PlayerPrefs.GetFloat(PP_KEY_PREFIX + "AutoCollectRange", 3f);
            settings._invertY = PlayerPrefs.GetInt(PP_KEY_PREFIX + "InvertY", 0) == 1;
            settings._sensitivityX = PlayerPrefs.GetFloat(PP_KEY_PREFIX + "SensitivityX", 1f);
            settings._sensitivityY = PlayerPrefs.GetFloat(PP_KEY_PREFIX + "SensitivityY", 1f);
            return settings;
        }

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void ResetToDefault()
        {
            _masterVolume = 0.8f;
            _musicVolume = 0.7f;
            _sfxVolume = 1.0f;
            _muteAll = false;
            _qualityLevel = 2;
            _fullscreen = true;
            _resolutionIndex = 0;
            _vSync = true;
            _language = "zh-CN";
            _cameraShake = true;
            _showDamageNumbers = true;
            _autoCollectRange = 3f;
            _invertY = false;
            _sensitivityX = 1f;
            _sensitivityY = 1f;
        }

        #endregion
    }
}
