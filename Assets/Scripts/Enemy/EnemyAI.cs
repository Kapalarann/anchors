using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float idleMoveRadius = 5f;

    [Header("Death Settings")]
    [SerializeField] private ParticleSystem deathParticles;

    private GameObject target;
    private Vector3 initialPosition;
    private Vector3 idleTargetPosition;
    private bool isDead = false;
    private bool isWandering = false;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("EnemyAI requires a Rigidbody component!");
        }

        initialPosition = transform.position;
        ChooseNewIdleTarget(); // Set the first idle target position
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        // Look for targets within detection range
        DetectPlayer();

        if (target != null)
        {
            FollowTarget();
        }
        else
        {
            Wander();
        }
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Unit") && hit.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                target = hit.gameObject;
                return;
            }
        }

        target = null; // Reset target if no player is detected
    }

    private void FollowTarget()
    {
        // Move towards the player
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.fixedDeltaTime;

        // Apply movement using Rigidbody
        _rigidbody.MovePosition(transform.position + movement);

        // Rotate to face the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f));
    }

    private void Wander()
    {
        if (!isWandering)
        {
            Vector3 direction = (idleTargetPosition - transform.position).normalized;
            Vector3 movement = direction * (moveSpeed / 2) * Time.fixedDeltaTime;

            _rigidbody.MovePosition(transform.position + movement);

            // Rotate to face the idle target position
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 2f));
            }

            // If the enemy has reached the idle target, choose a new target
            if (Vector3.Distance(transform.position, idleTargetPosition) < 0.5f)
            {
                isWandering = true;
                Invoke(nameof(ChooseNewIdleTarget), 2f); // Wait before wandering again
            }
        }
    }

    private void ChooseNewIdleTarget()
    {
        // Pick a random position within the idle move radius
        Vector2 randomPoint = Random.insideUnitCircle * idleMoveRadius;
        idleTargetPosition = initialPosition + new Vector3(randomPoint.x, 0, randomPoint.y);
        isWandering = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"{name} took {damage} damage. Remaining health: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{name} died!");

        // Play death particles
        if (deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Remove the enemy from the scene
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection range in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw the idle move radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(initialPosition, idleMoveRadius);
    }
}
