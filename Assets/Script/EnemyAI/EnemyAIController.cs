using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Node;

[RequireComponent(typeof(EnemyHealth), typeof(Animator))]
public abstract class EnemyAIController : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged
    }

    [Header("Enemy Type")]
    [SerializeField] protected EnemyType enemyType;

    [Header("References")]
    protected GridManager gridManager;
    protected EnemyHealth enemyHealth;
    protected Animator myAnimator;
    protected Transform playerTransform;
    protected SpriteRenderer spriteRenderer;
    protected Shooter shooter; 

    [Header("Settings")]
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected float detectRange = 10f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float retreatThreshold = 0.3f;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float waitTime = 2f;
    [SerializeField] protected Transform bulletPoint;

    [SerializeField] protected EnemyState currentState;
    protected EnemyState previousState = EnemyState.Patrol;

    protected List<Vector2> currentPath = null;
    protected float lastAttackTime = 0f;
    protected int currentPathIndex = 0;
    protected bool isWaiting = false;
    protected int currentPatrolIndex = 0;
    protected Vector2 moveDirection;
    protected EnemyFuzzyLogic fuzzyLogic;
    protected Coroutine currentPathCoroutine;

    protected HashSet<GameObject> attackedEntities = new HashSet<GameObject>();

    protected virtual void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridManager = FindObjectOfType<GridManager>();
        fuzzyLogic = new EnemyFuzzyLogic();

        if (enemyType == EnemyType.Ranged)
        {
            shooter = GetComponent<Shooter>();
            bulletPoint = shooter.bulletSpawnPoint;
        }

        AlignPatrolPointsToGrid();
    }

    protected virtual void Start()
    {
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
        StartCoroutine(BehaviorCycle());
    }

    protected virtual void Update()
    {
        UpdateMovement();
        UpdateAnimation();
    }

    protected void AlignPatrolPointsToGrid()
    {
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPoints[i].position = (Vector2)gridManager.AlignPositionToGrid(patrolPoints[i].position);
        }
    }

    protected void UpdateMovement()
    {
        moveDirection = patrolPoints[currentPatrolIndex].position - transform.position;
        moveDirection.Normalize(); 

        if (currentState == EnemyState.Chase && playerTransform != null)
        {
            float directionToPlayer = playerTransform.position.x - transform.position.x;

            if (directionToPlayer > 0)
            {
                spriteRenderer.flipX = false;
                if (enemyType == EnemyType.Ranged) {
                    bulletPoint.localPosition = new Vector2(0.5f, 0); 
                }
                myAnimator.SetBool("isFacingLeft", false);
            }
            else if (directionToPlayer < 0)
            {
                spriteRenderer.flipX = true;
                if (enemyType == EnemyType.Ranged) {
                    bulletPoint.localPosition = new Vector2(-0.5f, 0);  
                }
                myAnimator.SetBool("isFacingLeft", true);
            }
        }
        else
        {
            MoveToTarget(patrolPoints[currentPatrolIndex].position);
        }

        myAnimator.SetFloat("MoveX", moveDirection.x);
        myAnimator.SetFloat("MoveY", moveDirection.y);
    }

    protected void MoveToTarget(Vector2 targetPosition)
    {
        float scaleFactor = transform.localScale.x;  
        Vector2 adjustedTarget = targetPosition / scaleFactor;  

        transform.position = Vector2.MoveTowards(transform.position, adjustedTarget, moveSpeed * Time.deltaTime);
    }

    protected void UpdateAnimation()
    {
        myAnimator.SetFloat("MoveX", moveDirection.x);
        myAnimator.SetFloat("MoveY", moveDirection.y);

        if (currentState == EnemyState.Chase && playerTransform != null)
        {
            float directionToPlayer = playerTransform.position.x - transform.position.x;

            if (directionToPlayer > 0)
            {
                spriteRenderer.flipX = false;  // Menghadap kanan
                myAnimator.SetBool("isFacingLeft", false);
            }
            else if (directionToPlayer < 0)
            {
                spriteRenderer.flipX = true;  // Menghadap kiri
                myAnimator.SetBool("isFacingLeft", true);
            }
        }
        else
        {
            if (moveDirection.x < 0)
            {
                spriteRenderer.flipX = true;
                myAnimator.SetBool("isFacingLeft", true);
            }
            else if (moveDirection.x > 0)
            {
                spriteRenderer.flipX = false;
                myAnimator.SetBool("isFacingLeft", false);
            }
        }
    }

    protected abstract IEnumerator BehaviorCycle();

    protected void Patrol()
    {
        if (isWaiting || patrolPoints.Length == 0) return;

        Vector2 target = patrolPoints[currentPatrolIndex].position;
        float distanceToPatrolPoint = Vector2.Distance(transform.position, target);

        if (distanceToPatrolPoint < 0.1f)
        {
            if (!isWaiting)
            {
                StartCoroutine(WaitAtPatrolPoint());
            }
            return;
        }

        StartPathfindingTo(target);
    }

    protected void ChasePlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectRange || distanceToPlayer <= attackRange) return;

        StartPathfindingTo(playerTransform.position);
    }

    protected void AttackPlayer()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > attackRange)
        {
            return;
        }

        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            myAnimator.SetTrigger("Attack");
            attackedEntities.Clear();
        }
    }

    protected void RangedAttack()
    {
        if (shooter != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= detectRange && distanceToPlayer > attackRange)
            {
                if (Time.time > lastAttackTime + attackCooldown){
                    lastAttackTime = Time.time;
                    shooter.Attack();
                    attackedEntities.Clear();
                } 
            }
        }
    }

    public void AttackNotify()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance <= attackRange)
        {
            if (!attackedEntities.Contains(playerTransform.gameObject))
            {
                PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // playerHealth.TakeDamage(attackDamage, this.transform, distance); 
                    attackedEntities.Add(playerTransform.gameObject);
                }
            }
        }
    }

    protected void Retreat()
    {
        if (playerTransform == null) return;

        Vector2 retreatDirection = (transform.position - playerTransform.position).normalized;
        Vector2 retreatPosition = (Vector2)transform.position + retreatDirection * detectRange;
        StartPathfindingTo(retreatPosition);
    }

    protected void StartPathfindingTo(Vector2 targetPosition)
    {
        currentPath = gridManager.FindPath(transform.position, targetPosition);
        currentPathIndex = 0;

        if (currentPath == null || currentPath.Count == 0)
        {
            return;
        }

        if (currentPathCoroutine != null)
        {
            StopCoroutine(currentPathCoroutine);
        }
        currentPathCoroutine = StartCoroutine(FollowPath());
    }

    protected IEnumerator FollowPath()
    {
        while (currentPath != null && currentPathIndex < currentPath.Count)
        {
            Vector2 targetPosition = currentPath[currentPathIndex];

            if (Vector2.Distance(transform.position, targetPosition) <= 0.2f) // Gunakan jarak kecil yang wajar
            {
                currentPathIndex++;  // Move to next path point
                continue;
            }

            if (!gridManager.IsWalkable(targetPosition))
            {
                yield break;
            }

            while (Vector2.Distance(transform.position, targetPosition) > 0.2f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;  // Move to next point in path
        }
    }

    protected IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        isWaiting = false;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (var patrolPoint in patrolPoints)
            {
                if (patrolPoint != null)
                {
                    Gizmos.DrawSphere(patrolPoint.position, 0.2f);
                }
            }
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
                Gizmos.DrawSphere(currentPath[i], 0.1f);
            }
            Gizmos.DrawSphere(currentPath[currentPath.Count - 1], 0.1f);
        }
    }
}
