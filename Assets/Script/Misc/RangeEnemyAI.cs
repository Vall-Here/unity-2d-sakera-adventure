using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyAI : MonoBehaviour
{
    private enum State {
        Idle,
        Roaming,
        Attack
    }

    private State state;
    private EnemyPathFinding enemyPathFinding;

    private Vector2 lastPosition;
    private SpriteRenderer spriteRenderer;
    private Animator myAnimator;

    private Vector2 moveDirection;

    [SerializeField] private Transform playerTransform; // Referensi ke player
    [SerializeField] private float detectRange = 5f; // Jarak deteksi player
    [SerializeField] private float attackRange = 1.5f; // Jarak serangan
    [SerializeField] private float attackDamage = 10f; // Damage yang diberikan ke player
    [SerializeField] private float attackCooldown = 1f; // Waktu cooldown serangan
    [SerializeField] private float knockbackThrustAmount = 10f; 
    [SerializeField] private float idleDuration = 5f; // Durasi idle
    [SerializeField] private float roamingDuration = 5f; // Durasi roaming
    private bool canAttack = true;

    private void Awake() {
        enemyPathFinding = GetComponent<EnemyPathFinding>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        state = State.Idle; // Set awalnya ke Idle
    }

    private void Start() {
        StartCoroutine(StateCycleRoutine()); // Memulai siklus idle dan roaming
    }

    private void Update() {
        if (state == State.Roaming || state == State.Attack) {
            moveDirection = (Vector2)transform.position - lastPosition;

            // Update animator dengan moveX dan moveY
            myAnimator.SetFloat("MoveX", moveDirection.x);
            myAnimator.SetFloat("MoveY", moveDirection.y);

            // Flip sprite berdasarkan arah
            if (moveDirection.x < 0) {
                spriteRenderer.flipX = true;
            } else if (moveDirection.x > 0) {
                spriteRenderer.flipX = false;
            }

            // Simpan posisi terakhir
            lastPosition = transform.position;
        }

        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange) {
            state = State.Attack;
        } else if (Vector2.Distance(transform.position, playerTransform.position) <= detectRange) {
            state = State.Roaming;
        }
    }

    private void FixedUpdate() {
        switch (state) {
            case State.Roaming:
                RoamTowardsPlayer();
                break;
            case State.Attack:
                if (canAttack) {
                    StartCoroutine(AttackPlayer());
                }
                break;
        }
    }

    private void RoamTowardsPlayer() {
        if (Vector2.Distance(transform.position, playerTransform.position) <= detectRange) {
            // Jika player dalam jarak deteksi, kejar player
            enemyPathFinding.MoveTo(playerTransform.position);
        }
    }

    private IEnumerator AttackPlayer() {
        canAttack = false;
        enemyPathFinding.Stop(); // Hentikan pergerakan saat menyerang
        myAnimator.SetTrigger("Attack"); // Play animasi serangan

        yield return new WaitForSeconds(0.5f);

        // Berikan damage ke player
        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null) {
            playerHealth.TakeDamage((int)attackDamage, this.transform, knockbackThrustAmount);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator StateCycleRoutine() {
        while (true) {
            state = State.Idle;
            enemyPathFinding.Stop(); 
            yield return new WaitForSeconds(idleDuration); 

            state = State.Roaming;
            StartCoroutine(RoamingRoutine());
            yield return new WaitForSeconds(roamingDuration); 

        }
    }

    private IEnumerator RoamingRoutine() {
        while (state == State.Roaming) {
            Vector2 roamPosition = GetRoamingPosition();
            enemyPathFinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(Random.Range(1f, 3f)); // Interval perubahan posisi roaming
        }
    }

    private Vector2 GetRoamingPosition() {
        // Generate posisi roaming baru secara acak
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
