using UnityEngine;
using System.Collections;

public class BurnEffect : MonoBehaviour, ISpellEffect
{
    public float burnDamagePerSecond = 5f;
    public float burnDuration = 4f;

    public void ApplyEffect(GameObject target)
    {
        EnemyStats enemy = target.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.StartCoroutine(ApplyBurn(enemy));
        }
    }

    private IEnumerator ApplyBurn(EnemyStats enemy)
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
