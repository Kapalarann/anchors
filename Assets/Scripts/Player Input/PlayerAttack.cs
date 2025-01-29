using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    //private bool _isAttacking = false;
   [SerializeField] AnimationReceiver _receiver;
    MeleeMovement _meleeMovement;

    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    private void Awake()
    {
        _meleeMovement = GetComponent<MeleeMovement>();
        _animator = GetComponent<Animator>();
        _receiver.AttackEnd += EndAttack;
    }

    public void Attack_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_meleeMovement._isAttacking)
        {
            Debug.Log("Attack button pressed!");
            _animator.SetTrigger(OnAttackHash); // ✅ Triggers "OnAttack"
            _meleeMovement._isAttacking = true;
        }
    }

        
    private void OnDestroy()
    {
        _receiver.AttackEnd -= EndAttack;
    }

    // This function is called from AnimationReceiver when the attack animation ends
    public void EndAttack(AnimationEvent animationEvent)
    {
        Debug.Log("Resetting attack state.");
        _meleeMovement._isAttacking = false;
    }
}
