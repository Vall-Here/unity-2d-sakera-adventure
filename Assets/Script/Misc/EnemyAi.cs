// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemyAi : MonoBehaviour
// {
//     private enum State {
//         Idle,
//         Roaming,
//         Attack
//     }

//     private State state;
//     private EnemyPathFinding enemyPathFinding;

//     private Vector2 lastPosition;
//     private SpriteRenderer spriteRenderer;
//     private Animator myAnimator;

//     private Vector2 moveDirection;

//     private Transform playerTransform; 
//     [SerializeField] private float detectRange = 5f; 
//     [SerializeField] private float attackRange = 1.5f; 
//     [SerializeField] private float attackDamage = 10f; 
//     [SerializeField] private float attackCooldown = 1f; 
//     [SerializeField] private float knockbackThrustAmount = 10f; 
//     [SerializeField] private float idleDuration = 5f; 
//     [SerializeField] private float roamingDuration = 5f;
//     private bool canAttack = true;

//     [SerializeField] private AudioClip attackSFX; 
//     private AudioSource audioSource; 
//     private Shooter shooter;

//     private enum EnemyType {
//         Melee,
//         Ranged
//     }

//     [SerializeField] private EnemyType enemyType; 

//     private void Awake() {
//         enemyPathFinding = GetComponent<EnemyPathFinding>();
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         myAnimator = GetComponent<Animator>();
//         audioSource = GetComponent<AudioSource>(); 
//         shooter = GetComponent<Shooter>(); 
//         state = State.Idle; 
//     }

//     private void Start() {
//         StartCoroutine(StateCycleRoutine());
//         StartCoroutine(FindPlayer()); 
//     }

//     private void Update() {
//         if (state == State.Roaming || state == State.Attack) {
//             moveDirection = (Vector2)transform.position - lastPosition;

//             myAnimator.SetFloat("MoveX", moveDirection.x);
//             myAnimator.SetFloat("MoveY", moveDirection.y);

//             if (moveDirection.x < 0) {
//                 spriteRenderer.flipX = true;
//             } else if (moveDirection.x > 0) {
//                 spriteRenderer.flipX = false;
//             }

//             lastPosition = transform.position;
//         }

//         if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange) {
//             state = State.Attack;
//         } else if (Vector2.Distance(transform.position, playerTransform.position) <= detectRange) {
//             state = State.Roaming;
//         }
//     }

//     private void FixedUpdate() {
//         switch (state) {
//             case State.Roaming:
//                 RoamTowardsPlayer();
//                 break;
//             case State.Attack:
//                 if (canAttack) {
//                     StartCoroutine(AttackPlayer());
//                 }
//                 break;
//         }
//     }

//     private void RoamTowardsPlayer() {
//         if (Vector2.Distance(transform.position, playerTransform.position) <= detectRange) {
//             enemyPathFinding.MoveTo(playerTransform.position);
//         }
//     }



//         private IEnumerator AttackPlayer() {
//                 canAttack = false;
//                 enemyPathFinding.Stop();
//                 myAnimator.SetTrigger("Attack");
                
//                 if (audioSource != null && attackSFX != null) {
//                     audioSource.PlayOneShot(attackSFX);
//                 }
//                 if (enemyType == EnemyType.Ranged) {
//                     if (shooter != null) {
//                         shooter.Attack();
//                     }
//                 } else if (enemyType == EnemyType.Melee) {
//                     yield return new WaitForSeconds(0.5f);
//                     PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
//                     if (playerHealth != null) {
//                         playerHealth.TakeDamage((int)attackDamage, this.transform, knockbackThrustAmount);
//                     }
//                 }

//                 yield return new WaitForSeconds(attackCooldown);
//                 canAttack = true;
//             }


//     private IEnumerator StateCycleRoutine() {
//         while (true) {
//             state = State.Idle;
//             enemyPathFinding.Stop(); 
//             yield return new WaitForSeconds(idleDuration); 
//             state = State.Roaming;
//             StartCoroutine(RoamingRoutine());
//             yield return new WaitForSeconds(roamingDuration); 
//         }
//     }

//     private IEnumerator RoamingRoutine() {
//         while (state == State.Roaming) {
//             Vector2 roamPosition = GetRoamingPosition();
//             enemyPathFinding.MoveTo(roamPosition);
//             yield return new WaitForSeconds(Random.Range(1f, 3f)); 
//         }
//     }

//     private Vector2 GetRoamingPosition() {
//         return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
//     }

