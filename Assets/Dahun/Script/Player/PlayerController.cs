using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isBlockedByWall = false;
    [SerializeField] private CameraMovement _cameraMovement;
    [SerializeField] private float _rotationSyncSpeed = 20f;
    private PlayerStats _playerStats;

    public bool IsSetDirection;

    private float _moveSpeed;

    private Rigidbody _rigidbody;


    private void Awake()
    {
        
        _playerStats = GetComponent<PlayerStats>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _isMoving = false;
        IsSetDirection = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        _moveSpeed = _playerStats.CurrentSpeed;
        if (Keyboard.current.wKey.isPressed)
        {
            if (!_isBlockedByWall)
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
        if (_isMoving && !_isBlockedByWall)
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
        _rigidbody.MovePosition(_rigidbody.position + transform.forward * _moveSpeed * Time.fixedDeltaTime);
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
