using UnityEngine;

public class QSpell : Spell
{
    public float speed = 20f;
    public float damage = 10f;
    private Vector3 direction;
    private bool isFired = false;

    public override void Cast(Vector3 target)
    {
        direction = (target - transform.position).normalized;
        isFired = true;
    }

    private void Update()
    {
        if (isFired)
        {
            transform.position += direction * speed * Time.deltaTime;
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
