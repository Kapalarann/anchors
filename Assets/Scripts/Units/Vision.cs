using UnityEngine;

public class Vision : MonoBehaviour
{
    public float visionRange = 10f; // Vision radius
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    private void Update()
    {
        DetectEnemies();
    }

    void DetectEnemies()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, visionRange, enemyLayer);

        foreach (var enemyCollider in enemiesInRange)
        {
            GameObject enemy = enemyCollider.gameObject;
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // Check for obstacles between the unit and the enemy
            if (!Physics.Raycast(transform.position, directionToEnemy, distanceToEnemy, obstacleLayer))
            {
                enemy.GetComponent<EnemyVisibility>()?.AddViewer(gameObject);
            }
            else
            {
                enemy.GetComponent<EnemyVisibility>()?.RemoveViewer(gameObject);
            }
        }
    }
}
