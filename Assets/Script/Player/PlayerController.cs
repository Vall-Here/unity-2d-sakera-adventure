using System.Collections;
using Inventory;
using Inventory.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft {get {return facingLeft;}  set {facingLeft = value;}}
    [SerializeField] private float  dashSpeed = 5.0f;
    [SerializeField] private TrailRenderer dashTrail;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Transform weaponCollider;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 respawnPosition;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private bool facingLeft = false;
    private bool isDashing = false;
    AudioSource audioSource;
    private DayNightCycle dayNightCycle;

    private ActiveWeapon activeWeapon;
    private string weaponType;
    private KnockBack knockBack;
    public AudioClip dash;
    public AudioClip dead;
    public AudioClip walk;


    private UIManager uiManager;
    private InventoryController inventoryController;
    private Stamina stamina;
    private PlayerHealth playerHealth;



    protected override void Awake() 
    {
        base.Awake();
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        activeWeapon = GetComponentInChildren<ActiveWeapon>();
        knockBack = GetComponent<KnockBack>();
        audioSource = GetComponent<AudioSource>();

        dayNightCycle = FindObjectOfType<DayNightCycle>();
        
    }


    private void Start()
    {
        // LoadCheckpoint(); 
        playerControls.Combat.Dash.performed += _ => Dash();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }


    private void OnDisable() {
        playerControls.Disable();
    }


    private void Update()
    {
        PlayerInput();

        if (activeWeapon.hasWeapon)
        {
        
            weaponType = ActiveWeapon.Instance.GetWeaponType();

            if (weaponType == "Range")
            {
                myAnimator.SetBool("hasWeaponRange", true);
                myAnimator.SetBool("hasWeapon", false);
            }
            else
            {
                myAnimator.SetBool("hasWeaponRange", false);
                myAnimator.SetBool("hasWeapon", true);
            }
        }
        else
        {
            myAnimator.SetBool("hasWeapon", false);
            myAnimator.SetBool("hasWeaponRange", false);
        }
    }

    public void InitializeReferences(UIManager manager)
    {
        uiManager = manager;

        if (uiManager != null)
        {
            Debug.Log("PlayerController: UIManager references initialized successfully.");
            // Jika ada referensi lain ya
            inventoryController = GetComponent<InventoryController>();
            stamina = GetComponent<Stamina>();
            playerHealth = GetComponent<PlayerHealth>();

            if (inventoryController == null)
            {
                Debug.LogError("PlayerController: InventoryController component not found on player prefab!");
            }

            if (stamina == null)
            {
                Debug.LogError("PlayerController: Stamina component not found on player prefab!");
            }

            if (playerHealth == null)
            {
                Debug.LogError("PlayerController: PlayerHealth component not found on player prefab!");
            }

            // Anda bisa melakukan inisialisasi tambahan di sini jika diperlukan
        }
        else
        {
            Debug.LogError("PlayerController: Provided UIManager is null!");
        }
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move(); 
    }
    public Transform getWeaponCollider(){
        return weaponCollider;
    }
    private void PlayerInput()
    {
        moveInput = playerControls.Movement.Move.ReadValue<Vector2>();
        myAnimator.SetFloat("MoveX", moveInput.x);
        myAnimator.SetFloat("MoveY", moveInput.y);
    }

    private void Move()
    {
        if (knockBack.gettingKnockedBack){return;}
        rb.MovePosition(rb.position + moveInput * (speed * Time.fixedDeltaTime));

        if (moveInput.magnitude > 0.1f && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(walk);
        }
    }

    private void AdjustPlayerFacingDirection(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        if(mousePos.x < playerScreenPoint.x){
            mySpriteRenderer.flipX = true;
            FacingLeft = true;
        }
        else{
            mySpriteRenderer.flipX = false;
            FacingLeft = false;
        }
    }

    private void Dash(){
        if (!isDashing && Stamina.Instance.currentStamina > 0)
        {
            audioSource.PlayOneShot(dash);
            Stamina.Instance.UseStamina();
            myAnimator.SetTrigger("Dash");
            isDashing = true;
            speed *= dashSpeed;
            dashTrail.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = 0.2f;
        float dashCD = 0.25f;
        yield return new WaitForSeconds(dashTime);
        speed /= dashSpeed;
        dashTrail.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    public void TriggerAttackAnimation()
    {
        myAnimator.SetTrigger("Attack");
    }


    private void LoadCheckpoint()
    {
        if (PlayerPrefs.HasKey("CheckpointX") && PlayerPrefs.HasKey("CheckpointY"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            respawnPosition = new Vector2(x, y);

            string savedScene = PlayerPrefs.GetString("CurrentScene");
            if (savedScene != SceneManager.GetActiveScene().name)
            {
                respawnPosition = new Vector2(0, 0); 
            }
        }
        else
        {
            respawnPosition = new Vector2(0, 0); 
        }
        
        transform.position = respawnPosition; 
    }


    public DayNightCycle GetDayNightCycle()
    {
        return dayNightCycle;
    }

}
