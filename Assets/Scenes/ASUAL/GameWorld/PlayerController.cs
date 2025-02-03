using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public GameObject autoAttackPrefab;
    public Transform attackSpawnPoint;

    private Transform targetEnemy;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isAttacking = false;

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
                    isAttacking = true;
                    StartCoroutine(AutoAttackCoroutine());
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
            if (targetEnemy == null)
            {
                isAttacking = false;
                yield break;
            }

            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            if (distance > attackRange)
            {
                isMoving = true;
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

        GameObject projectile = Instantiate(autoAttackPrefab, attackSpawnPoint.position, Quaternion.identity);
        HomingProjectile homingScript = projectile.GetComponent<HomingProjectile>();

        if (homingScript != null)
        {
            homingScript.SetTarget(targetEnemy);
        }
        else
        {
            Debug.LogError("HomingProjectile script missing on autoAttackPrefab!");
        }
    }


    void StopAutoAttack()
    {
        isAttacking = false;
        StopCoroutine(AutoAttackCoroutine());
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
