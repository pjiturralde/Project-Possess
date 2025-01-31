using System.Collections.Generic;
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
    public bool isInRecess;
    PlayerManager playerManager;
    ArmedMeleeEnemyPool armedMeleeEnemyPool;
    RangedEnemyPool rangedEnemyPool;

    private int baseEnemiesToKill;
    private float baseTimer;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        playerManager = PlayerManager.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        rangedEnemyPool = RangedEnemyPool.instance;
        shopManager = ShopManager.instance;
        baseEnemiesToKill = 20;
        baseTimer = 4f;
        totalEnemies = 0;
        enemiesKilled = 0;
        totalCurrentEnemies = 0;
        waveNumber = 0;
        spawnTimer = baseTimer;
    }

    private void Update() {
        enemiesToKill = baseEnemiesToKill + waveNumber * 10;
        totalCurrentEnemies = totalEnemies - enemiesKilled;

        if (!isInRecess) {
            if (enemiesKilled <= enemiesToKill) {
                if (spawnTimer > 0) {
                    spawnTimer -= Time.deltaTime;

                    if (spawnTimer <= 0) {
                        spawnTimer = baseTimer - waveNumber * 0.3f;

                        int random = Random.Range(0, 3);

                        if (random == 0) {
                            SpawnArmedEnemy();
                        } else {
                            SpawnRangedEnemy();
                        }
                    }
                }
            } else {
                isInRecess = true;
                enemiesKilled = 0;
                waveNumber++;
                shopManager.SpawnWizard();

                GameObject[] weapons = GameObject.FindGameObjectsWithTag("FreeWeapon");

                for (int i = 0; i < weapons.Length; i++) {
                    Destroy(weapons[i]);
                }

                GameObject[] armedEnemies = GameObject.FindGameObjectsWithTag("ArmedEnemy");
                GameObject[] unarmedEnemies = GameObject.FindGameObjectsWithTag("UnarmedEnemy");
                GameObject[] rangedEnemies = GameObject.FindGameObjectsWithTag("RangedEnemy");

                for (int i = 0; i < armedEnemies.Length; i++) {
                    Destroy(armedEnemies[i]);
                }

                for (int i = 0; i < unarmedEnemies.Length; i++) {
                    Destroy(unarmedEnemies[i]);
                }

                for (int i = 0; i < rangedEnemies.Length; i++) {
                    Destroy(rangedEnemies[i]);
                }
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

        totalEnemies++;
    }

    private void SpawnShinyArmedEnemy() {
        float angle = Random.Range(0, 360);
        float radius = 12;

        Vector3 enemySpawnOffset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * radius;

        int weaponIndex = Random.Range(0, 3);

        GameObject armedEnemy = armedMeleeEnemyPool.GetInstance(weaponIndex, 50);
        armedEnemy.transform.position = playerManager.transform.position + enemySpawnOffset;

        EnemyWeapon enemyWeapon = armedEnemy.transform.Find("Weapon").GetComponent<EnemyWeapon>();
        enemyWeapon.isShiny = true;

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
