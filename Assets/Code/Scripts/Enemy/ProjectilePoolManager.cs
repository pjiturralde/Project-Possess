using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour {
    public GameObject projectilePrefab;
    public int poolSize = 10;

    List<GameObject> pool = new List<GameObject>();

    public static ProjectilePoolManager instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;

        for (int i = 0; i < poolSize; i++) {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform);
            DisableInstance(projectileInstance);
            pool.Add(projectileInstance);
        }
    }

    public GameObject GetInstance() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeSelf) {
                EnableInstance(pool[i]);

                Projectile projectile = pool[i].GetComponent<Projectile>();

                if (!projectile.isInitialized) {
                    projectile.Initialize();
                }

                return pool[i];
            }
        }

        // if code passed point then no instances available so add one
        GameObject projectileInstance = Instantiate(projectilePrefab);
        projectileInstance.transform.SetParent(transform);
        pool.Add(projectileInstance);
        projectileInstance.transform.SetParent(null); // perhaps choose a different location
        return projectileInstance;
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
