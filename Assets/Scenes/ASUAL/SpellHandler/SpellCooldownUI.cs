using UnityEngine;
using UnityEngine.UI;

public class SpellCooldownUI : MonoBehaviour
{
    public Image[] cooldownImages; 
    private float[] cooldownTimes = { 5f, 7f, 10f, 15f }; 
    private float[] cooldownTimers = new float[4];

    private void Start()
    {
        
        for (int i = 0; i < cooldownImages.Length; i++)
        {
            cooldownImages[i].fillAmount = 0; 
        }
    }

    private void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;
                cooldownImages[i].fillAmount = cooldownTimers[i] / cooldownTimes[i];
            }
            else
            {
                cooldownImages[i].fillAmount = 0; 
            }
        }
    }

    public void StartCooldown(int spellIndex)
    {
        cooldownTimers[spellIndex] = cooldownTimes[spellIndex];
        cooldownImages[spellIndex].fillAmount = 1; 
    }
}
