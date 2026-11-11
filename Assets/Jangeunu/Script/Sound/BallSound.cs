using UnityEngine;

public class BallSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player"))
        {
            PlayTingTingSound();
        }
    }

    private void PlayTingTingSound()
    {
        if(_audioSource != null && _audioSource.clip != null)
        {
            _audioSource.PlayOneShot(_audioSource.clip);
        }
    }
}
