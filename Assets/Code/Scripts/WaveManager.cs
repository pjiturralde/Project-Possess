using UnityEngine;

public class WaveManager : MonoBehaviour {
    ArmedMeleeEnemyPool armedMeleeEnemyPool;

    void Start() {
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;

        for (int i = 0; i < 3; i++) {
            GameObject armedEnemy = armedMeleeEnemyPool.GetInstance(i);
            armedEnemy.transform.position = new Vector2(i * 3, 0);
        }
    }

    void Update() {
        
    }
}
