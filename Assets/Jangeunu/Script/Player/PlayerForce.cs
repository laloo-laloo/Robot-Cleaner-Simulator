using UnityEngine;

public class PlayerForce : MonoBehaviour
{
    [SerializeField] private float _force;

    public float Force => _force;

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
                ballRb.AddForce(pushDirection * _force, ForceMode.Impulse);
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("Desk"))
        {
            Debug.Log("데스크 충돌");
            KitchenDesk kitchenDesk = other.GetComponent<KitchenDesk>();

            if (kitchenDesk != null)
            {
                Vector3 direction = transform.forward;
                Debug.Log("힘 전달 시도");
                kitchenDesk.HitDesk(direction, _force);
            }

        }
    }
}
