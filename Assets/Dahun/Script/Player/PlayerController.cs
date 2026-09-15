using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isBlockedByWall = false;
    [SerializeField] private float _rotationSyncSpeed = 20f;
    [SerializeField] private CameraMovement _cameraMovement;
    [SerializeField] private LayerMask _collisionMask;

    private PlayerStats _playerStats;

    public bool IsSetDirection;
    public bool IsCanMove;

    private float _moveSpeed;

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;


    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _playerStats = GetComponent<PlayerStats>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _isMoving = false;
        IsSetDirection = false;
        IsCanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        _moveSpeed = _playerStats.CurrentSpeed;

        if (Keyboard.current.wKey.isPressed)
        {
            if (!_isBlockedByWall && IsCanMove)
            {
                SetDirection();
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
                PlayerMoveStop();
            }
        }
        if (IsSetDirection)
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
        else
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
    }

    private void FixedUpdate()
    {
        if (_isMoving && !_isBlockedByWall && IsCanMove)
        {
            MoveForward();
        }
    }

    private void RotatePlayer()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (mouseDelta.x != 0)
        {
            _isBlockedByWall = false;
        }

        if (IsSetDirection) return;

        float targetYaw = _cameraMovement.FreeYaw;
        float newYaw = Mathf.LerpAngle(transform.eulerAngles.y, targetYaw, _rotationSyncSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
    }

    private void SetDirection()
    {
        SoundManager.Instance.PlayMoveSound();
        IsSetDirection = true;
    }

    private void MoveForward()
    {
        float moveDistance = _moveSpeed * Time.fixedDeltaTime;
        float radius = _capsuleCollider.radius;

        if (Physics.SphereCast(_rigidbody.position, radius, transform.forward, out RaycastHit hit, moveDistance, _collisionMask))
        {
            // 벽에 거의 다 닿았으면(정면충돌) 멈춤 처리
            if (Vector3.Dot(transform.forward, hit.normal) < -0.45f)
            {
                PlayerMoveStop();
                return;
            }

            // 벽 표면을 따라 미끄러지는 방향 계산
            Vector3 slideDirection = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            _rigidbody.MovePosition(_rigidbody.position + slideDirection * moveDistance);
        }
        else
        {
            _rigidbody.MovePosition(_rigidbody.position + transform.forward * moveDistance);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _isBlockedByWall = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            SoundManager.Instance.PlaySFX(SoundManager.SFX.BumpWall);
            PlayerMoveStop();
        }
        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            _rigidbody.position += normal * 0.05f;
        }
    }

    public void PlayerMoveStop()
    {
        _isMoving = false;
        IsSetDirection = false;
        _isBlockedByWall = true;
        SoundManager.Instance.StopMoveSound();
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
