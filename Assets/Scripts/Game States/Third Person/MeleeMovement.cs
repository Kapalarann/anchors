using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Ensure correct Cinemachine import

public class MeleeMovement : MonoBehaviour
{
    public float MoveSpeed { get; private set; } = 5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _rollSpeed = 5f;
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Transform _cameraTransform;

    private Rigidbody _rb;
    private Vector3 _direction = Vector3.zero;
    private bool _isSprinting = false;
    private Health _health;

    private Animator _animator;
    private PlayerAttack playerAttack;
    private AnimationManager _animManager;
    private static readonly int MovementSpeedHash = Animator.StringToHash("Movementspeed");
    private static readonly int OnRollHash = Animator.StringToHash("OnRoll");
    private static readonly int IsRollingHash = Animator.StringToHash("IsRolling");
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    public bool _isAttacking = false;
    public bool _isRolling = false;

    private PlayerInput _playerInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _animManager = GetComponent<AnimationManager>();

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            _playerInput.actions.Enable();
        }

        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform; // Get the main camera's transform
        }

        _health = GetComponent<Health>(); // Get Health component
    }

    private void FixedUpdate()
    {
        if (_isRolling) return;
        if (_isAttacking)
        {
            _direction = Vector3.zero;
            _animator.SetBool(IsRunningHash, false);
            return;
        }

        float currentSpeed = _isSprinting ? _sprintSpeed : _moveSpeed;

        // Convert input direction to camera-relative movement
        Vector3 moveDirection = CameraRelativeMovement(_direction);
        Vector3 newPosition = _rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);

        float speedValue = _direction.magnitude * currentSpeed;
        _animator.SetFloat(MovementSpeedHash, speedValue);
        _animator.SetBool(IsRunningHash, _isSprinting && _direction.magnitude > 0);

        // Rotate character towards movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    private Vector3 CameraRelativeMovement(Vector3 inputDirection)
    {
        if (_cameraTransform == null) return inputDirection;

        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        return (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
    }

    public void Move_Event(InputAction.CallbackContext context)
    {
        if (_isAttacking)
        {
            _direction = Vector3.zero;
            return;
        }

        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _direction = new Vector3(input.x, 0, input.y);
        }
        else if (context.canceled)
        {
            _direction = Vector3.zero;
        }
    }

    public void Sprint_Event(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isSprinting = true;
        }
        else if (context.canceled)
        {
            _isSprinting = false;
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _moveSpeed = 5f * multiplier;
        _sprintSpeed = 8f * multiplier;
    }

    public void Roll_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_isRolling)
        {
            _animator.SetTrigger(OnRollHash);
            _animator.SetBool(IsRollingHash, true);
            _isRolling = true;
            _isAttacking = false;
            if (playerAttack != null) playerAttack._isAttacking = false;
            _isSprinting = false;
            _animator.SetBool(IsRunningHash, false);
            _animManager.stopFlinching();

            if (_health != null)
            {
                _health.isInvulnerable = true;
            }
        }
    }

    public void ApplyRollMovement()
    {
        Vector3 rollDirection = CameraRelativeMovement(_direction) * _rollSpeed;
        _rb.AddForce(rollDirection, ForceMode.VelocityChange);
    }

    public void EndRoll()
    {
        _isRolling = false;
        _animator.SetBool(IsRollingHash, false);

        if (_health != null)
        {
            _health.isInvulnerable = false;
        }
    }

    public void AttackMove()
    {
        if (_rb == null) return;
        Vector3 dir = transform.forward * GetComponent<PlayerAttack>().attackForce;
        if (dir == null) return;
        _rb.AddForce(dir, ForceMode.VelocityChange);
    }

    public void ResetPlayerState()
    {
        _isAttacking = false;
        _isRolling = false;
        _isSprinting = false;
        _animator.SetBool(IsRollingHash, false);
        _animator.SetBool(IsRunningHash, false);

        if (_health != null)
        {
            _health.isInvulnerable = false;
        }
    }
}
