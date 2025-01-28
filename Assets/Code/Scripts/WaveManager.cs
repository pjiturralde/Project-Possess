using UnityEngine;

public class WaveManager : MonoBehaviour {
    ArmedMeleeEnemyPool armedMeleeEnemyPool;

    void Start() {
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;

        for (int i = 0; i < 2; i++) {
            GameObject armedEnemy = armedMeleeEnemyPool.GetInstance(0);
            armedEnemy.transform.position = new Vector2(i * 3, 0);
        }
    }

    void Update() {
        
    }
}
