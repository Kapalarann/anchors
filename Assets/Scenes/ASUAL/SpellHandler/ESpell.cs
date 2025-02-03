using UnityEngine;

public class ESpell : Spell
{
    public float radius = 5f;
    public float stunDuration = 2f;
    public float damage = 20f;

    public override void Cast(Vector3 targetPosition)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(targetPosition, radius);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(damage);
                    enemyStats.ApplyStun(stunDuration);

                }
            }
        }

        Destroy(gameObject, 0.5f); 
    }
}
