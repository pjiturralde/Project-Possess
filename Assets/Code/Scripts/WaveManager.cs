using UnityEngine;

public class WaveManager : MonoBehaviour {
    ArmedMeleeEnemyPool armedMeleeEnemyPool;
    RangedEnemyPool rangedEnemyPool;

    void Start() {
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        rangedEnemyPool = RangedEnemyPool.instance;

        for (int i = 0; i < 2; i++) {
            GameObject armedEnemy = armedMeleeEnemyPool.GetInstance(i);
            armedEnemy.transform.position = new Vector2(i * 3, 0);
        }

        for (int i = 0; i < 1; i++) {
            GameObject rangedEnemy = rangedEnemyPool.GetInstance();
            rangedEnemy.transform.position = new Vector2(i * 3, 2);
        }
    }

    void Update() {
        
    }
}
