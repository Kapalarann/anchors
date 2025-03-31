using UnityEngine;
using System.Collections;
using Spells;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public float attackDamage = 5f;
    public GameObject homingPrefab;
    public Transform attackSpawnPoint;

    private Transform targetEnemy;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Animator animator;

    private Coroutine autoAttackCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
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
                if (hit.collider.CompareTag("Unit"))
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
                FaceTarget(targetEnemy.position);
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
            animator.SetFloat("Movementspeed", moveSpeed);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            FaceTarget(targetPosition);

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

            FaceTarget(targetEnemy.position);
            animator.SetTrigger("onCast1");
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void ShootAutoAttack()
    {
        if (targetEnemy == null) return;

        if (homingPrefab == null) return;

        GameObject projectile = Instantiate(homingPrefab, attackSpawnPoint.position, Quaternion.identity);
        HomingSpell homingScript = projectile.GetComponent<HomingSpell>();

        if (homingScript != null)
        {
            homingScript.SetTarget(targetEnemy);
            homingScript.damage = attackDamage;
        }
    }

    void StopAutoAttack()
    {
        isAttacking = false;

        if (autoAttackCoroutine != null)
        {
            StopCoroutine(autoAttackCoroutine);
            autoAttackCoroutine = null;
            animator.SetTrigger("onCastCancel");
        }
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
