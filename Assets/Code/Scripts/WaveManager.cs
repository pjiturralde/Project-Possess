using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    public static WaveManager instance { get; private set; }

    private ShopManager shopManager;

    private float spawnTimer;
    public int enemiesKilled;
    private int enemiesToKill;
    private int totalCurrentEnemies;
    public int totalEnemiesKilled;
    private int totalEnemies;
    private int waveDifficulty;
    public int waveNumber;
    public bool isInRecess;
    PlayerManager playerManager;
    ArmedMeleeEnemyPool armedMeleeEnemyPool;
    RangedEnemyPool rangedEnemyPool;

    private float mageOnlyTimer; // timer when only mages are on the scene

    private int baseEnemiesToKill;
    private float baseTimer;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start() {
        playerManager = PlayerManager.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        rangedEnemyPool = RangedEnemyPool.instance;
        shopManager = ShopManager.instance;
        baseEnemiesToKill = 10;
        baseTimer = 4f;
        totalEnemies = 0;
        totalEnemiesKilled = 0;
        enemiesKilled = 0;
        totalCurrentEnemies = 0;
        waveNumber = 0;
        spawnTimer = baseTimer;
        mageOnlyTimer = 3;
    }

    private void Update() {
        enemiesToKill = baseEnemiesToKill + waveNumber * 7;
        totalCurrentEnemies = totalEnemies - enemiesKilled;

        if (!isInRecess) {
            if (enemiesKilled <= enemiesToKill) {
                if (spawnTimer > 0) {
                    spawnTimer -= Time.deltaTime;

                    if (spawnTimer <= 0) {
                        spawnTimer = baseTimer - waveNumber * 0.3f;

                        int random = Random.Range(0, 2);

                        if (random == 0) {
                            SpawnArmedEnemy();
                        } else {
                            SpawnRangedEnemy();
                        }
                    }

                    GameObject[] armedEnemies = GameObject.FindGameObjectsWithTag("ArmedEnemy");
                    GameObject[] rangedEnemies = GameObject.FindGameObjectsWithTag("RangedEnemy");

                    if (armedEnemies.Length == 0 && rangedEnemies.Length >= 0) {
                        if (mageOnlyTimer > 0) {
                            mageOnlyTimer -= Time.deltaTime;

                            if (mageOnlyTimer <= 0) {
                                SpawnArmedEnemy();
                            }
                        }
                    } else {
                        mageOnlyTimer = 3;
                    }

                }
            } else {
                isInRecess = true;
                enemiesKilled = 0;
                waveNumber++;
                shopManager.SpawnWizard();

                GameObject[] armedEnemies = GameObject.FindGameObjectsWithTag("ArmedEnemy");
                GameObject[] unarmedEnemies = GameObject.FindGameObjectsWithTag("UnarmedEnemy");
                GameObject[] rangedEnemies = GameObject.FindGameObjectsWithTag("RangedEnemy");

                for (int i = 0; i < armedEnemies.Length; i++) {
                    armedMeleeEnemyPool.DisableInstance(armedEnemies[i]);
                }

                for (int i = 0; i < unarmedEnemies.Length; i++) {
                    armedMeleeEnemyPool.DisableInstance(unarmedEnemies[i]);
                }

                for (int i = 0; i < rangedEnemies.Length; i++) {
                    rangedEnemyPool.DisableInstance(rangedEnemies[i]);
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

        int shinyRoll = Random.Range(0, 10); // 1 in 5 is shiny?

        if (shinyRoll == 0) {
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
