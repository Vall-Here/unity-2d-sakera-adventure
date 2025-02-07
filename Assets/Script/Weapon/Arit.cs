using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arit : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject SlashAnimPrefab;
    [SerializeField] private Transform SlashSP;
    
    // [SerializeField] private float weaponAttackCD = .5f;
    private Animator myAnimator;
    private GameObject SlashAnim;
    private Transform weaponCollider;
    public string WeaponType { get; private set; } = "Melee";

    

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        weaponCollider = PlayerController.Instance.getWeaponCollider();
        SlashSP = GameObject.Find("SlashSP").transform;
    }
    private void Update()
    {
        MouseFollowWithOffset();
    }
    public void Attack()
    {
            myAnimator.SetTrigger("Attack");
            PlayerController.Instance.TriggerAttackAnimation();

            weaponCollider.gameObject.SetActive(true); 
            SlashAnim = Instantiate(SlashAnimPrefab, SlashSP.position, Quaternion.identity);
            SlashAnim.transform.SetParent(SlashSP);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }


    private void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent()
    {
        SlashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            SlashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        SlashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            SlashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }


    private void MouseFollowWithOffset(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle - 45); 
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, angle - 45);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle - 45); 
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, angle - 45);
        }
    }

}
