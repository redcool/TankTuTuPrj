using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 俯视跟随相机
    /// 位置跟随目标，角度固定为欧拉角（避免晕眩）
    /// 单人：跟随一个目标
    /// 多人：取所有目标中心点
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [Header("跟随目标")]
        [SerializeField] private Transform[] _targets;

        [Header("相机设置")]
        [Tooltip("相对目标中心的位置偏移量")]
        [SerializeField] private Vector3 _positionOffset = new Vector3(0f, 20f, -20f);
        [Tooltip("欧拉角（固定角度，不跟随目标旋转，避免晕眩）")]
        [SerializeField] private Vector3 _eulerAngles = new Vector3(45f, 0f, 0f);
        [Tooltip("位置跟随速度（单位/秒），0=立即跟随")]
        [SerializeField] private float _positionMoveSpeed = 40f;

        [Header("边界限制")]
        [SerializeField] private bool _useBounds = false;
        [SerializeField] private Vector2 _boundsCenter;
        [SerializeField] private Vector2 _boundsSize;

        /// <summary>
        /// 设置单个跟随目标
        /// </summary>
        public void SetTarget(Transform target)
        {
            _targets = target != null ? new[] { target } : null;
        }

        /// <summary>
        /// 设置多个跟随目标（多人）
        /// </summary>
        public void SetTargets(Transform[] targets)
        {
            _targets = targets;
        }

        /// <summary>
        /// 从 TankController 数组设置目标
        /// </summary>
        public void SetTankTargets(TankController[] tanks)
        {
            if (tanks == null || tanks.Length == 0)
            {
                _targets = null;
                return;
            }

            _targets = new Transform[tanks.Length];
            for (int i = 0; i < tanks.Length; i++)
            {
                _targets[i] = tanks[i] != null ? tanks[i].transform : null;
            }
        }

        private void LateUpdate()
        {
            if (_targets == null || _targets.Length == 0) return;
            UpdateCameraPosition();
        }

        /// <summary>
        /// 计算所有有效目标的中心点
        /// </summary>
        private Vector3 GetCentroid()
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var t in _targets)
            {
                if (t != null)
                {
                    sum += t.position;
                    count++;
                }
            }
            return count > 0 ? sum / count : transform.position;
        }

        private void UpdateCameraPosition()
        {
            // 计算目标中心点
            Vector3 center = GetCentroid();

            // 目标位置 = 中心 + 偏移量
            Vector3 desiredPos = center + _positionOffset;

            // 边界限制
            if (_useBounds)
            {
                desiredPos.x = Mathf.Clamp(desiredPos.x,
                    _boundsCenter.x - _boundsSize.x * 0.5f,
                    _boundsCenter.x + _boundsSize.x * 0.5f);
                desiredPos.z = Mathf.Clamp(desiredPos.z,
                    _boundsCenter.y - _boundsSize.y * 0.5f,
                    _boundsCenter.y + _boundsSize.y * 0.5f);
            }

            // 位置缓动（=0 时立即跟随）
            if (_positionMoveSpeed > 0f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, desiredPos, _positionMoveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = desiredPos;
            }

            // 固定欧拉角（每帧应用，Inspector 改动即时生效）
            transform.eulerAngles = _eulerAngles;
        }

        /// <summary>
        /// 立即跳转到目标位置（无平滑过渡）
        /// </summary>
        public void SnapToTargets()
        {
            if (_targets == null || _targets.Length == 0) return;

            Vector3 center = GetCentroid();
            transform.position = center + _positionOffset;
        }

        /// <summary>
        /// 保持向前兼容：旧的 SnapToTarget 方法
        /// </summary>
        public void SnapToTarget()
        {
            SnapToTargets();
        }
    }
}
