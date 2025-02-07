using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Node;

public class MeleeEnemyAIController : EnemyAIController
{
    protected override void Awake()
    {
        enemyType = EnemyType.Melee;
        base.Awake();
    }

    protected override IEnumerator BehaviorCycle()
    {
        while (true)
        {
            if (playerTransform == null || enemyHealth.IsDead())
            {
                currentState = EnemyState.Idle;
                Debug.Log($"{enemyType} Enemy: Idle");
                yield return new WaitForSeconds(1f);
                continue;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            float currentHealth = enemyHealth.GetCurrentHealth();
            float maxHealth = enemyHealth.GetMaxHealth();

            currentState = fuzzyLogic.DecideState(
                distanceToPlayer, 
                currentHealth, 
                maxHealth, 
                detectRange, 
                retreatThreshold, 
                enemyType, 
                attackRange, 
                ref previousState
            );

            Debug.Log($"{enemyType} Enemy: Current State = {currentState}");

            // Buat behavior tree nodes spesifik untuk melee
            BTNode patrolNode = new ActionNode(Patrol);
            BTNode chaseNode = new ActionNode(ChasePlayer);
            BTNode attackNode = new ActionNode(AttackPlayer);
            BTNode retreatNode = new ActionNode(Retreat);

            List<BTNode> selectorNodes = new List<BTNode>();

            switch (currentState)
            {
                case EnemyState.Attack when distanceToPlayer <= attackRange:
                    selectorNodes.Add(attackNode); // Prioritas tertinggi
                    break;
                case EnemyState.Chase when distanceToPlayer > attackRange && distanceToPlayer <= detectRange:
                    selectorNodes.Add(chaseNode); // Prioritas kedua
                    break;
                case EnemyState.Retreat:
                    selectorNodes.Add(retreatNode);
                    break;
                default:
                    selectorNodes.Add(patrolNode); // Prioritas terakhir
                    break;
            }

            SelectorNode selector = new SelectorNode(selectorNodes);
            bool executed = selector.Execute();

            yield return null;
        }
    }
}
