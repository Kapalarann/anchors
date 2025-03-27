using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class UnitStateManager : MonoBehaviour
{
    private UnitState _currentState;

    [HideInInspector] public UnitIdleState idleState = new UnitIdleState();
    [HideInInspector] public UnitMoveState moveState = new UnitMoveState();
    [HideInInspector] public UnitAttackState attackState = new UnitAttackState();

    [Header("Movement")]
    [SerializeField] public float _movementSpeed;
    [HideInInspector]public Vector3 _targetPosition;

    [Header("Strafing")]
    [SerializeField] public bool strafes;
    [SerializeField] public float strafeStrength;

    [Header("Shooting")]
    [SerializeField] public bool isRanged;
    [SerializeField] public RangeAttack[] rangeAttacks;

    [HideInInspector] public UnitStats _unitStat;
    [HideInInspector] public UnitStats _target;

    [HideInInspector] public NavMeshAgent _agent;
    [HideInInspector] public Animator _animator;

    private void Awake()
    {
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

    public void Update()
    {
        foreach (var attack in rangeAttacks)
        {
            attack.attackTimer += Time.deltaTime;
            if (attack.attackTimer >= attack.attackCooldown) 
            {
                attack.attackTimer = 0f;
                Transform tar = AquireTarget();
                if(tar == null) continue;
                FireProjectile(tar.position, attack);
            }
        }

        _currentState?.Update(this);
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
        return GameStateManager.Instance.currentAgent.transform;
    }

    public void FireProjectile(Vector3 targetPosition, RangeAttack attack)
    {
        Vector3 direction = (targetPosition + attack.fireOffset) - attack.firePoint.position;
        float distance = new Vector3(direction.x, 0, direction.z).magnitude; // Horizontal distance
        float height = direction.y; // Vertical height

        // Calculate the launch angle
        if (CalculateLaunchAngle(distance, height, attack.projectileSpeed, -Physics.gravity.y, out float angle))
        {
            // Convert angle to a directional vector
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            Vector3 launchDirection = rotation * Quaternion.Euler(-angle, 0, 0) * Vector3.forward;

            // Instantiate and fire the projectile
            GameObject projectile = Instantiate(attack.projectilePrefab, attack.firePoint.position, Quaternion.LookRotation(launchDirection));
            projectile.GetComponent<PlayerBullet>().damage = attack.damage;
            projectile.GetComponent<PlayerBullet>().headshotMult = attack.headshotMultiplier;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = launchDirection * attack.projectileSpeed;
            }
        }
        else
        {
            Debug.LogWarning("No valid firing angle found!");
        }
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
}

[Serializable]
public class RangeAttack
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Vector3 fireOffset;
    public int projectileCount;
    public float attackCooldown;
    [HideInInspector]public float attackTimer = 0f;
    public float projectileSpeed;
    public float damage;
    public float headshotMultiplier;
    public float weight;
}