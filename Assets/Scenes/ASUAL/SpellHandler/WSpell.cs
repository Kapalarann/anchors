using UnityEngine;

public class WSpell : Spell
{
    public float slowPercent = 0.5f;
    public float slowDuration = 2f;
    public float radius = 5f;
    public float maxRange = 10f;

    public override void Cast(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > maxRange)
        {
            Debug.Log("W Spell target out of range!");
            return;
        }

        // Instantiate spell prefab at target position with a fixed rotation of (90,0,0)
        if (spellPrefab != null)
        {
            GameObject spellEffect = Instantiate(spellPrefab, targetPosition, Quaternion.Euler(90, 0, 0));
            Destroy(spellEffect, 2f);
        }

        // Apply slow effect to enemies in range
        Collider[] affectedEnemies = Physics.OverlapSphere(targetPosition, radius);
        foreach (Collider col in affectedEnemies)
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.ApplySlow(slowPercent, slowDuration);
            }
        }

        Debug.Log("W spell cast at: " + targetPosition);
    }
}
