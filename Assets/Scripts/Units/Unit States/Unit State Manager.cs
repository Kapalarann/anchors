using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitStateManager : MonoBehaviour
{
    private UnitState _currentState;

    [HideInInspector] public UnitIdleState idleState = new UnitIdleState();
    [HideInInspector] public UnitMoveState moveState = new UnitMoveState();
    [HideInInspector] public UnitAttackState attackState = new UnitAttackState();

    [Header("Movement")]
    [SerializeField] public float idleTime;
    [SerializeField] public float _movementSpeed;
    [HideInInspector]public Vector3 _targetPosition;

    [Header("Strafing")]
    [SerializeField] public bool strafes;
    [SerializeField] public float strafeStrength;

    [Header("Melee Combat")]
    [SerializeField] public bool isMelee;
    [SerializeField] public MeleeAttack[] meleeAttacks;

    [Header("Shooting")]
    [SerializeField] public bool isRanged;
    [SerializeField] public RangeAttack[] rangeAttacks;
    [HideInInspector] public int currentAttack = 0;

    [HideInInspector] public UnitStats _unitStat;
    [HideInInspector] public UnitStats _target;

    [HideInInspector] public HealthAndStamina _health;
    [HideInInspector] public NavMeshAgent _agent;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public bool isAttacking = false;

    private void Awake()
    {
        _health = GetComponent<HealthAndStamina>();
        _unitStat = GetComponent<UnitStats>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetState(new UnitIdleState());
    }

    public void SetState(UnitState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
        }

        _currentState = newState;
        _currentState.Enter(this);
    }

    public void FixedUpdate()
    {
        if (_health.isStunned) return; 

        if(_animator != null) _animator.SetFloat("Movementspeed", _agent.velocity.magnitude);

        if(isRanged){
            for (int i = 0; i < rangeAttacks.Length; i++)
            {
                rangeAttacks[i].attackTimer += Time.deltaTime;
            }
        }

        if(isMelee){
            for (int i = 0; i < meleeAttacks.Length; i++)
            {
                meleeAttacks[i].attackTimer += Time.deltaTime;
            }
        }

        _currentState?.FixedUpdate(this);
    }

    private void OnEnable()
    {
        if(_agent != null) _agent.enabled = true;
    }
    private void OnDisable()
    {
        _agent.enabled = false;
    }

    public void MoveTo(Vector3 position)
    {
        _targetPosition = position;
        SetState(moveState);
    }

    public Transform AquireTarget()
    {
        if(GameStateManager.Instance.currentAgent != null) return GameStateManager.Instance.currentAgent.transform;
        return null;
    }

    public void FireProjectile()
    {
        Vector3 targetPosition = _target.transform.position;
        RangeAttack attack = rangeAttacks[currentAttack];
        Vector3 direction = (targetPosition + attack.fireOffset) - attack.firePoint.position;
        float distance = new Vector3(direction.x, 0, direction.z).magnitude;
        float height = direction.y;
        float angle = 0f;

        if (attack.hasGravity && !CalculateLaunchAngle(distance, height, attack.projectileSpeed, -Physics.gravity.y, out angle))
        {
            return; // Target out of range, don't fire
        }

        GameObject projectile = Instantiate(attack.projectilePrefab, attack.firePoint.position, Quaternion.identity);
        PlayerBullet proj = projectile.GetComponent<PlayerBullet>();
        Spell spell = projectile.GetComponent<Spell>();

        proj.shooterTransform = transform;
        proj.damage = attack.damage;
        proj.headshotMult = attack.headshotMultiplier;

        if(spell != null)
        {
            spell.Cast(targetPosition + attack.fireOffset);
            spell.SetTarget(_target.transform);
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Vector3 launchDirection = Vector3.zero;
        if (attack.hasGravity)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            launchDirection = rotation * Quaternion.Euler(-angle, 0, 0) * Vector3.forward;
            projectile.transform.rotation = Quaternion.LookRotation(launchDirection);
        }
        else
        {
            launchDirection = direction.normalized * attack.projectileSpeed;
            projectile.transform.rotation = Quaternion.LookRotation(launchDirection);
        }
        rb.linearVelocity = launchDirection * attack.projectileSpeed;
        if(attack.projectileDuration > 0) Destroy(projectile, attack.projectileDuration);
    }

    private bool CalculateLaunchAngle(float distance, float height, float speed, float gravity, out float angle)
    {
        float speedSquared = speed * speed;
        float underRoot = (speedSquared * speedSquared) - gravity * (gravity * distance * distance + 2 * height * speedSquared);

        if (underRoot < 0)
        {
            angle = 0;
            return false; // No real solution, target is unreachable
        }

        float root = Mathf.Sqrt(underRoot);
        float angle1 = Mathf.Atan((speedSquared + root) / (gravity * distance)) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan((speedSquared - root) / (gravity * distance)) * Mathf.Rad2Deg;

        angle = Mathf.Min(angle1, angle2); // Choose the lower arc
        return true;
    }

    public void DoneAttacking()
    {
        isAttacking = false;
    }
}

[Serializable]
public class RangeAttack
{
    public GameObject projectilePrefab;
    public string animationTrigger;
    public Transform firePoint;
    public bool hasGravity;
    public Vector3 fireOffset;
    public float maxRange;
    public float projectileDuration;
    public float attackCooldown;
    [HideInInspector]public float attackTimer = 0f;
    public float projectileSpeed;
    public float damage;
    public float headshotMultiplier;
    public float weight;
}

[Serializable]
public class MeleeAttack
{
    public GameObject meleeObj;
    public string animationTrigger;
    public float attackSpeed;
    public float attackRange;
    public float attackCooldown;
    [HideInInspector] public float attackTimer = 0f;
    public float damage;
}