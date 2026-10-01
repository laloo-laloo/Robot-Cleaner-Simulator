using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _notebookPanel;
    [SerializeField] private GameObject[] _subPages;

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleNotebook();
        }
    }

    private void ToggleNotebook()
    {
        bool state = !_notebookPanel.activeSelf;
        _notebookPanel.SetActive(state);

        // [추가] 메뉴 열림 여부에 따른 마우스 커서 제어
        UpdateCursorState(state);

        if (state)
        {
            OpenSubPage(0); // 기본 페이지(0번) 열기
        }
    }

    public void OpenSubPage(int pageIndex)
    {
        for (int i = 0; i < _subPages.Length; i++)
        {
            if (_subPages[i] != null)
            {
                _subPages[i].SetActive(i == pageIndex);
            }
        }

        if (pageIndex == 0 && ZoneManager.Instance != null)
        {
            ZoneManager.Instance.UpdateAllUI();
        }
    }

    // [추가] 버튼 클릭 이벤트 전용 메서드 (버튼 OnClick에 연결)
    public void OnClickPageButton(int pageIndex)
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        OpenSubPage(pageIndex);
    }

    // [추가] 마우스 커서 잠금/해제 및 표시 제어
    private void UpdateCursorState(bool isMenuOpen)
    {
        if (isMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None; // 커서 고정 해제
            Cursor.visible = true;                  // 커서 보이게 처리
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // 화면 중앙 고정 (프로젝트 설정에 따라 None/Confined 선택 가능)
            Cursor.visible = false;                   // 커서 숨김
        }
    }
}