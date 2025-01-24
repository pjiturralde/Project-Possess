using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        for (int i = 0; i < poolSize; i++) {
            GameObject projectileInstance = Instantiate(projectilePrefab, transform);
            /*            projectileInstance.transform.SetParent(transform);*/
            DisableInstance(projectileInstance);
            pool.Add(projectileInstance);
        }
    }

    public GameObject GetInstance() {
        for (int i = 0; i < pool.Count; i++) {
            SpriteRenderer sprite = pool[i].GetComponent<SpriteRenderer>();

            if (!sprite.enabled) {
                EnableInstance(pool[i]);
                return pool[i];
            }
        }

        // if code passed point then no instances available so add one
        GameObject projectileInstance = Instantiate(projectilePrefab);
        projectileInstance.transform.SetParent(transform);
        pool.Add(projectileInstance);
        return projectileInstance;
    }

    public void DisableInstance(GameObject instance) {
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        CircleCollider2D collider = instance.GetComponent<CircleCollider2D>();
        SpriteRenderer sprite = instance.GetComponent<SpriteRenderer>();
        Projectile projectile = instance.GetComponent<Projectile>();

        Debug.Log(projectile.name);
        rb.bodyType = RigidbodyType2D.Static;
        collider.enabled = false;
        sprite.enabled = false;

        instance.transform.SetParent(transform);
    }

    public void EnableInstance(GameObject instance) {
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        CircleCollider2D collider = instance.GetComponent<CircleCollider2D>();
        SpriteRenderer sprite = instance.GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        sprite.enabled = true;

        instance.transform.SetParent(null); // perhaps choose a different location
    }
}
