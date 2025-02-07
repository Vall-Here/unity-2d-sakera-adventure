using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : Singleton<Stamina>
{
    public int currentStamina { get; private set; }

    [SerializeField] private Sprite fullStaminaSprite, emptyStaminaSprite;
    [SerializeField] private int timeBetweenStaminaRegen = 3;

    [SerializeField] private Transform staminaContainer;
    private int startingStamina = 4;
    private int maxStamina = 4;
    const string STAMINA_CONTAINER_TEXT = "Stamina Container";

    protected override void Awake()
    {
        base.Awake();
        maxStamina = startingStamina;
        currentStamina = startingStamina;
    }

    private void Start()
    {
        if (staminaContainer != null)
        {
            UpdateStaminaUI(); // Initialize UI
            StartCoroutine(RegenStaminaRoutine());
        }
        else
        {
            Debug.LogError("Stamina: Stamina Container not assigned!");
        }
    }

    // Metode untuk mengatur referensi secara dinamis
    public void SetStaminaContainer(Transform container)
    {
        staminaContainer = container;
        if (staminaContainer != null)
        {
            UpdateStaminaUI();
            StartCoroutine(RegenStaminaRoutine());
            Debug.Log("Stamina: Stamina Container set successfully.");
        }
        else
        {
            Debug.LogError("Stamina: Provided Stamina Container is null!");
        }
    }

    public void addStamina(int amount){
        if (currentStamina < maxStamina) {
            currentStamina += amount;
            UpdateStaminaUI();
        }else {
            Debug.LogWarning("Stamina: Full stamina!");
        }
    }

    public void UseStamina()
    {
        if (currentStamina > 0)
        {
            currentStamina--;
            UpdateStaminaUI();
        }
        else
        {
            Debug.LogWarning("Stamina: No stamina left!");
        }
    }

    public void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina++;
            UpdateStaminaUI();
        }
    }

    private IEnumerator RegenStaminaRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenStaminaRegen);
            RegenStamina();
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaContainer == null)
        {
            Debug.LogError("Stamina: staminaContainer is null!");
            return;
        }

        for (int i = 0; i < maxStamina; i++)
        {
            if (i < currentStamina)
            {
                Image staminaImage = staminaContainer.GetChild(i).GetComponent<Image>();
                if (staminaImage != null)
                {
                    staminaImage.sprite = fullStaminaSprite;
                }
                else
                {
                    Debug.LogError($"Stamina: Image component not found on child {i}.");
                }
            }
            else
            {
                Image staminaImage = staminaContainer.GetChild(i).GetComponent<Image>();
                if (staminaImage != null)
                {
                    staminaImage.sprite = emptyStaminaSprite;
                }
                else
                {
                    Debug.LogError($"Stamina: Image component not found on child {i}.");
                }
            }
        }

        if (currentStamina < maxStamina)
        {
            StopAllCoroutines();
            StartCoroutine(RegenStaminaRoutine());
        }
    }
}
