using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private AnimationReceiver _receiver;
    private AnimationManager _manager;
    private MeleeMovement _meleeMovement;

    [Header("Weapon Settings")]
    [SerializeField] private Collider weaponCollider; // Weapon collider for detecting hits
    [SerializeField] private SwordCollision sword;
    [SerializeField] private float lightAttackDamage = 10f; // Base light attack damage
    [SerializeField] private float heavyAttackDamage = 20f; // Base heavy attack damage
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] public float attackForce = 1f;

    [Header("Targeting Settings")]
    [SerializeField] private float maxSnapDistance = 2f; // Magnetic snapping max distance
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float detectionAngle = 90f;

    private float baseLightAttackDamage;
    private float baseHeavyAttackDamage;

    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    private static readonly int OnHeavyAttackHash = Animator.StringToHash("OnHeavyAttack");

    public bool _isAttacking = false;

    private void Awake()
    {
        _meleeMovement = GetComponent<MeleeMovement>();
        _animator = GetComponent<Animator>();
        _manager = GetComponent<AnimationManager>();
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
        if (context.performed && !_meleeMovement._isRolling && !_isAttacking && !_manager.isFlinching)
        {
            Transform target = FindBestEnemy();
            if (target != null)
            {
                RotateTowardsTarget(target);
            }

            _animator.SetFloat("attackSpeed", attackSpeed);
            _animator.SetTrigger(OnAttackHash);
            _meleeMovement._isAttacking = true;
            _isAttacking = true;
        }
    }

    public void HeavyAttack_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !_meleeMovement._isRolling && !_isAttacking && !_manager.isFlinching)
        {
            Transform target = FindBestEnemy();
            if (target != null)
            {
                RotateTowardsTarget(target);
            }

            _animator.SetTrigger(OnHeavyAttackHash);
            _meleeMovement._isAttacking = true;
            _isAttacking = true;
        }
    }

    private Transform FindBestEnemy()
    {
        Transform closestEnemy = null;
        float closestDistance = detectionRange;
        float bestSnapDistance = maxSnapDistance;

        foreach (var enemy in UnitStats.units)
        {
            Transform enemyTransform = enemy.transform;
            Vector3 directionToEnemy = (enemyTransform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
            float distanceToEnemy = Vector3.Distance(transform.position, enemyTransform.position);

            // Must be within the attack cone
            if (angleToEnemy < detectionAngle / 2 && distanceToEnemy < detectionRange)
            {
                // If within magnetic snapping distance, prioritize snapping
                if (distanceToEnemy < bestSnapDistance)
                {
                    bestSnapDistance = distanceToEnemy;
                    closestEnemy = enemyTransform;
                }
                // Otherwise, just find the closest
                else if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemyTransform;
                }
            }
        }
        return closestEnemy;
    }

    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Keep rotation horizontal
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void TransferToggle_Event(InputAction.CallbackContext context)
    {
        if(sword.isTransfer) sword.isTransfer = false;
        else sword.isTransfer = true;
    }

    // Enable weapon collider during attack animation
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    // Disable weapon collider after attack animation
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    public void EndAttack(AnimationEvent animationEvent)
    {
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
