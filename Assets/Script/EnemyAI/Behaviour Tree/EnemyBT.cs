// using UnityEngine;

// public class EnemyBehaviorTree : MonoBehaviour
// {
//     public enum State { Patrol, Chase, Attack, Retreat }
//     public State currentState;

//     private EnemyPatrol patrolBehavior;
//     private EnemyChase chaseBehavior;
//     private EnemyAttack attackBehavior;
//     private EnemyRetreat retreatBehavior;
//     private EnemyFuzzyLogic fuzzyLogic;

//     public Transform player;
//     public EnemyHealth enemyHealth;
//     public float attackRange = 10f;
//     public float retreatHealthThreshold = 20f;

//     void Start()
//     {
//         // Initialize behaviors
//         patrolBehavior = GetComponent<EnemyPatrol>();
//         chaseBehavior = GetComponent<EnemyChase>();
//         attackBehavior = GetComponent<EnemyAttack>();
//         retreatBehavior = GetComponent<EnemyRetreat>();
//         fuzzyLogic = new EnemyFuzzyLogic();
//         enemyHealth = GetComponent<EnemyHealth>();

//         currentState = State.Patrol;
//     }

//     void Update()
//     {
//         // Hitung jarak ke pemain
//         float distanceToPlayer = Vector3.Distance(transform.position, player.position);

//         // Ambil persentase kesehatan
//         float healthPercentage = enemyHealth.GetHealthPercentage();

//         // Gunakan logika fuzzy untuk menentukan status saat ini
//         currentState = fuzzyLogic.DecideState(distanceToPlayer, healthPercentage, attackRange, retreatHealthThreshold);

//         // Eksekusi perilaku berdasarkan status
//         switch (currentState)
//         {
//             case State.Patrol:
//                 patrolBehavior.Patrol();
//                 break;
//             case State.Chase:
//                 chaseBehavior.Chase(player);
//                 break;
//             case State.Attack:
//                 attackBehavior.Attack(player);
//                 break;
//             case State.Retreat:
//                 retreatBehavior.Retreat();
//                 break;
//         }
//     }
// }
