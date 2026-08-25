using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZoneProgressUIItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _zoneNameText;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Slider _progressSlider;

    // 데이터를 받아서 내 UI 컴포넌트들을 갱신하는 함수
    public void SetZoneData(string zoneName, float progressPercent)
    {
        if (_zoneNameText != null) _zoneNameText.text = zoneName;
        if (_progressText != null) _progressText.text = $"{progressPercent:F1}%";
        if (_progressSlider != null) _progressSlider.value = progressPercent / 100f; // 0~1 비율
    }
}
