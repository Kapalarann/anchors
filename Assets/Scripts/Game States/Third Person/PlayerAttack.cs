using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private HealthAndStamina hp;
    private Animator _animator;
    [SerializeField] private AnimationReceiver _receiver;
    private AnimationManager _manager;
    private MeleeMovement _meleeMovement;

    [Header("Weapon Settings")]
    [SerializeField] private ParticleSystem swordTrail;
    [SerializeField] private Collider weaponCollider; // Weapon collider for detecting hits
    [SerializeField] private SwordCollision sword;
    [SerializeField] private float lightAttackDamage = 10f; // Base light attack damage
    [SerializeField] private float lightStaminaCost = 3f;
    [SerializeField] private float heavyAttackDamage = 20f; // Base heavy attack damage
    [SerializeField] private float heavyStaminaCost = 5f;
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
        hp = GetComponent<HealthAndStamina>();
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
    }

    private void OnDisable()
    {
        sword.isTransfer = false;
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

            hp.ConsumeStamina(lightStaminaCost);
            if (hp.isStunned) return;
            _animator.SetFloat("attackSpeed", attackSpeed);
            _animator.SetTrigger(OnAttackHash);
            sword.damage = lightAttackDamage;
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

            hp.ConsumeStamina(heavyStaminaCost);
            if (hp.isStunned) return;
            _animator.SetFloat("attackSpeed", attackSpeed);
            _animator.SetTrigger(OnHeavyAttackHash);
            sword.damage = heavyAttackDamage;
            _meleeMovement._isAttacking = true;
            _isAttacking = true;
        }
    }

    private Transform FindBestEnemy()
    {
        Transform closestEnemy = null;
        float closestDistance = detectionRange;
        float bestSnapDistance = maxSnapDistance;

        UnitStats[] unitList = UnitStats.GetUnitsArray();
        for (int i = 0; i < unitList.Length; i++)
        {
            var enemy = unitList[i];
            if (enemy.gameObject == this.gameObject) continue;

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
            swordTrail.Play();
        }
    }

    // Disable weapon collider after attack animation
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            swordTrail.Stop();
        }
    }

    public void EndAttack(AnimationEvent animationEvent)
    {
        _meleeMovement._isAttacking = false;
        _meleeMovement._direction = _meleeMovement._lastDirection;
        _isAttacking = false;
    }

    private void OnDestroy()
    {
        _receiver.AttackEnd -= EndAttack;
    }

    public void ModifyDamage(float multiplier)
    {
        lightAttackDamage = baseLightAttackDamage * multiplier;
        heavyAttackDamage = baseHeavyAttackDamage * multiplier;
    }

    public void ResetDamage()
    {
        lightAttackDamage = baseLightAttackDamage;
        heavyAttackDamage = baseHeavyAttackDamage;
    }
}
