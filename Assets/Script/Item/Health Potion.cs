
using UnityEngine;

public class HealthPotion : MonoBehaviour, IUsable 
{
    [SerializeField] private int healAmount = 5;

    public void Use(){
        for(int i = 0; i <= healAmount; i++){
            PlayerController.Instance.GetComponent<PlayerHealth>().HealPlayer();
            Debug.Log("Healing Player with potion");
        }
        // Destroy(gameObject);
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
