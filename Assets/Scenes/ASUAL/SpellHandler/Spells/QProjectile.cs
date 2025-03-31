using UnityEngine;

public class QProjectile : MonoBehaviour
{
    public float damage = 10f;
    public GameObject caster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body"))
        {
            if (other.transform.root.gameObject == caster) return;
            other.transform.root.GetComponent<HealthAndStamina>()?.TakeDamage(damage);
            Destroy(gameObject); 
        }
        if (other.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }
}
