using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 3f;
    private HealthAndStamina _health;

    private void Start()
    {
        _health = GetComponent<HealthAndStamina>();
        if (_health != null)
        {
            InvokeRepeating(nameof(ApplyDamage), 1f, 1f);
        }
    }

    private void ApplyDamage()
    {
        if (_health != null)
        {
            _health.TakeDamage(damagePerSecond);
        }
    }
}
