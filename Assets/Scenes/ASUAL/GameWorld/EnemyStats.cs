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

    public void ApplySlow(float slowPercent, float duration)
    {
        StartCoroutine(SlowCoroutine(slowPercent, duration));
    }

    private IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        currentSpeed = baseSpeed * (1f - slowPercent); // Reduce speed
        yield return new WaitForSeconds(duration);
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        currentSpeed = baseSpeed; // Restore normal speed
    }

    public bool IsInsideSpellArea(Vector3 spellPosition, float radius)
    {
        return Vector3.Distance(transform.position, spellPosition) <= radius;
    }

    public void ApplyStun(float duration)
    {
        if (!isStunned) StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private void Die()
    {
        Destroy(gameObject); // Replace with your custom death logic
    }
}
