using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;
    public float baseSpeed = 5f;
    private float currentSpeed;
    private bool isStunned = false;

    // Track active effects
    private List<string> activeEffects = new List<string>();

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
        if (!activeEffects.Contains("Slow"))
        {
            activeEffects.Add("Slow");
            Debug.Log($"Enemy {gameObject.name} got Slowed for {duration} seconds.");
        }
        StartCoroutine(SlowCoroutine(slowPercent, duration));
    }

    private IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        currentSpeed = baseSpeed * (1f - slowPercent);
        yield return new WaitForSeconds(duration);
        ResetSpeed();
        activeEffects.Remove("Slow");
        Debug.Log($"Slow effect on {gameObject.name} expired.");
    }

    public void ApplyBurn(float damagePerSecond, float duration)
    {
        if (!activeEffects.Contains("Burn"))
        {
            activeEffects.Add("Burn");
            Debug.Log($"Enemy {gameObject.name} got Burned for {duration} seconds.");
            StartCoroutine(BurnCoroutine(damagePerSecond, duration));
        }
    }

    private IEnumerator BurnCoroutine(float damagePerSecond, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            TakeDamage(damagePerSecond * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        activeEffects.Remove("Burn");
        Debug.Log($"Burn effect on {gameObject.name} expired.");
    }

    public void ApplyStun(float duration)
    {
        if (!isStunned)
        {
            activeEffects.Add("Stun");
            Debug.Log($"Enemy {gameObject.name} got Stunned for {duration} seconds.");
            StartCoroutine(StunCoroutine(duration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        activeEffects.Remove("Stun");
        Debug.Log($"Stun effect on {gameObject.name} expired.");
    }

    public void ResetSpeed()
    {
        currentSpeed = baseSpeed;
    }

    public bool IsInsideSpellArea(Vector3 spellPosition, float radius)
    {
        return Vector3.Distance(transform.position, spellPosition) <= radius;
    }

    public void PrintActiveEffects()
    {
        if (activeEffects.Count > 0)
        {
            Debug.Log($"Active Effects on {gameObject.name}: " + string.Join(", ", activeEffects));
        }
        else
        {
            Debug.Log($"No active effects on {gameObject.name}.");
        }
    }

    private void Die()
    {
        Debug.Log($"Enemy {gameObject.name} died.");
        Destroy(gameObject);
    }
}
