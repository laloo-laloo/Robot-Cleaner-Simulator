using UnityEngine;

public class BGMScript : MonoBehaviour
{

    [SerializeField] private AudioSource _bgmSource;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetBGMVolume(float value)
    {
        _bgmSource.volume = value;
    }
}
