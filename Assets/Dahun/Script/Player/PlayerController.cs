using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isBlockedByWall = false;
    public bool IsSetDirection;

    [SerializeField] private float _moveSpeed = 1f;

    private Rigidbody _rigidbody;

    public float MouseSensitivity = 0.1f;

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
        if (!IsSetDirection)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            // 회전 입력(마우스 움직임)이 발생하면 벽 막힘 상태 해제! 
            // (벽에서 마우스를 돌려 다른 곳을 바라봤을 때 즉시 탈출 가능하게 하기 위함)
            if (mouseDelta.x != 0)
            {
                _isBlockedByWall = false;
            }

            transform.Rotate(Vector3.up * mouseDelta.x * MouseSensitivity, Space.World);
        }
    }

    private void SetDirection()
    {
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
            PlayerMoveStop();
        }
    }

    public void PlayerMoveStop()
    {
        _isMoving = false;
        IsSetDirection = false;
        _isBlockedByWall = true;

        if (_rigidbody != null)
        {
            // 남아있는 선형/회전 속도 강제 0으로 초기화
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
