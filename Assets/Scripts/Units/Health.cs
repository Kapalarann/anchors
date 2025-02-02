using UnityEngine;

public enum HealthType
{
    unit,
    structure,
    resource
}

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    [System.NonSerialized] public bool isInvulnerable = false;

    [SerializeField] public HealthType type;
    private void Awake()
    {
        HP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;
        //Show damage number

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        if (HP <= 0)
        {
            Die();
            return;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
