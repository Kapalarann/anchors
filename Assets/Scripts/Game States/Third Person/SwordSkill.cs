using UnityEngine;
using UnityEngine.InputSystem;

public class SwordSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private ParticleSystem skillEffectPrefab; // Particle effect prefab
    [SerializeField] private Transform swordTransform; // Reference to the sword object
    [SerializeField] private float cooldown = 5f; // Cooldown time in seconds
    [SerializeField] private float skillDuration = 5f; // Duration of the skill in seconds

    private bool isCooldown = false;
    private Animator _animator;
    private PlayerAttack _playerAttack;
    private ParticleSystem activeParticleEffect;

    private static readonly int SwordArtHash = Animator.StringToHash("SwordArt"); // Animator trigger for the skill

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    public void SwordArt_Event(InputAction.CallbackContext context)
    {
        if (context.performed && !isCooldown)
        {
            ActivateSkill();
        }
    }

    private void ActivateSkill()
    {
        Debug.Log("SwordArt skill activated!");

        // Trigger animation
        _animator.SetTrigger(SwordArtHash);

        // Play particle effect and attach it to the sword
        if (skillEffectPrefab != null && swordTransform != null)
        {
            activeParticleEffect = Instantiate(skillEffectPrefab, swordTransform.position, swordTransform.rotation, swordTransform);
            activeParticleEffect.Play();
        }

        // Increase damage values in PlayerAttack
        if (_playerAttack != null)
        {
            _playerAttack.ModifyDamage(1.1f); // Increase damage by 10%
        }

        // Start cooldown and skill duration timers
        isCooldown = true;
        Invoke(nameof(ResetCooldown), cooldown);
        Invoke(nameof(EndSkill), skillDuration);
    }

    private void ResetCooldown()
    {
        isCooldown = false;
        Debug.Log("SwordArt skill cooldown reset.");
    }

    private void EndSkill()
    {
        Debug.Log("SwordArt skill ended.");

        // Revert damage values in PlayerAttack
        if (_playerAttack != null)
        {
            _playerAttack.ResetDamage(); // Revert to base damage
        }

        // Stop and destroy particle effect
        if (activeParticleEffect != null)
        {
            activeParticleEffect.Stop();
            Destroy(activeParticleEffect.gameObject, 1f); // Delay to allow particles to fade out
        }
    }
}
