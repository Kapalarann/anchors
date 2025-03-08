using UnityEngine;

public class StunEffect : MonoBehaviour, ISpellEffect
{
    public float stunDuration = 2f;

    public void ApplyEffect(GameObject target)
    {
        EnemyStats enemy = target.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.ApplyStun(stunDuration);
            Debug.Log($"{target.name} is stunned for {stunDuration} seconds!");
        }
    }
}
