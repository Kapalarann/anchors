using UnityEngine;

public class QSpell : Spell
{
    public float speed = 20f;
    public float damage = 10f;
    private Vector3 targetPoint;

    public override void Cast(Vector3 target)
    {
        targetPoint = target;
    }

    private void Update()
    {
        if (targetPoint != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyStats>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
