using UnityEngine;

public class ESpell : Spell
{
    public float radius = 5f; 
    public float maxRange = 10f;

    public override void Cast(Vector3 _)
    {
        
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        playerPosition.y = 0.01f; 

        
        GameObject spellEffect = Instantiate(spellPrefab, playerPosition, Quaternion.Euler(-90, 0, 0));

        
        Collider[] affectedEnemies = Physics.OverlapSphere(playerPosition, radius);

        foreach (Collider col in affectedEnemies)
        {
            ApplyEffects(col.gameObject);
        }

        Debug.Log($"ESpell cast at {playerPosition} affecting {affectedEnemies.Length} enemies.");

        
        Destroy(spellEffect, 2f);
    }
}
