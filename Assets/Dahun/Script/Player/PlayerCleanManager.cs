using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCleanManager : MonoBehaviour
{
    public static PlayerCleanManager instance;

    public enum CleaningMode
    {
        Sweeping, Wiping
    }
    public CleaningMode Mode;

    private Player _player;

    [SerializeField] private Image _warning;


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

        // if (먼지 흡입시)
            //AddDustProgress();
    }

    private void ToggleCleaningMode()
    {
        if (Mode == CleaningMode.Sweeping)
        {
            Mode = CleaningMode.Wiping;
        }
        else
        {
            Mode = CleaningMode.Sweeping;
        }
    }

    private void AddDustProgress()
    {
        _player.AddDust();
        //_cleanProgress.value = 초기 오염량 - (현재 오염량 / 초기 오염량)
    }

    private void CheckReturnCondition()
    {
        if (_player.CheckBatteryValue() <= 10f || _player.CheckDustValue() >= 99.9f)
        {
            _warning.gameObject.SetActive(true);
        }
        else
        {
            _warning.gameObject.SetActive(false);
        }
    }
}
