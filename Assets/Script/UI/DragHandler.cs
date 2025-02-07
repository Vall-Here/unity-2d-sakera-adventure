using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private InputActionAsset inputActions;

    private InputAction pointerAction;

    private void Awake()
    {
        inputActions = new InputActionAsset();
        pointerAction = inputActions.FindActionMap("UI").FindAction("Pointer");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        // Logika untuk memulai drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        // Logika untuk menggerakkan item mengikuti mouse
        transform.position = Pointer.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        // Logika untuk menghentikan drag
    }
}
