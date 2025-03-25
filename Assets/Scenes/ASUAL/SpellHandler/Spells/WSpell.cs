using System.Collections;
using UnityEngine;

public class WSpell : Spell
{
    public float radius = 5f;
    public float duration = 10f;
    public float applyDelay = 0.1f;

    public override void Cast(Vector3 targetPosition)
    {
        Debug.Log($"🎯 {name} casting at {targetPosition}.");

        GameObject spellObj = Instantiate(spellPrefab, targetPosition, Quaternion.Euler(90, 0, 0));

        WSpell spellComponent = spellObj.GetComponent<WSpell>();
        if (spellComponent == null)
        {
            spellComponent = spellObj.AddComponent<WSpell>();
        }

        spellComponent.StartCoroutine(spellComponent.ApplyEffectsWithDelay());

        Debug.Log($"🕒 {name} effect lasts {duration} seconds.");
        Destroy(spellObj, duration);
    }

    private IEnumerator ApplyEffectsWithDelay()
    {
        yield return new WaitForSeconds(applyDelay);

        Collider[] affectedEnemies = Physics.OverlapSphere(transform.position, radius);
        Debug.Log($"🔍 {name} detecting enemies within radius {radius}: {affectedEnemies.Length}");

        foreach (Collider col in affectedEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                Debug.Log($"🟢 {name} detected enemy: {col.gameObject.name}, applying effects.");
                ApplyEffects(col.gameObject);
            }
            else
            {
                Debug.Log($"⚪ {name} ignored non-enemy: {col.gameObject.name}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"⚡ {name} hit {other.name}, applying effects!");
            StartCoroutine(DelayedEffectApplication(other.gameObject));
        }
    }

    private IEnumerator DelayedEffectApplication(GameObject enemy)
    {
        yield return new WaitForSeconds(applyDelay);
        ApplyEffects(enemy);
    }
}
