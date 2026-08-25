using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public enum SFX
    {
        Upgrade,
        SuckDust,
        WipeLipuid,
        BumpWall,
        UIClick,
        RechargingBattery
    }

    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip[] _sfxClips;

    [SerializeField] private AudioSource _moveAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(SFX type)
    {
        int index = (int)type;
        if (index >= 0 && index < _sfxClips.Length && _sfxClips[index] != null)
        {
            _sfxSource.PlayOneShot(_sfxClips[index]);
        }
    }

    public void PlayMoveSound()
    {
        if (_moveAudioSource != null && !_moveAudioSource.isPlaying)
        {
            _moveAudioSource.Play();
        }
    }

    public void StopMoveSound()
    {
        if (_moveAudioSource != null && _moveAudioSource.isPlaying)
        {
            _moveAudioSource.Stop();
        }
    }

    public void SetSFXVolume(float value)
    {
        _sfxSource.volume = value;
        _moveAudioSource.volume = value;
    }
}
