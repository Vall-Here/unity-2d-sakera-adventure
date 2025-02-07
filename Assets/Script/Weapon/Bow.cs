
using UnityEngine;
using Inventory.Model;
using TMPro;
using System.Collections;

public class Bow : MonoBehaviour, IWeapon
{
    public string WeaponType { get; private set; } = "Range";
    [SerializeField] private float distanceFromPlayer = 0.3f;  
    [SerializeField] private WeaponInfo weaponInfo;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private TMP_Text noArrowMessageText;

    readonly int FiRE_HASH = Animator.StringToHash("Fire");

    private Animator myAnimator;
    private EquipmentManager equipmentManager;
    

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        equipmentManager = EquipmentManager.Instance;
        if (equipmentManager == null)
        {
            Debug.LogError("Bow: EquipmentManager.Instance tidak ditemukan di scene!");
        }
    }
    private void LateUpdate()
    {
        if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            ApplyBowOffset();
        }
    }

    private void ApplyBowOffset() {
        transform.localPosition = new Vector3(distanceFromPlayer, 0f, 0f);
    }
    public void Attack()
    {
        // myAnimator.SetTrigger(FiRE_HASH);
        // GameObject newArrow = Instantiate(projectilePrefab, projectileSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        // Projectile projectile = newArrow.GetComponent<Projectile>();

        // projectile.UpdateProjectileRange(weaponInfo.weaponRange);
        // projectile.UpdateWeaponInfo(weaponInfo);

        EquipSlot arrowEquipSlot = equipmentManager.GetEquipSlotByType(ItemTypeEnum.SlotItemType.Projectile);

        if (arrowEquipSlot != null && !arrowEquipSlot.EquipItemSO.IsEmpty && arrowEquipSlot.EquipItemSO.Quantity > 0)
        {
            arrowEquipSlot.UnequipItem();
            myAnimator.SetTrigger(FiRE_HASH);
            GameObject newArrow = Instantiate(projectilePrefab, projectileSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
            Projectile projectile = newArrow.GetComponent<Projectile>();

            projectile.UpdateProjectileRange(weaponInfo.weaponRange);
            projectile.UpdateWeaponInfo(weaponInfo);
        }
        else
        {
            if (noArrowMessageText != null)
            {
                StartCoroutine(ShowNoArrowMessage());
            }
        }
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    private IEnumerator ShowNoArrowMessage()
    {
        noArrowMessageText.text = "Tidak ada panah!";
        noArrowMessageText.enabled = true;
        yield return new WaitForSeconds(2f); 
        noArrowMessageText.enabled = false;
    }
}
