using UnityEngine;
using System.Collections;
using Spells;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public GameObject autoAttackPrefab;
    public GameObject homingPrefab; // Assign this in Unity
    public Transform attackSpawnPoint;

    private Transform targetEnemy;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Coroutine autoAttackCoroutine;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMovementInput();
        MoveCharacter();
    }

    void HandleMovementInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    targetEnemy = hit.collider.transform;
                    isMoving = true;

                    if (!isAttacking)
                    {
                        isAttacking = true;
                        autoAttackCoroutine = StartCoroutine(AutoAttackCoroutine());
                    }
                }
                else
                {
                    StopAutoAttack();
                    targetEnemy = null;
                    targetPosition = hit.point;
                    isMoving = true;
                }
            }
        }
    }

    void MoveCharacter()
    {
        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            if (distance <= attackRange)
            {
                isMoving = false;
                FaceTarget(targetEnemy);
                return;
            }
            else
            {
                targetPosition = targetEnemy.position;
                isMoving = true;
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    IEnumerator AutoAttackCoroutine()
    {
        while (isAttacking)
        {
            if (targetEnemy == null || Vector3.Distance(transform.position, targetEnemy.position) > attackRange)
            {
                StopAutoAttack();
                yield break;
            }

            FaceTarget(targetEnemy);
            ShootAutoAttack();
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    void ShootAutoAttack()
    {
        if (targetEnemy == null) return;

        if (homingPrefab == null)
        {
            Debug.LogError("HomingPrefab is not assigned in PlayerController!");
            return;
        }

        // Instantiate the homing projectile (now using HomingSpell)
        GameObject projectile = Instantiate(homingPrefab, attackSpawnPoint.position, Quaternion.identity);

        // Get the HomingSpell script instead of HomingProjectile
        HomingSpell homingScript = projectile.GetComponent<HomingSpell>();

        if (homingScript != null)
        {
            homingScript.SetTarget(targetEnemy);
        }
        else
        {
            Debug.LogError("HomingSpell script missing on homingPrefab!");
        }
    }

    void StopAutoAttack()
    {
        isAttacking = false;

        if (autoAttackCoroutine != null)
        {
            StopCoroutine(autoAttackCoroutine);
            autoAttackCoroutine = null;
        }
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
