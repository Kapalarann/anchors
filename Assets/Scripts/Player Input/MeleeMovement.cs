using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeMovement : MonoBehaviour
{
    public float MoveSpeed { get; private set; } = 5f;

    [SerializeField] private float _moveSpeed = 5f;
    private Rigidbody _rb;
    private Vector3 _direction = Vector3.zero;

    private Animator _animator;
    private static readonly int MovementSpeedHash = Animator.StringToHash("Movementspeed");
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    public bool _isAttacking = false;

    private PlayerInput _playerInput;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            _playerInput.actions.Enable();  // Ensure input system is enabled
        }
    }

    private void FixedUpdate()
    {
        if (_isAttacking) return;

        Vector3 normalizedDirection = _direction.normalized;
        Vector3 newPosition = _rb.position + normalizedDirection * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);

        float speedValue = _direction.magnitude * _moveSpeed;
        _animator.SetFloat(MovementSpeedHash, speedValue);

        if (_direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
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
            _direction.x = input.x;
            _direction.z = input.y;
        }
        else
        {
            _direction = Vector3.zero;
        }
    }



    // **This function should be called via an Animation Event at the end of the attack animation**
    public void EndAttack()
    {
        Debug.Log("Attack animation finished! Resetting attack state.");
        _isAttacking = false;
    }

}
