using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] AnimationReceiver _receiver;
    MeleeMovement _meleeMovement;

    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    private static readonly int OnHeavyAttackHash = Animator.StringToHash("OnHeavyAttack");

    private void Awake()
    {
        _meleeMovement = GetComponent<MeleeMovement>();
        _animator = GetComponent<Animator>();
        _receiver.AttackEnd += EndAttack;
    }

    public void Attack_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_meleeMovement._isRolling)
        {
            Debug.Log("Light Attack triggered!");
            _animator.SetTrigger(OnAttackHash);
            _meleeMovement._isAttacking = true;
        }
    }

    public void HeavyAttack_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_meleeMovement._isRolling)
        {
            Debug.Log("Heavy Attack triggered!");
            _animator.SetTrigger(OnHeavyAttackHash);
            _meleeMovement._isAttacking = true;
        }
    }

    private void OnDestroy()
    {
        _receiver.AttackEnd -= EndAttack;
    }

    public void EndAttack(AnimationEvent animationEvent)
    {
        Debug.Log("Attack animation finished!");
        _meleeMovement._isAttacking = false;
    }
}
