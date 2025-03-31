using UnityEngine;
using System.Collections;

public class BurnEffect : MonoBehaviour, ISpellEffect
{
    public float burnDamagePerSecond = 5f;
    public float burnDuration = 4f;

    public void ApplyEffect(GameObject target)
    {
        HealthAndStamina enemy = target.GetComponent<HealthAndStamina>();
        if (enemy != null)
        {
            Debug.Log($"🔥 {target.name} is burning for {burnDuration} seconds!");
            enemy.StartCoroutine(ApplyBurn(enemy));
        }
        else
        {
            Debug.LogWarning($"⚠️ {target.name} does not have EnemyStats! Burn effect skipped.");
        }
    }

    private IEnumerator ApplyBurn(HealthAndStamina enemy)
    {
        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {
            enemy.TakeDamage(burnDamagePerSecond);
            Debug.Log($"{enemy.name} takes {burnDamagePerSecond} burn damage.");
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        Debug.Log($"{enemy.name}'s burn effect ended.");
    }
}
