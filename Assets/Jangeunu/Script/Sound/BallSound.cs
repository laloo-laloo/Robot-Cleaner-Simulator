using UnityEngine;

public class BallSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player"))
        {
            PlayTingTingSound();
        }
    }

    private void PlayTingTingSound()
    {
        if(audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
