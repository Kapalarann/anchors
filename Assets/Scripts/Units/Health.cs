using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.AI;

public class HealthAndStamina : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    public bool isInvulnerable = false;
    public bool invisibleHpBar = false;

    [Header("Stamina")]
    [SerializeField] public float maxStamina;
    [SerializeField] public float stamina;
    [SerializeField] public float staminaRegen;
    [HideInInspector] public bool isStunned = false;

    private GameObject healthBarObj;
    private HealthBar healthBar;
    private AnimationManager animationManager;

    public event Action<float, float> OnHealthChanged;

    [Header("UI Elements")]
    public Slider healthSlider;
    private Bleed bleed;

    [Header("Game Over Panel")]
    public GameOverPanel gameOverPanel; // Assign in Inspector

    private void Awake()
    {
        HP = maxHP;
        stamina = maxStamina;
    }

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = HP;
        }

        healthBarObj = HealthBarPool.Instance.GetHealthBar();
        if (healthBarObj == null) return;
        healthBar = healthBarObj.GetComponent<HealthBar>();
        if (healthBar == null) return;

        healthBarObj.transform.SetParent(UIManager.Instance.HealthBarContainer);
        healthBar.InitializeHP(this);

        bleed = Bleed.instance;

        animationManager = GetComponent<AnimationManager>();
    }

    private void Update()
    {
        if (stamina >= maxStamina || isStunned) return;
        stamina += staminaRegen * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        if (healthBar != null) healthBar.UpdateStamina(stamina, maxStamina);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);
        if (GameStateManager.Instance.currentUnit != this.gameObject) ConsumeStamina(damage);

        animationManager.Flinch(isStunned);
        GetComponent<Animator>().SetTrigger("onHit");

        if (healthBar != null) healthBar.UpdateHP(HP, maxHP);

        if (healthSlider != null) healthSlider.value = HP;

        OnHealthChanged?.Invoke(HP, maxHP);

        if (bleed != null && GameStateManager.Instance.currentUnit == this.gameObject) bleed.ShowBleed();

        if (HP <= 0)
        {
            Die();
        }
    }

    public void ConsumeStamina(float staminaCost)
    {
        stamina -= staminaCost;
        if(stamina < 0) isStunned = true;
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);

        if (healthBar != null) healthBar.UpdateStamina(stamina, maxStamina);
    }

    public void StopStun()
    {
        animationManager._animator.SetBool("isStunned", false);
        OnStunFinish();
    }

    public void OnStunFinish()
    {
        isStunned = false;
        stamina = maxStamina;
        if (healthBar != null) healthBar.UpdateStamina(stamina, maxStamina);
    }

    private void Die()
    {
        HealthBarPool.Instance.ReturnHealthBar(healthBarObj);

        // Show Game Over panel instead of destroying the player
        if (gameOverPanel != null && GameStateManager.Instance.currentUnit == this.gameObject)
        {
            gameOverPanel.ShowGameOverPanel();
        }

        // Hide the player instead of destroying (optional)
        gameObject.SetActive(false);
    }
}
