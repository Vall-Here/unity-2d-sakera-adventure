using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryCanvasController : Singleton<InventoryCanvasController>
{
    public GameObject inventoryUI;
    private bool isInventoryOpen = false;
    private PlayerControls playerInput;
    

    protected override void Awake()
    {// Create InputAction object
        base.Awake();
        playerInput = new PlayerControls();
    }

    private void Start() {
        playerInput.UI.ToggleInventory.performed += OnToggleInventory;
    }

    private void OnEnable()
    {
        playerInput.UI.Enable();
    }

    private void OnDisable()
    {
        playerInput.UI.Disable(); // Disable the InputAction map for UI
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen); 
        
        if (isInventoryOpen)
        {
            ActiveWeapon.Instance.DisableControls();
        }
        else
        {
            ActiveWeapon.Instance.EnableControls();
        }
    }
}
