using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _illustrationImage;
    [SerializeField] private RectTransform _illustrationRect;
    [SerializeField] private TMP_Text _storyText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Cutscene Pages")]
    [SerializeField] private CutscenePage[] _pages;
    [SerializeField] private float _flipDuration = 0.6f; // 책장 넘어가는 시간

    [Header("Text Typing Settings")]
    [SerializeField] private float _typingSpeed = 0.05f; // 글자 출력 속도 (작을수록 빠름)

    private int _currentPageIndex = 0;
    private bool _isTyping = false;
    private bool _skipTyping = false;

    private void Start()
    {
        if (_pages.Length > 0)
        {
            StartCoroutine(PlayIntroSequence());
        }
    }

    private IEnumerator PlayIntroSequence()
    {
        _canvasGroup.alpha = 1f;

        while (_currentPageIndex < _pages.Length)
        {
            CutscenePage currentPage = _pages[_currentPageIndex];
            _illustrationImage.sprite = currentPage.illustrationSprite;

            // 1. 책장 들어오는 연출
            yield return StartCoroutine(AnimatePageIn(currentPage.flipDirection));

            // 2. 텍스트 한 글자씩 출력 연출
            yield return StartCoroutine(TypeText(currentPage.storyText));

            // 3. 유저 입력 대기 (클릭/스페이스바/터치)
            yield return new WaitUntil(IsPressedThisFrame);

            // 4. 책장 넘어가는 연출
            yield return StartCoroutine(AnimatePageOut(currentPage.flipDirection));

            _currentPageIndex++;
            yield return null;
        }

        // 모든 페이지 완료 후 Fade Out & 씬 전환
        yield return StartCoroutine(FadeOutCutscene());
       
        SceneManager.LoadScene("Title");
        
    }

    /// <summary>
    /// 텍스트가 따다닥 한 글자씩 출력되는 코루틴
    /// </summary>
    private IEnumerator TypeText(string targetText)
    {
        _storyText.text = "";
        _isTyping = true;
        _skipTyping = false;

        foreach (char letter in targetText.ToCharArray())
        {
            // 타이핑 중에 클릭하면 텍스트 전체를 한 번에 보여주고 스킵
            if (_skipTyping)
            {
                _storyText.text = targetText;
                break;
            }

            _storyText.text += letter;

            // 타이핑 도중 입력을 감지하기 위한 대기
            float timer = 0f;
            while (timer < _typingSpeed)
            {
                timer += Time.deltaTime;
                if (IsPressedThisFrame())
                {
                    _skipTyping = true;
                    break;
                }
                yield return null;
            }
        }

        _isTyping = false;
        yield return new WaitForSeconds(0.1f); // 스킵 클릭과 다음 전환 클릭이 겹치지 않도록 아주 잠시 대기
    }

    /// <summary>
    /// New Input System 입력 체크 (마우스/키보드/터치)
    /// </summary>
    private bool IsPressedThisFrame()
    {
        return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
               (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
               (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
    }

    private IEnumerator AnimatePageIn(CutscenePage.FlipDirection dir)
    {
        float elapsed = 0f;
        float startAngle = (dir == CutscenePage.FlipDirection.RightToLeft) ? 45f : -45f;

        _illustrationRect.localRotation = Quaternion.Euler(0, startAngle, startAngle * 0.2f);
        _illustrationRect.localScale = Vector3.one * 0.8f;

        while (elapsed < _flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _flipDuration);

            _illustrationRect.localRotation = Quaternion.Euler(0, Mathf.Lerp(startAngle, 0f, t), Mathf.Lerp(startAngle * 0.2f, 0f, t));
            _illustrationRect.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

            yield return null;
        }

        _illustrationRect.localRotation = Quaternion.identity;
        _illustrationRect.localScale = Vector3.one;
    }

    private IEnumerator AnimatePageOut(CutscenePage.FlipDirection dir)
    {
        float elapsed = 0f;
        float targetAngle = (dir == CutscenePage.FlipDirection.RightToLeft) ? -80f : 80f;

        Vector3 startPos = _illustrationRect.anchoredPosition;
        Vector3 targetPos = startPos + new Vector3((dir == CutscenePage.FlipDirection.RightToLeft) ? -200f : 200f, -50f, 0f);

        while (elapsed < _flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _flipDuration;
            t = t * t;

            _illustrationRect.localRotation = Quaternion.Euler(0, Mathf.Lerp(0f, targetAngle, t), Mathf.Lerp(0f, -targetAngle * 0.15f, t));
            _illustrationRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);
            _illustrationRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.7f, t);

            yield return null;
        }

        _illustrationRect.anchoredPosition = startPos;
    }

    private IEnumerator FadeOutCutscene()
    {
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.5f);
            yield return null;
        }
    }
}