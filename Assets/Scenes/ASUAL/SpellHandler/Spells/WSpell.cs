using UnityEngine;

public class WSpell : Spell
{
    public float radius = 5f;
    public float maxRange = 10f;
    public float duration = 10f;

    public override void Cast(Vector3 targetPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) > maxRange)
        {
            Debug.Log("W Spell target out of range!");
            return;
        }

        
        GameObject spellObj = Instantiate(spellPrefab, targetPosition, Quaternion.Euler(90, 0, 0));

        
        Collider[] affectedEnemies = Physics.OverlapSphere(targetPosition, radius);
        foreach (Collider col in affectedEnemies)
        {
            ApplyEffects(col.gameObject);
        }

       
        Destroy(spellObj, duration);

        Debug.Log($"WSpell cast at {targetPosition}, will despawn in {duration} seconds.");
    }
}
