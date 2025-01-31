using UnityEngine;

public class WaveManager : MonoBehaviour {
    public static WaveManager instance { get; private set; }

    private ShopManager shopManager;

    private float spawnTimer;
    public int enemiesKilled;
    private int enemiesToKill;
    private int totalCurrentEnemies;
    private int totalEnemies;
    private int waveDifficulty;
    private int waveNumber;
    private bool isInRecess;
    PlayerManager playerManager;
    ArmedMeleeEnemyPool armedMeleeEnemyPool;
    RangedEnemyPool rangedEnemyPool;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        waveDifficulty = 1;
        playerManager = PlayerManager.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        rangedEnemyPool = RangedEnemyPool.instance;
        shopManager = ShopManager.instance;
        totalEnemies = 0;
        enemiesKilled = 0;
        totalCurrentEnemies = 0;
        spawnTimer = 1000000f;

        Invoke(nameof(YoLeaveHerAlone), 1);
    }

    public void YoLeaveHerAlone() {
        shopManager.SpawnWizard();
    }

    private void Update() {
        enemiesToKill = 100;
        totalCurrentEnemies = totalEnemies;

        Debug.Log(totalCurrentEnemies);
        if (enemiesKilled <= enemiesToKill) {
            if (spawnTimer > 0) {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0) {
                    spawnTimer = 5f;

                    SpawnArmedEnemy();
                    SpawnRangedEnemy();
                }
            }
        } else {
            isInRecess = true;
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

        totalEnemies++;
    }

    private void SpawnRangedEnemy() {
        float angle = Random.Range(0, 360);
        float radius = 12;

        Vector3 enemySpawnOffset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * radius;

        GameObject rangedEnemy = rangedEnemyPool.GetInstance(50);
        rangedEnemy.transform.position = playerManager.transform.position + enemySpawnOffset;

        totalEnemies++;
    }
}
