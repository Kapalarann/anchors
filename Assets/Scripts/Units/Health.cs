using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP;
    [SerializeField] public float HP;
    public bool isInvulnerable = false;

    private GameObject healthBarObj;
    private HealthBar healthBar;
    private AnimationManager animationManager;
    private NavMeshAgent agent;

    public event Action<float, float> OnHealthChanged;

    [Header("UI Elements")]
    public Slider healthSlider;
    private Bleed bleed;

    [Header("Game Over Panel")]
    public GameOverPanel gameOverPanel; // Assign in Inspector

    private void Awake()
    {
        HP = maxHP;
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
        healthBar.Initialize(this);

        bleed = Bleed.instance;

        animationManager = GetComponent<AnimationManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);
        if(healthBar != null) healthBar.UpdateFill(HP, maxHP);
        
        animationManager.Flinch();
        GetComponent<Animator>().SetTrigger("onHit");

        if (healthBar != null)
            healthBar.UpdateFill(HP, maxHP);

        if (healthSlider != null)
            healthSlider.value = HP;

        OnHealthChanged?.Invoke(HP, maxHP);

        if (bleed != null && GameStateManager.Instance.currentAgent == agent) bleed.ShowBleed();

        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        HealthBarPool.Instance.ReturnHealthBar(healthBarObj);

        // Show Game Over panel instead of destroying the player
        if (gameOverPanel != null)
        {
            gameOverPanel.ShowGameOverPanel();
        }

        // Hide the player instead of destroying (optional)
        gameObject.SetActive(false);
    }
}
