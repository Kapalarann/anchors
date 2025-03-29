using UnityEngine;

public class QProjectile : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"QSpell hit {other.name}, dealing {damage} damage!");
            other.GetComponent<EnemyStats>()?.TakeDamage(damage);
            Destroy(gameObject); 
        }
    }
}
