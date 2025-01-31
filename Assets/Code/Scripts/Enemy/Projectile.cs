using Unity.Cinemachine;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private Vector2 direction;
    private Rigidbody2D rb;
    private float speed;
    private ProjectilePoolManager poolManager;
    private Camera mainCamera;
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    public int damage;
    private float timer;
    public bool isInitialized;

    public void Initialize() {
        rb = GetComponent<Rigidbody2D>();
        poolManager = ProjectilePoolManager.instance;
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        direction = Vector2.zero;
        mainCamera = Camera.main;
        damage = 20;
        isInitialized = true;
    }

    private void Awake() {
        isInitialized = false;
    }

    private void Start() {
        Initialize();
    }

    public void SetDirection(Vector2 direction) {
        this.direction = direction.normalized;
        rb.linearVelocity = direction * speed;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    private void Update() {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // check if off player screen
        if (viewportPosition.x < -0.5f || viewportPosition.x > 1.5f ||
            viewportPosition.y < -0.5f || viewportPosition.y > 1.5f) {
            // Send back to le pool
            if (timer > 0) {
                timer -= Time.deltaTime;

                if (timer <= 0) {
                    poolManager.DisableInstance(gameObject);
                }
            }
        } else {
            timer = 1;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            // HIT!
            // take damage and send back to le poole!
            playerStats.TakeDamage(10);
            poolManager.DisableInstance(gameObject);
        } else if (collision.CompareTag("PlayerWeapon")) {
            GameObject playerWeapon = null;

            foreach (Transform child in playerManager.transform) {
                if (child.CompareTag("PlayerWeapon")) {
                    playerWeapon = child.gameObject;
                }
            }

            WeaponStats weaponStats = null;

            if (playerWeapon != null) {
                weaponStats = playerWeapon.GetComponent<WeaponStats>();
            }

            weaponStats.TakeDamage(damage);
            poolManager.DisableInstance(gameObject);
        }
    }
}
