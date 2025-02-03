using UnityEngine;
using System.Collections;

public class WSpell : Spell
{
    public float duration = 5f;
    public float damagePerSecond = 5f;
    public float slowPercentage = 0.5f;
    public float tickRate = 1f;

    public override void Cast(Vector3 target)
    {
        transform.position = target;
        StartCoroutine(DestroyAfterTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                StartCoroutine(ApplyEffects(enemy));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyStats>()?.ResetSpeed();
        }
    }

    private IEnumerator ApplyEffects(EnemyStats enemy)
    {
        enemy.ApplySlow(slowPercentage);

        while (enemy != null && enemy.IsInsideSpellArea(this))
        {
            enemy.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(tickRate);
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
