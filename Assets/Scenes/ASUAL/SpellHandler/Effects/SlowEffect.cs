using UnityEngine;

public class SlowEffect : MonoBehaviour, ISpellEffect
{
    public float slowPercent = 0.5f;
    public float slowDuration = 3f;

    public void ApplyEffect(GameObject target)
    {
        EnemyStats enemy = target.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.ApplySlow(slowPercent, slowDuration);
            Debug.Log($"{target.name} is slowed by {slowPercent * 100}% for {slowDuration} seconds!");
        }
    }
}
