using System.Collections.Generic;
using UnityEngine;

public class UnarmedMeleeEnemyPool : MonoBehaviour {
    public GameObject UnarmedMeleeEnemyPrefab;
    public int poolSize = 10;

    List<GameObject> pool = new List<GameObject>();

    public static UnarmedMeleeEnemyPool instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++) {
            GameObject UnarmedMeleeEnemyInstance = Instantiate(UnarmedMeleeEnemyPrefab, transform);
            DisableInstance(UnarmedMeleeEnemyInstance);
            pool.Add(UnarmedMeleeEnemyInstance);
        }
    }

    public GameObject GetInstance() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeSelf) {
                EnableInstance(pool[i]);

                UnarmedMeleeEnemyBehaviour enemyBehaviour = pool[i].GetComponent<UnarmedMeleeEnemyBehaviour>();
                EnemyStats enemyStats = pool[i].GetComponent<EnemyStats>();

                if (!enemyBehaviour.isInitialized) {
                    enemyBehaviour.Initialize();
                    enemyStats.Initialize();
                }

                return pool[i];
            }
        }

        // if code passed point then no instances available so add one
        GameObject UnarmedMeleeEnemyInstance = Instantiate(UnarmedMeleeEnemyPrefab);
        UnarmedMeleeEnemyInstance.transform.SetParent(transform);
        pool.Add(UnarmedMeleeEnemyInstance);
        UnarmedMeleeEnemyInstance.transform.SetParent(null); // perhaps choose a different location
        return UnarmedMeleeEnemyInstance;
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
