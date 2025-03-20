using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private AnimationReceiver _receiver;
    private MeleeMovement _meleeMovement;

    [Header("Weapon Settings")]
    [SerializeField] private Collider weaponCollider; // Weapon collider for detecting hits
    [SerializeField] private SwordCollision sword;
    [SerializeField] private float lightAttackDamage = 10f; // Base light attack damage
    [SerializeField] private float heavyAttackDamage = 20f; // Base heavy attack damage
    [SerializeField] private float attackSpeed = 1f;

    private float baseLightAttackDamage;
    private float baseHeavyAttackDamage;

    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    private static readonly int OnHeavyAttackHash = Animator.StringToHash("OnHeavyAttack");

    private bool _isAttacking = false;

    private void Awake()
    {
        _meleeMovement = GetComponent<MeleeMovement>();
        _animator = GetComponent<Animator>();
        sword._animator = _animator;
        _receiver.AttackEnd += EndAttack;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // Disable the weapon collider by default
        }

        // Initialize base damage values
        baseLightAttackDamage = lightAttackDamage;
        baseHeavyAttackDamage = heavyAttackDamage;

        UpdateSword();
    }

    public void Attack_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_meleeMovement._isRolling && !_isAttacking)
        {
            Debug.Log("Light Attack triggered!");
            _animator.SetFloat("attackSpeed", attackSpeed);
            _animator.SetTrigger(OnAttackHash);
            _meleeMovement._isAttacking = true;
            _isAttacking = true;
        }
    }

    public void HeavyAttack_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_meleeMovement._isRolling && !_isAttacking)
        {
            Debug.Log("Heavy Attack triggered!");
            _animator.SetTrigger(OnHeavyAttackHash);
            _meleeMovement._isAttacking = true;
            _isAttacking = true;
        }
    }

    // Enable weapon collider during attack animation
    public void EnableWeaponCollider(AnimationEvent animationEvent)
    {
        if (weaponCollider != null)
        {
            Debug.Log("Weapon collider enabled!");
            weaponCollider.enabled = true;
        }
    }

    // Disable weapon collider after attack animation
    public void DisableWeaponCollider(AnimationEvent animationEvent)
    {
        if (weaponCollider != null)
        {
            Debug.Log("Weapon collider disabled!");
            weaponCollider.enabled = false;
        }
    }

    public void EndAttack(AnimationEvent animationEvent)
    {
        Debug.Log("Attack animation finished!");
        _meleeMovement._isAttacking = false;
        _isAttacking = false;
    }

    private void OnDestroy()
    {
        _receiver.AttackEnd -= EndAttack;
    }

    // Modify damage values (and update Inspector)
    public void ModifyDamage(float multiplier)
    {
        lightAttackDamage = baseLightAttackDamage * multiplier;
        heavyAttackDamage = baseHeavyAttackDamage * multiplier;

        UpdateSword();

        Debug.Log($"Damage updated: Light = {lightAttackDamage}, Heavy = {heavyAttackDamage}");
    }

    // Reset damage values to their base
    public void ResetDamage()
    {
        lightAttackDamage = baseLightAttackDamage;
        heavyAttackDamage = baseHeavyAttackDamage;

        UpdateSword();

        Debug.Log("Damage reverted to base values.");
    }

    private void UpdateSword()
    {
        sword.lDamage = lightAttackDamage;
        sword.hDamage = heavyAttackDamage;
    }
}