//     private IEnumerator FindPlayer() {
//         while (playerTransform == null) {
//             if (PlayerController.Instance != null) {
//                 playerTransform = PlayerController.Instance.transform; 
//             } else {
//                 PlayerController playerController = FindObjectOfType<PlayerController>();
//                 if (playerController != null) {
//                     playerTransform = playerController.transform;
//                 }
//             }
//             yield return new WaitForSeconds(1f); 
//         }
//     }
// }



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
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

    private Transform playerTransform; 
    [SerializeField] private float detectRange = 5f; 
    [SerializeField] private float attackRange = 1.5f; 
    [SerializeField] private float attackDamage = 10f; 
    [SerializeField] private float attackCooldown = 1f; 
    [SerializeField] private float knockbackThrustAmount = 10f; 
    [SerializeField] private float idleDuration = 5f; 
    [SerializeField] private float roamingDuration = 5f;
    private bool canAttack = true;

    [SerializeField] private AudioClip attackSFX; 
    private AudioSource audioSource; 
    private Shooter shooter;

    private enum EnemyType {
        Melee,
        Ranged
    }

    [SerializeField] private EnemyType enemyType; 

    private void Awake() {
        enemyPathFinding = GetComponent<EnemyPathFinding>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
        shooter = GetComponent<Shooter>(); 
        state = State.Idle; 
    }

    private void OnEnable() {
        if (GameController.Instance != null) {
            GameController.Instance.OnPlayerInstantiated += HandlePlayerInstantiated;
            
            if (PlayerController.Instance != null) {
                HandlePlayerInstantiated(PlayerController.Instance.gameObject);
            }
        }
        else {
        }
    }

    private void OnDisable() {
        if (GameController.Instance != null) {
            GameController.Instance.OnPlayerInstantiated -= HandlePlayerInstantiated;
        }
    }

    /// <summary>
    /// Handler untuk event OnPlayerInstantiated
    /// </summary>
    /// <param name="player">Player yang diinstansiasi</param>
    private void HandlePlayerInstantiated(GameObject player)
    {
        if (player != null)
        {
            playerTransform = player.transform;

            StartCoroutine(StateCycleRoutine());
        }

    }

    private void Update() {
        if (playerTransform == null) {
            return;
        }

        if (state == State.Roaming || state == State.Attack) {
            moveDirection = ((Vector2)transform.position - (Vector2)lastPosition).normalized;

            myAnimator.SetFloat("MoveX", moveDirection.x);
            myAnimator.SetFloat("MoveY", moveDirection.y);

            if (moveDirection.x < 0) {
                spriteRenderer.flipX = true;
            } else if (moveDirection.x > 0) {
                spriteRenderer.flipX = false;
            }

            lastPosition = transform.position;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange) {
            if (state != State.Attack) {
                state = State.Attack;
            }
        } else if (distanceToPlayer <= detectRange) {
            if (state != State.Roaming) {
                state = State.Roaming;
            }
        } else {
            if (state != State.Idle) {
                state = State.Idle;
            }
        }
    }

    private void FixedUpdate() {
        if (playerTransform == null) {
            return;
        }

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
            enemyPathFinding.MoveTo(playerTransform.position);
        }
    }

    private IEnumerator AttackPlayer() {
        canAttack = false;
        enemyPathFinding.Stop();
        myAnimator.SetTrigger("Attack");

        if (audioSource != null && attackSFX != null) {
            audioSource.PlayOneShot(attackSFX);
        }

        if (enemyType == EnemyType.Ranged) {
            if (shooter != null) {
                shooter.Attack();
            }
        } else if (enemyType == EnemyType.Melee) {
            yield return new WaitForSeconds(0.5f);
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage((int)attackDamage, this.transform, knockbackThrustAmount);
            }
            else {
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator StateCycleRoutine() {
        while (true) {
            state = State.Idle;
            enemyPathFinding.Stop(); 
            // Debug.Log($"{gameObject.name}: State diatur ke Idle.");
            yield return new WaitForSeconds(idleDuration); 

            state = State.Roaming;
            StartCoroutine(RoamingRoutine());
            // Debug.Log($"{gameObject.name}: State diatur ke Roaming.");
            yield return new WaitForSeconds(roamingDuration); 
        }
    }

    private IEnumerator RoamingRoutine() {
        while (state == State.Roaming) {
            Vector2 roamPosition = GetRoamingPosition();
            enemyPathFinding.MoveTo(roamPosition);
            // Debug.Log($"{gameObject.name}: Roaming menuju posisi random: {roamPosition}.");
            yield return new WaitForSeconds(Random.Range(1f, 3f)); 
        }
    }

    private Vector2 GetRoamingPosition() {
        return (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
    }
}
