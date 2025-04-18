﻿using UnityEngine;

public class DamageEffect : MonoBehaviour, ISpellEffect
{
    public float damageAmount = 10f;

    public void ApplyEffect(GameObject target)
    {
        HealthAndStamina enemy = target.GetComponent<HealthAndStamina>();
        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
            Debug.Log(target.name + " took " + damageAmount + " damage!");
        }
        else
        {
            Debug.LogWarning(target.name + " does not have EnemyStats! No damage applied.");
        }
    }
}
