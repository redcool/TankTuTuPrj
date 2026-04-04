using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 伤害数字显示 - 漂浮在目标头顶的伤害数值
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        // 序列化字段
        [Header("设置")]
        [SerializeField] private float _floatHeight = 1.5f;
        [SerializeField] private float _floatDuration = 1f;
        [SerializeField] private float _fadeStartTime = 0.6f;

        // 私有字段
        private TextMesh _textMesh;
        private Vector3 _startPosition;
        private float _elapsedTime;
        private Color _textColor;

        /// <summary>
        /// 显示伤害数字
        /// </summary>
        public static void Show(Transform target, int damage, bool isCritical = false, bool isDodge = false)
        {
            GameObject dmgObj = new GameObject("DamageNumber");
            dmgObj.transform.position = target.position + Vector3.up * 1.5f;

            var damageNumber = dmgObj.AddComponent<DamageNumber>();
            damageNumber.Initialize(damage, isCritical, isDodge);
        }

        /// <summary>
        /// 显示治疗数字
        /// </summary>
        public static void ShowHeal(Transform target, int healAmount)
        {
            GameObject healObj = new GameObject("HealNumber");
            healObj.transform.position = target.position + Vector3.up * 1.5f;

            var textMesh = healObj.AddComponent<TextMesh>();
            textMesh.characterSize = 0.15f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.fontSize = 25;
            textMesh.color = Color.green;
            textMesh.text = "+" + healAmount;

            GameObject.Destroy(healObj, 1f);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize(int damage, bool isCritical, bool isDodge)
        {
            _textMesh = gameObject.AddComponent<TextMesh>();
            _textMesh.characterSize = isCritical ? 0.3f : 0.2f;
            _textMesh.anchor = TextAnchor.MiddleCenter;
            _textMesh.text = isDodge ? "闪避" : damage.ToString();

            if (isDodge)
            {
                _textColor = Color.gray;
            }
            else if (isCritical)
            {
                _textColor = new Color(1f, 0.6f, 0f);
            }
            else
            {
                _textColor = Color.white;
            }
            _textMesh.color = _textColor;
            _textMesh.fontSize = isCritical ? 50 : 30;

            _startPosition = transform.position;
            _elapsedTime = 0;

            Destroy(gameObject, _floatDuration);
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            float progress = _elapsedTime / _floatDuration;
            transform.position = _startPosition + Vector3.up * _floatHeight * progress;

            if (_elapsedTime > _fadeStartTime)
            {
                float fadeProgress = (_elapsedTime - _fadeStartTime) / (_floatDuration - _fadeStartTime);
                Color color = _textColor;
                color.a = Mathf.Lerp(1f, 0f, fadeProgress);
                _textMesh.color = color;
            }
        }
    }
}
