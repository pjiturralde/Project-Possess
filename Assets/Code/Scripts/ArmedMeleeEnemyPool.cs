using System.Collections.Generic;
using UnityEngine;

public class ArmedMeleeEnemyPool : MonoBehaviour {
    public GameObject ArmedMeleeEnemyPrefab;
    public int poolSize = 10;

    List<GameObject> pool = new List<GameObject>();

    public static ArmedMeleeEnemyPool instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++) {
            GameObject ArmedMeleeEnemyInstance = Instantiate(ArmedMeleeEnemyPrefab, transform);
            DisableInstance(ArmedMeleeEnemyInstance);
            pool.Add(ArmedMeleeEnemyInstance);
        }
    }

    public GameObject GetInstance() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeSelf) {
                EnableInstance(pool[i]);

                MeleeEnemyBehaviour enemyBehaviour = pool[i].GetComponent<MeleeEnemyBehaviour>();
                EnemyStats enemyStats = pool[i].GetComponent<EnemyStats>();
                EnemyWeapon enemyWeapon = pool[i].transform.Find("Weapon").GetComponent<EnemyWeapon>();

                if (!enemyBehaviour.isInitialized) {
                    enemyBehaviour.Initialize();
                    enemyStats.Initialize();
                    enemyWeapon.Initialize();
                }

                return pool[i];
            }
        }

        // if code passed point then no instances available so add one
        GameObject ArmedMeleeEnemyInstance = Instantiate(ArmedMeleeEnemyPrefab);
        ArmedMeleeEnemyInstance.transform.SetParent(transform);
        pool.Add(ArmedMeleeEnemyInstance);
        ArmedMeleeEnemyInstance.transform.SetParent(null); // perhaps choose a different location
        return ArmedMeleeEnemyInstance;
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
