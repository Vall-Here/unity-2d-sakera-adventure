using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Node;

public class RangedEnemyAIController : EnemyAIController
{
    protected override void Awake()
    {
        enemyType = EnemyType.Ranged;
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

            // Buat behavior tree nodes spesifik untuk ranged
            BTNode patrolNode = new ActionNode(Patrol);
            BTNode chaseNode = new ActionNode(ChasePlayer);
            BTNode rangedAttackNode = new ActionNode(RangedAttack);
            BTNode retreatNode = new ActionNode(Retreat);

            List<BTNode> selectorNodes = new List<BTNode>();

            switch (currentState)
            {
                case EnemyState.Retreat:
                    selectorNodes.Add(retreatNode);
                    break;
                case EnemyState.Chase when distanceToPlayer > attackRange && distanceToPlayer <= detectRange:
                    selectorNodes.Add(rangedAttackNode);
                    selectorNodes.Add(chaseNode);
                    break;
                case EnemyState.Attack when distanceToPlayer <= attackRange:
                    selectorNodes.Add(patrolNode); // Ranged enemy mungkin tidak memiliki attack langsung
                    break;
                default:
                    selectorNodes.Add(patrolNode);
                    break;
            }

            SelectorNode selector = new SelectorNode(selectorNodes);
            bool executed = selector.Execute();

            yield return null;
        }
    }
}
