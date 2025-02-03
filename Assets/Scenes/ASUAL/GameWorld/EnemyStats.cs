using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;
    public float baseSpeed = 5f;
    private float currentSpeed;
    private bool isStunned = false; 

    private void Start()
    {
        currentSpeed = baseSpeed;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public void ApplySlow(float slowPercent)
    {
        if (!isStunned) 
        {
            currentSpeed = baseSpeed * (1f - slowPercent);
        }
    }

    public void ResetSpeed()
    {
        if (!isStunned)
        {
            currentSpeed = baseSpeed;
        }
    }

    public bool IsInsideSpellArea(WSpell spell)
    {
        return Vector3.Distance(transform.position, spell.transform.position)
            <= spell.GetComponent<SphereCollider>().radius;
    }

    public void ApplyStun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        currentSpeed = 0f;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        currentSpeed = baseSpeed;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
