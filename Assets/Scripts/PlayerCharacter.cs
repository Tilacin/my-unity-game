using Colyseus.Schema;
using KinematicCharacterController.Examples;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _maxHeadAngle = 90;
    [SerializeField] private float _minHeadAngle = -90;
    [SerializeField] private float _jumpForce = 5;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private float _jumpDelay = .2f;
    [Header("Camera")]
    [SerializeField] private ExampleCharacterCamera _gameCamera;
    [SerializeField] private float _mouseSensitivity = 1f;

    private float _inputH;
    private float _inputV;
    private float _rotateY;
    private float _currentRotateX;
    private float _jumpTime;
    private float _mouseX;
    private float _mouseY;
    private bool _isCameraLocked;

    public void SetCamera(ExampleCharacterCamera cam)
    {
        _gameCamera = cam;
        if (_gameCamera != null)
        {
            _gameCamera.SetFollowTransform(_cameraPoint != null ? _cameraPoint : transform);
        }
    }
    public void SetInput(float h, float v, float rotateY)
    {
        _inputH = h;
        _inputV = v;
        _rotateY += rotateY;
    }
    private void Update()
    {
        if (!_isCameraLocked)
        {
            _mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            _mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            if (_gameCamera != null)
            {
                // Всегда передаем инпут в камеру, даже если кнопка не зажата
                // Но rotateY добавляем только при зажатой кнопке
                _gameCamera.UpdateWithInput(Time.deltaTime, 0, new Vector3(_mouseX, _mouseY, 0));
            }

            if (Input.GetMouseButton(1))
            {
                _rotateY += _mouseX;
            }
        }
        
    }
    private void FixedUpdate()
    {
        Move();
        RotateY();
    }

    private void Move()
    {
        Vector3 cameraForward = _gameCamera.transform.forward;
        Vector3 cameraRight = _gameCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * _inputV + cameraRight * _inputH).normalized;

        // Если есть движение - поворачиваем модель в сторону движения
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
           transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
           
        }

        Vector3 velocity = moveDirection * speed;
        velocity.y = _rigidbody.velocity.y;
        base.velocity = velocity;
        _rigidbody.velocity = base.velocity;
    }
    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }

    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY)
    {
        position = transform.position;
        velocity = _rigidbody.velocity;
        rotateX = _head.localEulerAngles.x;
        rotateY = transform.eulerAngles.y;
    }

    private bool _isFly = true;

    private void OnCollisionStay(Collision collision)
    {
        var contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (contactPoints[i].normal.y > .45f) _isFly = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isFly = true;
    }

    public void Jump()
    {
        if (_checkFly.IsFly) return;
        if (Time.time - _jumpTime < _jumpDelay) return;

        _jumpTime = Time.time;
        _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);
    }

    internal void OnChange(List<DataChange> changes)
    {
        foreach (var dataChange in changes)
        {
            switch (dataChange.Field)
            {
                case "loss":
                    MultiplayerManager.Instance._lossCounter.SetPlayerLoss((byte)dataChange.Value);
                    break;
                default:
                    break;
            }
        }
    }
    public void LockCamera(bool locked)
    {
        _isCameraLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = locked;
    }
    public Transform GetCameraFollowPoint()
    {
        return _cameraPoint != null ? _cameraPoint : transform;
    }
}
