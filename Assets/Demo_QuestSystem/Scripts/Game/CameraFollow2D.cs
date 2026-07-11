using UnityEngine;

/// <summary>
/// 2D 相机跟随
/// 
/// 职责：
/// 1. 平滑跟随玩家
/// 2. 限制在地图范围内
/// 
/// 挂载在 Main Camera 上。
/// </summary>
public class CameraFollow2D : MonoBehaviour
{
    [Header("── 跟随配置 ──")]
    [Tooltip("跟随目标")]
    [SerializeField] private Transform _target;
    [Tooltip("平滑跟随速度")]
    [SerializeField] private float _smoothSpeed = 8f;
    [Tooltip("相机偏移")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -10);

    [Header("── 地图边界 ──")]
    [Tooltip("是否限制在地图范围内")]
    [SerializeField] private bool _clampToMap = true;
    [Tooltip("地图最小坐标")]
    [SerializeField] private Vector2 _mapMin = new Vector2(-15, -10);
    [Tooltip("地图最大坐标")]
    [SerializeField] private Vector2 _mapMax = new Vector2(15, 10);

    private void LateUpdate()
    {
        if (_target == null)
        {
            // 自动查找 Player
            var player = PlayerController2D.Instance;
            if (player != null) _target = player.transform;
            else return;
        }

        Vector3 desired = _target.position + _offset;

        if (_clampToMap)
        {
            float camHeight = Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            desired.x = Mathf.Clamp(desired.x, _mapMin.x + camWidth, _mapMax.x - camWidth);
            desired.y = Mathf.Clamp(desired.y, _mapMin.y + camHeight, _mapMax.y - camHeight);
        }

        transform.position = Vector3.Lerp(transform.position, desired, _smoothSpeed * Time.deltaTime);
    }
}
