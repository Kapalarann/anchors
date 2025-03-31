using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public GameObject spellPrefab;

    [SerializeField]
    private MonoBehaviour[] effectComponents;

    public List<ISpellEffect> spellEffects = new List<ISpellEffect>();

    [Header("Spell Settings")]
    public float cooldownTime = 5f;

    public SpellCaster caster;

    private void Awake()
    {
        Debug.Log($"📜 Spell {name} (Instance ID: {GetInstanceID()}) is being instantiated...");
    }

    private void Start()
    {
        Debug.Log($"📜 Initializing spell: {name} (Instance ID: {GetInstanceID()})");

        if (effectComponents == null || effectComponents.Length == 0)
        {
            Debug.LogError($"❌ Spell {name}: effectComponents is NULL or EMPTY! Check Inspector.");
            return;
        }

        spellEffects.Clear();

        foreach (var component in effectComponents)
        {
            if (component == null)
            {
                Debug.LogError($"❌ Spell {name}: Found NULL component in effectComponents array.");
                continue;
            }

            ISpellEffect effect = component as ISpellEffect; 
            if (effect == null)
            {
                Debug.LogError($"❌ Spell {name}: Component {component.GetType().Name} does NOT implement ISpellEffect!");
            }
            else
            {
                spellEffects.Add(effect);
                Debug.Log($"✅ Spell {name} added effect: {component.GetType().Name}");
            }
        }

        Debug.Log($"🎯 Spell {name} now has {spellEffects.Count} valid effects.");
    }

    public void Initialize(SpellCaster caster)
    {
        this.caster = caster;
    }

    protected void ApplyEffects(GameObject target)
    {
        Debug.Log($"🔍 ApplyEffects() called on {name} (Instance ID: {GetInstanceID()}) for {target.name}");

        if (spellEffects.Count == 0)
        {
            Debug.LogError($"❌ Spell {name}: spellEffects list is EMPTY when calling ApplyEffects()! Check if Start() ran properly.");
            return;
        }

        foreach (var effect in spellEffects)
        {
            if (effect != null)
            {
                Debug.Log($"✅ Applying {effect.GetType().Name} to {target.name}");
                effect.ApplyEffect(target);
            }
            else
            {
                Debug.LogError($"❌ Spell {name} has a NULL effect in spellEffects list.");
            }
        }
    }

    public abstract void Cast(Vector3 targetPosition);
}
