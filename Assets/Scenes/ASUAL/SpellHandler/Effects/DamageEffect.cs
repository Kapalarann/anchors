using UnityEngine;

public class DamageEffect : MonoBehaviour, ISpellEffect
{
    public float damageAmount = 10f;

    public void ApplyEffect(GameObject target)
    {
        EnemyStats enemy = target.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
            Debug.Log($"{target.name} took {damageAmount} damage from {gameObject.name}!");
        }
    }
}
