using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPotion : MonoBehaviour, IUsable
{

    [SerializeField] private int StaminaAmount = 4;
    public void Use(){
        PlayerController.Instance.GetComponent<Stamina>().addStamina(StaminaAmount);
    }
    private void Update()
    {
        MouseFollowWithOffset();
    }

    private void MouseFollowWithOffset(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle - 45); 
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle - 45); 
        }
    }
}
