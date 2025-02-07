using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private float DamageRecoveryTime = 0.5f;

    private Slider healthSlider;
    private Image healthFillImage;
    public int currentHealth = 0;
    private bool canTakeDamage = true;
    private KnockBack knockBack;
    private Flash flash;
    const string HEALTH_SLIDER_TEXT = "Health Slider";

    private GameController gameController;
    private IngameUi ingameUi;

    protected override void Awake()
    {
        base.Awake();
        knockBack = GetComponent<KnockBack>();
        flash = GetComponent<Flash>();
        gameController = GameController.Instance;
        ingameUi = IngameUi.Instance;
    }

    private void Start()
    {
        if (currentHealth == 0){
        currentHealth = maxHealth;
        }
        if (healthSlider != null && healthFillImage != null){
            UpdateHealthSlider();
        }
        else{
            // Debug.LogError("PlayerHealth: Health Slider or Fill Image is not assigned!");
        }
    }
    public void SetHealth(int health)
    {
        currentHealth = health;
        UpdateHealthSlider();
    }

    public void InitializeReferences(Slider slider, Image fillImage)
    {
        healthSlider = slider;
        healthFillImage = fillImage;    
        UpdateHealthSlider(); 

        if (healthSlider != null && healthFillImage != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            UpdateHealthFillColor();
            // Debug.Log("PlayerHealth: References initialized successsfully.");
        }
        else
        {
            // Debug.LogError("PlayerHealth: Failed to initialize healthSlider or healthFillImage!");
        }
    }

    public void TakeDamage(int damageAmount, Transform attackerTransform, float knockbackThrustAmount)
    {
        if (canTakeDamage)
        {
            ScreenShakeManager.Instance.ShakeScreen();
            knockBack.GetKnockedBack(attackerTransform, knockbackThrustAmount);
            canTakeDamage = false;
            currentHealth -= damageAmount;
            StartCoroutine(flash.FlashRoutine());
            StartCoroutine(RecoveryFromDamage());
            UpdateHealthSlider();
            CheckIfDead();
        }
    }

    private IEnumerator RecoveryFromDamage()
    {
        yield return new WaitForSeconds(DamageRecoveryTime);
        canTakeDamage = true;
    }

    public void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider == null)
        {
            // Debug.LogError("PlayerHealth: Health Slider is not assigned!");
            return;
        }

        healthSlider.value = currentHealth;
        UpdateHealthFillColor();
    }

    private void UpdateHealthFillColor()
    {
        if (healthFillImage != null && healthSlider != null)
        {
            healthFillImage.color = Color.Lerp(Color.red, Color.green, healthSlider.value / maxHealth);
        }
    }

    private void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Player is Dead");
            gameController.SaveGameManually();
            Destroy(gameObject);
            ingameUi.gameObject.SetActive(false);
            SceneManager.LoadScene("Deadscreen");
        }
    }
}
