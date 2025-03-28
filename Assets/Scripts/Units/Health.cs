using UnityEngine;
using System;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    public bool isInvulnerable = false;

    private GameObject healthBarObj;
    private HealthBar healthBar;

    public event Action<float, float> OnHealthChanged; // Event to notify health changes

    private void Awake()
    {
        HP = maxHP;
    }

    private void Start()
    {
        // Get a health bar from the pool
        healthBarObj = HealthBarPool.Instance.GetHealthBar();
        if (healthBarObj == null) return;
        healthBar = healthBarObj.GetComponent<HealthBar>();
        if (healthBar == null) return;

        // Attach health bar to this enemy
        healthBarObj.transform.SetParent(UIManager.Instance.HealthBarContainer);
        healthBar.Initialize(this);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);
        if(healthBar != null) healthBar.UpdateFill(HP, maxHP);

        OnHealthChanged?.Invoke(HP, maxHP); // Notify listeners (BerserkPassive)

        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        HealthBarPool.Instance.ReturnHealthBar(healthBarObj);
        Destroy(gameObject);
    }
}
