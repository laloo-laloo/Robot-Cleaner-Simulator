using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCleanManager : MonoBehaviour
{
    public static PlayerCleanManager instance;
    private bool _isInDanger;

    private Coroutine _blinkCoroutine;

    public enum CleaningMode
    {
        Sweeping, Wiping
    }
    public CleaningMode Mode;

    private Player _player;

    [SerializeField] private Image _warning, _SweepingImage, _WipingImage;


    private void Awake()
    {
        _player = GetComponent<Player>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _warning.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckReturnCondition();

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("청소모드 전환");
            ToggleCleaningMode();
        }
    }

    private void ToggleCleaningMode()
    {
        if (Mode == CleaningMode.Sweeping)
        {
            _WipingImage.gameObject.SetActive(true);
            _SweepingImage.gameObject.SetActive(false);
            Mode = CleaningMode.Wiping;
        }
        else
        {
            _WipingImage.gameObject.SetActive(false);
            _SweepingImage.gameObject.SetActive(true);
            Mode = CleaningMode.Sweeping;
        }
    }

    private void CheckReturnCondition()
    {
        if (_player.CheckBatteryValue() <= 15f || _player.CheckDustValue() >= 85f)
        {
            if (!_isInDanger)
            {
                _isInDanger = true;
                _blinkCoroutine = StartCoroutine(BlinkingWarning());
            }
        }
        else
        {
            _isInDanger = false;
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
            }
            _warning.gameObject.SetActive(false);
        }
    }

    private IEnumerator BlinkingWarning()
    {
        while (_isInDanger)
        {
            _warning.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _warning.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
