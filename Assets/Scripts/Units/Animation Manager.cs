using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [HideInInspector] public Animator _animator;
    private PlayerAttack attack;
    private MeleeMovement movement;

    public bool isFlinching = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        attack = GetComponent<PlayerAttack>();
        movement = GetComponent<MeleeMovement>();
    }

    public void Flinch(bool isStunned)
    {
        if (attack != null)
        {
            attack._isAttacking = false;
            attack.DisableWeaponCollider();
        }

        if (movement != null)
        {
            movement._isRolling = false;
            movement._isAttacking = false;
        }

        _animator.SetFloat("Movementspeed", 0f);
        _animator.SetBool("IsRunning", false);
        _animator.SetBool("isDrawing", false);
        _animator.SetBool("IsRolling", false);
        _animator.SetBool("isStunned", isStunned);

        isFlinching = true;
    }

    public void stopFlinching()
    {
        isFlinching = false;
    }
}
