using UnityEngine;
using Inventory.UI;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    [SerializeField]
    private Transform staminaContainer;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private Image healthFillImage;


    public Transform GetStaminaContainer()
    {
        return staminaContainer;
    }

    public Slider GetHealthSlider()
    {
        return healthSlider;
    }

    public Image GetHealthFillImage()
    {
        return healthFillImage;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jika ingin UIManager persist antar scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
