using UnityEngine;

public class MiniMapIcon : MonoBehaviour
{
    [SerializeField] private Transform _target; // 월드에 있는 실제 오브젝트
    [SerializeField] private RectTransform _iconRect;
    [SerializeField] private float _scale = 1f; // 월드 좌표 → 미니맵 좌표 축소 비율

    private const float WorldWidth = 55.97f;
    private const float WorldHeight = 52.6f;
    private const float MapPixelWidth = 350f;   // 미니맵 이미지 실제 픽셀 폭
    private const float MapPixelHeight = 350f;  // 미니맵 이미지 실제 픽셀 높이
    private const float CenterX = 4.125f;
    private const float CenterZ = -1.3f;

    void LateUpdate()
    {
        float scaleX = MapPixelWidth / WorldWidth;
        float scaleY = MapPixelHeight / WorldHeight;

        float mapX = (_target.position.x - CenterX) * scaleX;
        float mapY = (_target.position.z - CenterZ) * scaleY;

        _iconRect.anchoredPosition = new Vector2(mapX, mapY);
    }
}

