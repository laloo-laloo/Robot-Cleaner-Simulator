using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform Player;
    public PlayerController PlayerController;

    [SerializeField] private Vector3 _offset = new Vector3(0, 1, -1.5f);
    [SerializeField] private float _freeLookSensitivity = 0.1f;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _collisionBuffer = 0.2f;

    private float _freeYaw;

    void Update()
    {
        float yaw;

        if (PlayerController.IsSetDirection)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            _freeYaw += mouseDelta.x * _freeLookSensitivity;
            yaw = _freeYaw;
        }
        else
        {
            _freeYaw = Player.eulerAngles.y;
            yaw = Player.eulerAngles.y;
        }

        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 desiredPosition = Player.position + yawRotation * _offset;

        transform.position = GetCollisionAdjustedPosition(desiredPosition);
        transform.rotation = yawRotation;
    }

    private Vector3 GetCollisionAdjustedPosition(Vector3 desiredPosition)
    {
        Vector3 direction = desiredPosition - Player.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(Player.position, direction.normalized, out RaycastHit hit, distance, _collisionMask))
        {
            return Player.position + direction.normalized * (hit.distance - _collisionBuffer);
        }

        return desiredPosition;
    }
}