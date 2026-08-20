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

        if (state)
        {
            OpenSubPage(0);
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
}
