using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    public bool isInvulnerable = false;

    private GameObject healthBarObj;
    private HealthBar healthBar;

    public event Action<float, float> OnHealthChanged;

    [Header("UI Elements")]
    public Slider healthSlider;
    public Image bleedOverlay; // Bleed effect overlay

    private Coroutine fadeCoroutine; // To manage fading effect

    private void Awake()
    {
        HP = maxHP;
    }

    private void Start()
    {
        // Initialize UI Slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = HP;
        }

        // Get a health bar from the pool
        healthBarObj = HealthBarPool.Instance.GetHealthBar();
        if (healthBarObj == null) return;
        healthBar = healthBarObj.GetComponent<HealthBar>();
        if (healthBar == null) return;

        // Attach health bar to this enemy
        healthBarObj.transform.SetParent(UIManager.Instance.HealthBarContainer);
        healthBar.Initialize(this);

        if (bleedOverlay != null)
            bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, 0);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);

        if (healthBar != null)
            healthBar.UpdateFill(HP, maxHP);

        if (healthSlider != null)
            healthSlider.value = HP;

        OnHealthChanged?.Invoke(HP, maxHP);

        //Bleed Effect
        if (bleedOverlay != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(ShowBleedEffect());
        }

        if (HP <= 0)
        {
            Die();
        }
    }

    private IEnumerator ShowBleedEffect()
    {
        float fadeDuration = 0.5f;
        float stayDuration = 0.3f;
        float alpha = 0.6f; // Blood effect intensity

        // Fade In
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float newAlpha = Mathf.Lerp(0, alpha, t / fadeDuration);
            bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, newAlpha);
            yield return null;
        }
        bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, alpha);

        // Stay visible
        yield return new WaitForSeconds(stayDuration);

        // Fade Out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float newAlpha = Mathf.Lerp(alpha, 0, t / fadeDuration);
            bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, newAlpha);
            yield return null;
        }
        bleedOverlay.color = new Color(bleedOverlay.color.r, bleedOverlay.color.g, bleedOverlay.color.b, 0);
    }

    private void Die()
    {
        HealthBarPool.Instance.ReturnHealthBar(healthBarObj);
        Destroy(gameObject);
    }
}
