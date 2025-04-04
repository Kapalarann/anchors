using UnityEngine;
using System.Collections;

public class BerserkPassive : MonoBehaviour
{
    private HealthAndStamina _health;
    private MeleeMovement _movement;
    private Animator _animator;

    [Header("Berserk Settings")]
    [SerializeField] private float _berserkThreshold = 0.5f; // 50% HP
    [SerializeField] private float _speedMultiplier = 1.5f;
    [SerializeField] private float _sizeMultiplier = 1.2f; // Character grows in size
    [SerializeField] private float _animationSpeedMultiplier = 1.5f;
    [SerializeField] private float _invulnerabilityDuration = 5f; // Temporary invulnerability
    [SerializeField] private float _berserkDuration = 10f; // How long Berserk mode lasts
    [SerializeField] private float _berserkCooldown = 15f; // Cooldown time before Berserk can trigger again

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem _berserkEffect; // Particle System for Berserk Mode

    private bool _isBerserk = false;
    private bool _isOnCooldown = false;
    private Vector3 _originalScale;

    private void Start()
    {
        _health = GetComponent<HealthAndStamina>();
        _movement = GetComponent<MeleeMovement>();
        _animator = GetComponent<Animator>();

        if (_health != null)
        {
            _health.OnHealthChanged += CheckBerserkMode;
        }

        _originalScale = transform.localScale; // Store original size
    }

    private void CheckBerserkMode(float currentHP, float maxHP)
    {
        if (currentHP / maxHP <= _berserkThreshold && !_isBerserk && !_isOnCooldown)
        {
            ActivateBerserk();
        }
    }

    private void ActivateBerserk()
    {
        _isBerserk = true;
        _isOnCooldown = true;
        _health.isInvulnerable = true;
        _movement.SetSpeedMultiplier(_speedMultiplier);
        _animator.speed *= _animationSpeedMultiplier;
        transform.localScale = _originalScale * _sizeMultiplier;

        if (_berserkEffect != null)
        {
            ParticleSystem effects = Instantiate(_berserkEffect, transform);
        }

        StartCoroutine(BerserkTimer());
        StartCoroutine(EndBerserkInvulnerability());
        StartCoroutine(BerserkCooldownTimer());
    }

    private IEnumerator BerserkTimer()
    {
        yield return new WaitForSeconds(_berserkDuration);
        DeactivateBerserk();
    }

    private IEnumerator EndBerserkInvulnerability()
    {
        yield return new WaitForSeconds(_invulnerabilityDuration);
        _health.isInvulnerable = false;
    }

    private void DeactivateBerserk()
    {
        _isBerserk = false;
        _movement.SetSpeedMultiplier(1f);
        _animator.speed /= _animationSpeedMultiplier;
        transform.localScale = _originalScale; // Reset to normal size

        if (_berserkEffect != null)
        {
            _berserkEffect.Stop(); // Stop Berserk Particle Effect
        }
    }

    private IEnumerator BerserkCooldownTimer()
    {
        yield return new WaitForSeconds(_berserkCooldown);
        _isOnCooldown = false;
    }
}
