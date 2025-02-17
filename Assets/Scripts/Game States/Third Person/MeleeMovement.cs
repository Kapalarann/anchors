using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Ensure correct Cinemachine import

public class MeleeMovement : MonoBehaviour
{
    public float MoveSpeed { get; private set; } = 5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _rollSpeed = 5f;
    [SerializeField] private float _rollDuration = 0.5f;
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Transform _cameraTransform; // Reference to the main camera

    private Rigidbody _rb;
    private Vector3 _direction = Vector3.zero;
    private bool _isSprinting = false;

    private Animator _animator;
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
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            _playerInput.actions.Enable();
        }

        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform; // Get the main camera's transform
        }
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

        // Get camera forward and right, ignoring vertical tilt
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0; // Ignore vertical tilt
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // Convert input into world-space movement direction
        Vector3 moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
        return moveDirection;
    }

    public void Move_Event(InputAction.CallbackContext context)
    {
        if (_isAttacking || _isRolling)
        {
            _direction = Vector3.zero;
            return;
        }

        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _direction = new Vector3(input.x, 0, input.y);
        }
        else
        {
            _direction = Vector3.zero;
        }
    }

    public void Sprint_Event(InputAction.CallbackContext context)
    {
        _isSprinting = context.started;
    }

    public void Roll_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_isRolling)
        {
            _animator.SetTrigger(OnRollHash);
            _animator.SetBool(IsRollingHash, true);
            _isRolling = true;
            _isAttacking = false;
            _isSprinting = false;
            _animator.SetBool(IsRunningHash, false);
            Invoke(nameof(EndRoll), _rollDuration);
        }
    }

    public void ApplyRollMovement()
    {
        Vector3 rollDirection = transform.forward * _rollSpeed;
        _rb.AddForce(rollDirection, ForceMode.VelocityChange);
    }

    public void EndRoll()
    {
        _isRolling = false;
        _animator.SetBool(IsRollingHash, false);
    }

    public void ResetPlayerState()
    {
        _isAttacking = false;
        _isRolling = false;
        _isSprinting = false;
        _animator.SetBool(IsRollingHash, false);
        _animator.SetBool(IsRunningHash, false);
    }
}
