using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

public class MeleeMovement : MonoBehaviour
{
    public float MoveSpeed { get; private set; } = 5f;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _sprintStaminaCost = 2f;

    [Header("Dodge")]
    [SerializeField] private float _rollSpeed = 5f;
    [SerializeField] private float _rollStaminaCost = 3f;
    [SerializeField] private float _dodgeDuration = 1.05f;
    [SerializeField] private AnimationCurve _dodgeSpeedCurve;
    private Coroutine dodgeRoutine;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private Transform _cameraTransform;

    private Rigidbody _rb;
    [HideInInspector] public Vector3 _direction = Vector3.zero;
    [HideInInspector] public Vector3 _lastDirection = Vector3.zero;
    private bool _isSprinting = false;
    
    private HealthAndStamina _health;
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

        _health = GetComponent<HealthAndStamina>(); // Get Health component
    }

    private void FixedUpdate()
    {
        if (_isRolling || _health.isStunned) return;
        if (_isAttacking)
        {
            _direction = Vector3.zero;
            _animator.SetBool(IsRunningHash, false);
            return;
        }

        if(_isSprinting) _health.ConsumeStamina(_sprintStaminaCost * Time.deltaTime);
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
        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _lastDirection = new Vector3(input.x, 0, input.y);
            if (!_isAttacking) _direction = _lastDirection;
        }
        else if (context.canceled)
        {
            _direction = Vector3.zero;
            _lastDirection = Vector3.zero;
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
        if (context.performed && !_isRolling && !_health.isStunned && !_animManager.isFlinching)
        {
            _health.ConsumeStamina(_rollStaminaCost);
            if (_health.isStunned) return;
            _health.isInvulnerable = true;

            _animator.SetTrigger(OnRollHash);
            _animator.SetBool(IsRollingHash, true);
            _isRolling = true;
            _isAttacking = false;
            if (playerAttack != null) playerAttack._isAttacking = false;
            _isSprinting = false;
            _animator.SetBool(IsRunningHash, false);
        }
    }

    public void ApplyRollMovement()
    {
        if (dodgeRoutine != null) return;
        dodgeRoutine = StartCoroutine(DodgeRoutine());
    }

    private IEnumerator DodgeRoutine()
    {
        float elapsedTime = 0f;
        Vector3 dodgeDirection = _direction == Vector3.zero ? transform.forward : CameraRelativeMovement(_direction);

        while (elapsedTime < _dodgeDuration)
        {
            float speedMultiplier = _dodgeSpeedCurve.Evaluate(elapsedTime / _dodgeDuration);
            _rb.linearVelocity = dodgeDirection * (_rollSpeed * speedMultiplier);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rb.linearVelocity = Vector3.zero;
        dodgeRoutine = null;
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
