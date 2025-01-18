using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    public float Speed = 5;
    public Transform playerTransform;
    public Rigidbody2D rb;
    private Vector2 playerDirection;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        playerDirection = (playerTransform.position - transform.position).normalized;
    }

    private void FixedUpdate() {
        rb.linearVelocity = playerDirection * Speed;
    }
}
