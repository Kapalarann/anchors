using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Ensure correct Cinemachine import

public class MeleeMovement : MonoBehaviour
{
    public float MoveSpeed { get; private set; } = 5f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rollSpeed = 5f;
    [SerializeField] private float _rollDuration = 0.5f;
    [SerializeField] private CinemachineCamera _cinemachineCamera; // Cinemachine reference
    [SerializeField] private Transform _cameraFollowTarget; // Target for the camera to follow

    private Rigidbody _rb;
    private Vector3 _direction = Vector3.zero;

    private Animator _animator;
    private static readonly int MovementSpeedHash = Animator.StringToHash("Movementspeed");
    private static readonly int OnRollHash = Animator.StringToHash("OnRoll");
    private static readonly int IsRollingHash = Animator.StringToHash("IsRolling");

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
    }

    private void FixedUpdate()
    {
        if (_isRolling) return; // Prevent movement during rolling
        if (_isAttacking) // Disable movement but allow rolling
        {
            _direction = Vector3.zero;
            return;
        }

        Vector3 normalizedDirection = _direction.normalized;
        Vector3 newPosition = _rb.position + normalizedDirection * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);

        float speedValue = _direction.magnitude * _moveSpeed;
        _animator.SetFloat(MovementSpeedHash, speedValue);

        if (_direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            UpdateCameraRotation(targetRotation);
        }
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
            _direction.x = input.x;
            _direction.z = input.y;
        }
        else
        {
            _direction = Vector3.zero;
        }
    }

    public void Roll_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_isRolling)
        {
            Debug.Log("Rolling!");
            _animator.SetTrigger(OnRollHash);
            _animator.SetBool(IsRollingHash, true);
            _isRolling = true;
            _isAttacking = false;
            Invoke(nameof(EndRoll), _rollDuration);
        }
    }

    public void ApplyRollMovement()
    {
        Debug.Log("Applying Roll Movement...");
        Vector3 rollDirection = transform.forward * _rollSpeed;
        _rb.AddForce(rollDirection, ForceMode.VelocityChange);
    }

    public void EndRoll()
    {
        Debug.Log("Rolling finished.");
        _isRolling = false;
        _animator.SetBool(IsRollingHash, false);
    }

    public void ResetPlayerState()
    {
        _isAttacking = false;
        _isRolling = false;
        _animator.SetBool(IsRollingHash, false);
    }

    private void UpdateCameraRotation(Quaternion targetRotation)
    {
        if (_cameraFollowTarget != null)
        {
            _cameraFollowTarget.rotation = targetRotation; // Rotate camera follow target
        }
    }
}
