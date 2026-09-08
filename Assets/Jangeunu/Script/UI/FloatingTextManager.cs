using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject _floatingTextPrefab; // TextMeshPro가 들어있는 UIFloatingText 프리팹
    [SerializeField] private RectTransform _spawnTransform;   // 왼쪽 상단 구역 UI 근처에 배치한 빈 RectTransform

    [Header("Spawn Settings")]
    [SerializeField] private float _randomOffsetRange = 15f;  // 텍스트가 겹치지 않도록 줄 랜덤 범위(픽셀)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 외부(ZoneManager 또는 TrashObject)에서 호출하는 생성 함수
    public void ShowText(string message, Color textColor)
    {
        if (_floatingTextPrefab == null || _spawnTransform == null) return;

        // 1. 프리팹 생성 후 스폰 지점 자식으로 배치
        GameObject textObj = Instantiate(_floatingTextPrefab, _spawnTransform);

        // 2. 약간의 위치 변형(오프셋)을 주어 연속으로 떠올라도 겹치지 않게 처리
        RectTransform rect = textObj.GetComponent<RectTransform>();
        Vector2 randomOffset = new Vector2(
            Random.Range(-_randomOffsetRange, _randomOffsetRange),
            Random.Range(-_randomOffsetRange, _randomOffsetRange)
        );
        rect.anchoredPosition = randomOffset;

        // 3. UI 텍스트 데이터 세팅
        if (textObj.TryGetComponent<UIFloatingText>(out var floatingText))
        {
            floatingText.Setup(message, textColor);
        }
    }
}
