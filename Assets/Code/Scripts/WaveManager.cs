using UnityEngine;

public class WaveManager : MonoBehaviour {
    private float spawnTimer;
    PlayerManager playerManager;
    ArmedMeleeEnemyPool armedMeleeEnemyPool;
    RangedEnemyPool rangedEnemyPool;



    void Start() {
        playerManager = PlayerManager.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        rangedEnemyPool = RangedEnemyPool.instance;
        spawnTimer = 1f;
    }

    private void Update() {
        if (spawnTimer > 0) {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0) {
                spawnTimer = 5f;

                SpawnArmedEnemy();
                SpawnRangedEnemy();
            }
        }
    }

    private void SpawnArmedEnemy() {
        float angle = Random.Range(0, 360);
        float radius = 12;

        Vector3 enemySpawnOffset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * radius;

        int weaponIndex = Random.Range(0, 3);

        GameObject armedEnemy = armedMeleeEnemyPool.GetInstance(weaponIndex, 50);
        armedEnemy.transform.position = playerManager.transform.position + enemySpawnOffset;

        int shinyRoll = Random.Range(0, 5); // 1 in 5 is shiny?

        if (shinyRoll == 5) {
            EnemyWeapon enemyWeapon = armedEnemy.transform.Find("Weapon").GetComponent<EnemyWeapon>();
            enemyWeapon.isShiny = true;
        }
    }

    private void SpawnRangedEnemy() {
        float angle = Random.Range(0, 360);
        float radius = 12;

        Vector3 enemySpawnOffset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * radius;

        GameObject rangedEnemy = rangedEnemyPool.GetInstance(50);
        rangedEnemy.transform.position = playerManager.transform.position + enemySpawnOffset;
    }
}
