using UnityEngine;
using TMPro;

public class UIFloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _moveSpeed = 30f; // UI 픽셀 단위 위로 이동 속도
    [SerializeField] private float _fadeSpeed = 1.5f; // 알파값 감소 속도

    private RectTransform _rectTransform;
    private Color _textColor;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // FloatingTextManager에서 생성 후 초기 텍스트 및 색상 설정
    public void Setup(string message, Color color)
    {
        if (_text == null) return;

        _text.text = message;
        _textColor = color;
        _text.color = _textColor;
    }

    private void Update()
    {
        // 1. Y축 방향(위쪽)으로 이동
        _rectTransform.anchoredPosition += Vector2.up * _moveSpeed * Time.deltaTime;

        // 2. 시간이 지나면서 Alpha(투명도) 감소
        _textColor.a -= _fadeSpeed * Time.deltaTime;
        _text.color = _textColor;

        // 3. 완전히 투명해지면 삭제 (추후 풀링 시 SetActive(false)로 변경 가능)
        if (_textColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
