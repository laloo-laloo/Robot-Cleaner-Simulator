using UnityEngine;

public class MiniMapIcon : MonoBehaviour
{
    [SerializeField] private Transform _target; // 월드에 있는 실제 오브젝트
    [SerializeField] private RectTransform _iconRect;
    [SerializeField] private float _scale = 1f; // 월드 좌표 → 미니맵 좌표 축소 비율

    void LateUpdate()
    {
        Vector2 pos = new Vector2(_target.position.x, _target.position.z) * _scale;
        _iconRect.anchoredPosition = pos;
    }
}

