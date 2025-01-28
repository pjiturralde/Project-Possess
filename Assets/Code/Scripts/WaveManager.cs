using UnityEngine;

public class WaveManager : MonoBehaviour {
    ArmedMeleeEnemyPool armedMeleeEnemyPool;

    void Start() {
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;

        for (int i = 0; i < 10; i++) {
            GameObject armedEnemy = armedMeleeEnemyPool.GetInstance();
            armedEnemy.transform.position = new Vector2(i * 3, 0);
            Debug.Log("WHAT HTE SKIDIDI");
        }
    }

    void Update() {
        
    }
}
