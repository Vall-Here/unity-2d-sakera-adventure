using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10; // Pastikan ini sesuai dengan kebutuhan Anda
    [SerializeField] private float deathDelay = 1f;

    [Header("References")]
    private EnemyPathFinding enemyPathFinding;
    private KnockBack knockBack;
    private Flash flash;
    private Animator animator;
    
    [Header("Status")]
    public int currentHealth;
    private bool isDead = false;

    // Event untuk memberitahukan sistem lain bahwa musuh mati
    public delegate void OnDeathHandler(EnemyHealth enemyHealth);
    public event OnDeathHandler OnDeath;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockBack = GetComponent<KnockBack>();
        animator = GetComponent<Animator>();
        enemyPathFinding = GetComponent<EnemyPathFinding>();
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Terima damage dari sumber eksternal.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Knockback dan efek flash
        knockBack.GetKnockedBack(PlayerController.Instance.transform, 5f);
        StartCoroutine(flash.FlashRoutine());

        // Periksa kematian
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Animasi kematian
        animator.SetBool("isDead", true);

        // Hentikan semua perilaku musuh
        if (enemyPathFinding != null)
            enemyPathFinding.Stop();

        // Beri tahu listener bahwa musuh telah mati
        OnDeath?.Invoke(this);

        // Mulai sequence penghancuran
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathDelay);

        // Spawn item drop jika ada komponen yang relevan
        var dropSpawner = GetComponent<EnemyDropSpawner>();
        if (dropSpawner != null)
        {
            dropSpawner.DropItems();
        }

        // Hancurkan game object
        Destroy(gameObject);
    }

    /// <summary>
    /// Kesehatan dalam bentuk persentase (0 - 1).
    /// </summary>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// Apakah musuh sudah mati?
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
}
