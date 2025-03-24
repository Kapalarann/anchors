using UnityEngine;
using UnityEngine.UI;

public class SpellCooldownUI : MonoBehaviour
{
    public Image[] cooldownImages; // Assign in Inspector
    private float[] cooldownTimers; // Tracks remaining cooldowns
    private float[] cooldownDurations; // Stores max cooldown durations

    public SpellCaster spellCaster; // Reference to SpellCaster to get spell cooldowns

    private void Start()
    {
        int spellCount = cooldownImages.Length;
        cooldownTimers = new float[spellCount];
        cooldownDurations = new float[spellCount];

        // Initialize cooldowns based on SpellCaster's spells
        for (int i = 0; i < spellCount; i++)
        {
            if (spellCaster.spells[i] != null)
            {
                cooldownDurations[i] = spellCaster.spells[i].cooldownTime;
                cooldownTimers[i] = cooldownDurations[i]; // Start on full cooldown
                cooldownImages[i].fillAmount = 1; // UI shows full cooldown at start
            }
            else
            {
                cooldownImages[i].fillAmount = 0; // No spell, no cooldown UI
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;
                cooldownImages[i].fillAmount = cooldownTimers[i] / cooldownDurations[i]; // Smooth cooldown
            }
            else
            {
                cooldownImages[i].fillAmount = 0; // Hide UI when ready
            }
        }
    }

    public void StartCooldown(int spellIndex, float cooldownTime)
    {
        if (spellIndex < 0 || spellIndex >= cooldownTimers.Length) return; // Prevent errors

        cooldownTimers[spellIndex] = cooldownTime;
        cooldownDurations[spellIndex] = cooldownTime;
        cooldownImages[spellIndex].fillAmount = 1; // Reset cooldown UI
    }
}
