using UnityEngine;
public class EnemyFuzzyLogic
{
    private float previousFar = 0f;
    private float smoothingFactor = 0.8f; // Sesuaikan sesuai kebutuhan

    public EnemyState DecideState(
        float distanceToPlayer, 
        float currentHealth, 
        float maxHealth, 
        float detectRange, 
        float retreatHealthThreshold, 
        EnemyAIController.EnemyType enemyType, 
        float attackRange, 
        ref EnemyState previousState)
    {
        // Normalisasi health percentage
        float healthPercentage = currentHealth / maxHealth;

        // Fuzzifikasi jarak
        float near = FuzzifyTrapezoid(distanceToPlayer, 0, detectRange * 0.25f, detectRange * 0.5f, detectRange * 0.6f); 
        float medium = FuzzifyTrapezoid(distanceToPlayer, detectRange * 0.4f, detectRange * 0.5f, detectRange * 0.7f, detectRange * 0.85f);
        float far = FuzzifyTrapezoid(distanceToPlayer, detectRange * 0.75f, detectRange * 0.9f, detectRange, detectRange * 1.1f);

        if (enemyType == EnemyAIController.EnemyType.Melee)
        {
            near = FuzzifyTrapezoid(distanceToPlayer, 0, attackRange * 0.3f, attackRange * 0.7f, attackRange);
            medium = FuzzifyTrapezoid(distanceToPlayer, attackRange * 0.6f, detectRange * 0.4f, detectRange * 0.7f, detectRange);
            far = FuzzifyTrapezoid(distanceToPlayer, detectRange * 0.6f, detectRange * 0.8f, detectRange * 1.1f, detectRange * 1.3f);

            // Implementasi smoothing pada nilai 'far'
            far = SmoothValue(previousFar, far, smoothingFactor);
            previousFar = far;
        }

        float lowHealth = FuzzifyTrapezoid(healthPercentage, 0, 0, retreatHealthThreshold * 0.5f, retreatHealthThreshold);
        float mediumHealth = FuzzifyTrapezoid(healthPercentage, retreatHealthThreshold * 0.3f, retreatHealthThreshold, 0.6f, 0.8f);
        float highHealth = FuzzifyTrapezoid(healthPercentage, 0.7f, 0.8f, 1f, 1.1f);

        if (enemyType == EnemyAIController.EnemyType.Ranged)
        {
            Debug.Log($"Ranged enemy: near = {near}, medium = {medium}");

            // Keputusan untuk Ranged Enemy
            if (near > 0.7f)
            {
                Debug.Log("Ranged: Retreat");
                previousState = EnemyState.Retreat;
                return EnemyState.Retreat;
            }
            else if (medium > 0.4f)
            {
                Debug.Log("Ranged: Chase");
                previousState = EnemyState.Chase;
                return EnemyState.Chase;
            }
            else
            {
                Debug.Log("Ranged: Patrol");
                previousState = EnemyState.Patrol;
                return EnemyState.Patrol;
            }
        }
        else // Tipe Melee
        {
            Debug.Log($"Melee enemy: near = {near}, medium = {medium}, far = {far}");

            EnemyState newState = previousState; // Mulai dengan state sebelumnya

            if (near > 0.7f) // Kondisi Attack
            {
                Debug.Log("Melee: Attack (near)");
                newState = EnemyState.Attack;
            }
            else
            {
                // Histeresis logic untuk Chase dan Patrol menggunakan 'far'
                if (far > 0.6f)
                {
                    Debug.Log("Melee: Chase (far)");
                    newState = EnemyState.Chase;
                }
                else if (far < 0.4f)
                {
                    Debug.Log("Melee: Patrol");
                    newState = EnemyState.Patrol;
                }
                // Jika 'far' berada di antara 0.4 dan 0.6, tetap di state sebelumnya
            }

            // Update previousState hanya jika terjadi perubahan
            if (newState != previousState)
            {
                previousState = newState;
            }

            return newState;
        }
    }

    private float FuzzifyTrapezoid(float value, float a, float b, float c, float d)
    {
        if (value <= a || value >= d) return 0f; // Nilai di luar trapezoid
        if (value > a && value <= b) return (value - a) / (b - a); // Naik
        if (value > b && value <= c) return 1f; // Bagian puncak
        if (value > c && value < d) return (d - value) / (d - c); // Turun
        return 0f;
    }

    private float SmoothValue(float previousValue, float currentValue, float smoothingFactor)
    {
        return (previousValue * (1f - smoothingFactor)) + (currentValue * smoothingFactor);
    }
}
