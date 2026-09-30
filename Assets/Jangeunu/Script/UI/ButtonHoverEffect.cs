using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Target Child Image")]
    [SerializeField] private Image _childImage; // 변경할 자식 Image 컴포넌트

    [Header("Sprites")]
    [SerializeField] private Sprite _normalSprite; // 평소 이미지
    [SerializeField] private Sprite _hoverSprite;  // 마우스 올렸을 때 이미지

    private void Start()
    {
        if (_childImage != null && _normalSprite != null)
        {
            _childImage.sprite = _normalSprite;
        }
    }

    // 마우스가 버튼 영역에 들어왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_childImage != null && _hoverSprite != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.SFX.ButtonSelect);
            _childImage.sprite = _hoverSprite;
        }
    }

    // 마우스가 버튼 영역에서 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_childImage != null && _normalSprite != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.SFX.ButtonSelect);
            _childImage.sprite = _normalSprite;
        }
    }
}
