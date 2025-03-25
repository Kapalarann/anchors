using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    public bool isInvulnerable = false;

    public event Action<float, float> OnHealthChanged; // Event to notify health changes

    private void Awake()
    {
        HP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        OnHealthChanged?.Invoke(HP, maxHP); // Notify listeners (BerserkPassive)

        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
