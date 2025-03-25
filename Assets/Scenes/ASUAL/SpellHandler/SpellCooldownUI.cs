using UnityEngine;
using UnityEngine.UI;

public class SpellCooldownUI : MonoBehaviour
{
    public cooldownUI[] cooldownUIs;

    private void Start()
    {
        
        for (int i = 0; i < cooldownUIs.Length; i++)
        {
            cooldownUIs[i].Images.fillAmount = 0; 
        }
    }

    private void Update()
    {
        for (int i = 0; i < cooldownUIs.Length; i++)
        {
            if (cooldownUIs[i].Timers > 0)
            {
                cooldownUIs[i].Timers -= Time.deltaTime;
                cooldownUIs[i].Images.fillAmount = cooldownUIs[i].Timers / cooldownUIs[i].Times;
            }
            else
            {
                cooldownUIs[i].Images.fillAmount = 0; 
            }
        }
    }

    public void StartCooldown(int spellIndex, float cooldownTime)
        cooldownUIs[spellIndex].Timers = cooldownUIs[spellIndex].Times;
        cooldownUIs[spellIndex].Images.fillAmount = 1; 
    }

    [System.Serializable] 
    public class cooldownUI
    {
        public Image Images;
        public float Times;
        public float Timers;
    }
}
