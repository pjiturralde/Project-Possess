using UnityEngine;

public class Projectile : MonoBehaviour {
    private Vector2 direction;
    private Rigidbody2D rb;
    private float speed;
    private ProjectilePoolManager poolManager;
    private Camera mainCamera;
    private PlayerManager playerManager;
    private PlayerStats playerStats;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        poolManager = ProjectilePoolManager.instance;
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        direction = Vector2.zero;
        mainCamera = Camera.main;
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
            poolManager.DisableInstance(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            // HIT!
            // take damage and send back to le poole!
            playerStats.TakeDamage(10);
            poolManager.DisableInstance(gameObject);
        }
    }
}
