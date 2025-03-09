using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public GameObject spellPrefab;

    [SerializeField]
    private MonoBehaviour[] effectComponents; 

    private ISpellEffect[] spellEffects; 
    private void Awake()
    {
        Debug.Log($"Spell {name} is being instantiated...");
    }

    private void Start()
    {
        Debug.Log($"Spell {name} is initializing effects in Start()...");

        if (effectComponents == null || effectComponents.Length == 0)
        {
            Debug.LogError($"Spell {name}: No effects assigned in Inspector!");
            return;
        }

        spellEffects = new ISpellEffect[effectComponents.Length];

        for (int i = 0; i < effectComponents.Length; i++)
        {
            spellEffects[i] = effectComponents[i] as ISpellEffect;
            if (spellEffects[i] == null)
            {
                Debug.LogError($"Effect at index {i} on {name} is NOT an ISpellEffect! Check Inspector.");
            }
            else
            {
                Debug.Log($"Spell {name} initialized effect in Start(): {spellEffects[i].GetType().Name}");
            }
        }
    }

    protected void ApplyEffects(GameObject target)
    {
        Debug.Log($"ApplyEffects() called on {name} at {Time.time} seconds");

        if (spellEffects == null || spellEffects.Length == 0)
        {
            Debug.LogError($"Spell {name}: spellEffects is NULL or EMPTY when calling ApplyEffects()!");
            return;
        }

        foreach (var effect in spellEffects)
        {
            if (effect != null)
            {
                Debug.Log($"Applying {effect.GetType().Name} to {target.name}");
                effect.ApplyEffect(target);
            }
            else
            {
                Debug.LogError($"Spell {name} has a NULL effect in spellEffects array.");
            }
        }
    }


    public abstract void Cast(Vector3 targetPosition);
}
