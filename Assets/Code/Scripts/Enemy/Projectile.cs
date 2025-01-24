using UnityEngine;

public class Projectile : MonoBehaviour {
    private Vector2 direction;
    private Rigidbody2D rb;
    private float speed;
    private ProjectilePoolManager poolManager;
    private Camera mainCamera;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        poolManager = ProjectilePoolManager.instance;
        speed = 0.5f;
        direction = Vector2.zero;
        rb.linearVelocity = direction * speed;
        mainCamera = Camera.main;
    }

    public void SetDirection(Vector2 direction) {
        this.direction = direction.normalized;
        rb.linearVelocity = direction * speed;
    }

    private void Update() {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // check if off player screen
        if (viewportPosition.x < -0.1f || viewportPosition.x > 1.1f ||
            viewportPosition.y < -0.1f || viewportPosition.y > 1.1f) {
            // Send back to le pool
            poolManager.DisableInstance(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            // HIT!
            // take damage and send back to le poole!
            poolManager.DisableInstance(gameObject);
        }
    }
}
