using UnityEngine;

public class BallKicker : MonoBehaviour
{
    [SerializeField] private float Force;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;

                pushDirection.y = 0.5f;
                pushDirection = pushDirection.normalized;

                ballRb.linearVelocity = Vector3.zero;
                ballRb.AddForce(pushDirection * Force, ForceMode.Impulse);
            }
        }
    }
}
