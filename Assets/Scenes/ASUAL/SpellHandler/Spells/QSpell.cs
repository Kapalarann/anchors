using UnityEngine;

public class QSpell : Spell
{
    public float speed = 20f;
    public float maxRange = 10f; 
    public GameObject projectilePrefab; 

    public override void Cast(Vector3 targetPosition)
    {
        Vector3 playerPosition = caster.transform.position;
        
        if (Vector3.Distance(playerPosition, targetPosition) > maxRange)
        {
            Debug.Log("target out of range!");
            return;
        }

        targetPosition.y += 1f;
        Vector3 direction = (targetPosition - caster.attackSpawnPoint.position).normalized;
        
        GameObject projectile = Instantiate(projectilePrefab, caster.attackSpawnPoint.position, Quaternion.identity);
        projectile.GetComponent<QProjectile>().caster = caster.gameObject;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            Debug.LogError("QSpell projectile is missing a Rigidbody!");
        }

        Debug.Log($"QSpell cast towards {targetPosition}, projectile spawned at {playerPosition}");
    }
}
