using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyPool : MonoBehaviour {
    public GameObject RangedEnemyPrefab;
    public int poolSize = 10;

    List<GameObject> pool = new List<GameObject>();

    public static RangedEnemyPool instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++) {
            GameObject RangedEnemyInstance = Instantiate(RangedEnemyPrefab, transform);
            DisableInstance(RangedEnemyInstance);
            pool.Add(RangedEnemyInstance);
        }
    }

    public GameObject GetInstance(float health) {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeSelf) {
                EnableInstance(pool[i]);

                RangedEnemyBehaviour enemyBehaviour = pool[i].GetComponent<RangedEnemyBehaviour>();
                EnemyStats enemyStats = pool[i].GetComponent<EnemyStats>();
                RangedEnemyAttack enemyAttack = pool[i].GetComponent<RangedEnemyAttack>();

                enemyBehaviour.Initialize();
                enemyStats.Initialize(health);
                enemyAttack.Initialize();

                return pool[i];
            }
        }

        // if code passed point then no instances available so add one
        GameObject RangedEnemyInstance = Instantiate(RangedEnemyPrefab);
        RangedEnemyInstance.transform.SetParent(transform);
        pool.Add(RangedEnemyInstance);
        RangedEnemyInstance.transform.SetParent(null); // perhaps choose a different location
        return RangedEnemyInstance;
    }

    public void DisableInstance(GameObject instance) {
        instance.transform.SetParent(transform);
        instance.SetActive(false);
    }

    public void EnableInstance(GameObject instance) {
        instance.SetActive(true);
        instance.transform.SetParent(null); // perhaps choose a different location
    }
}
