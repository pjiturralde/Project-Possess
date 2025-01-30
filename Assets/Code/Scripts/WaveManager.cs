using UnityEngine;

public class WaveManager : MonoBehaviour {
    PlayerManager playerManager;
    ArmedMeleeEnemyPool armedMeleeEnemyPool;
    RangedEnemyPool rangedEnemyPool;

    void Start() {
        playerManager = PlayerManager.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        rangedEnemyPool = RangedEnemyPool.instance;

        for (int i = 0; i < 3; i++) {
            SpawnArmedEnemy();
        }

        for (int i = 0; i < 3; i++) {
        }
    }

    private void SpawnArmedEnemy() {
        float angle = Random.Range(1, 360);
        float radius = 12;

        Vector3 enemySpawnOffset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * radius;

        int weaponIndex = Random.Range(0, 2);

        GameObject armedEnemy = armedMeleeEnemyPool.GetInstance(weaponIndex);
        armedEnemy.transform.position = playerManager.transform.position + enemySpawnOffset;

        int shinyRoll = Random.Range(1, 5); // 1 in 5 is shiny?

        if (shinyRoll == 1) {
            EnemyWeapon enemyWeapon = armedEnemy.transform.Find("Weapon").GetComponent<EnemyWeapon>();
            enemyWeapon.isShiny = true;
        }
    }

    private void SpawnRangedEnemy() {
        float angle = Random.Range(1, 360);
        float radius = 12;

        Vector3 enemySpawnOffset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * radius;

        GameObject rangedEnemy = rangedEnemyPool.GetInstance();
        rangedEnemy.transform.position = enemySpawnOffset;
    }
}
