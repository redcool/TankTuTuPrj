using UnityEngine;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 45度俯视跟随相机
    /// 作者：AI Agent
    /// 最后修改时间：2026-04-09
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [Header("跟随目标")]
        [SerializeField] private Transform _target;

        [Header("相机设置")]
        [SerializeField] private float _distance = 10f;
        [SerializeField] private float _angle = 45f;
        [SerializeField] private float _smoothSpeed = 5f;

        [Header("边界限制")]
        [SerializeField] private bool _useBounds = false;
        [SerializeField] private Vector2 _boundsCenter;
        [SerializeField] private Vector2 _boundsSize;

        private Vector3 _velocity;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            float rad = _angle * Mathf.Deg2Rad;
            float yOffset = Mathf.Sin(rad) * _distance;
            float zOffset = -Mathf.Cos(rad) * _distance;
            Vector3 offset = new Vector3(0f, yOffset, zOffset);
            Vector3 desiredPos = _target.position + offset;

            if (_useBounds)
            {
                desiredPos.x = Mathf.Clamp(desiredPos.x, _boundsCenter.x - _boundsSize.x * 0.5f, _boundsCenter.x + _boundsSize.x * 0.5f);
                desiredPos.z = Mathf.Clamp(desiredPos.z, _boundsCenter.y - _boundsSize.y * 0.5f, _boundsCenter.y + _boundsSize.y * 0.5f);
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _velocity, 1f / _smoothSpeed);
            transform.LookAt(_target);
        }

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
        }

        public void SnapToTarget()
        {
            if (_target == null) return;
            float rad = _angle * Mathf.Deg2Rad;
            float yOffset = Mathf.Sin(rad) * _distance;
            float zOffset = -Mathf.Cos(rad) * _distance;
            transform.position = _target.position + new Vector3(0f, yOffset, zOffset);
            transform.LookAt(_target);
        }
    }
}