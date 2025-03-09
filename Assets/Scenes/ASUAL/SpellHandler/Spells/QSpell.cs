using UnityEngine;

public class QSpell : Spell
{
    public float speed = 20f;
    public float maxRange = 10f; 
    public GameObject projectilePrefab; 

    public override void Cast(Vector3 targetPosition)
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        
        if (Vector3.Distance(playerPosition, targetPosition) > maxRange)
        {
            Debug.Log("QSpell target out of range!");
            return;
        }

        
        Vector3 direction = (targetPosition - playerPosition).normalized;

        
        GameObject projectile = Instantiate(projectilePrefab, playerPosition, Quaternion.identity);
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
