using UnityEngine;

public class ESpell : Spell
{
    public float stunDuration = 2f;
    public float damage = 20f;
    public float radius = 5f;

    public override void Cast(Vector3 position)
    {
        
        if (spellPrefab != null)
        {
            GameObject spellEffect = Instantiate(spellPrefab, position, Quaternion.Euler(90, 0, 0));
            spellEffect.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

            SphereCollider sphereCollider = spellEffect.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = spellEffect.AddComponent<SphereCollider>();
            }
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;

            Destroy(spellEffect, 2f);
        }

        
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.ApplyStun(stunDuration);
                    enemyStats.TakeDamage(damage);
                }
            }
        }

        Debug.Log("E spell cast, stunned enemies in range.");
    }
}
