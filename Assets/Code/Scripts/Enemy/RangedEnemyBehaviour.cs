using UnityEngine;

public class RangedEnemyBehaviour : MonoBehaviour {
    public Transform playerTransform;
    public Rigidbody2D rb;

    public float Speed = 5;
    private Vector2 playerDirection;
    private float timer; // Timer to roll to retreat
    public float minDistance = 6; // If player gets closer than this roll to move
    public float maxDistance = 10;
    private float targetDistance; // Between minDistance and maxDistance
    private bool retreat;
    private bool reTarget;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        timer = 3;
        reTarget = true;
        retreat = false;
    }

    void Update() {
        playerDirection = (playerTransform.position - transform.position).normalized;
        float playerDistance = (playerTransform.position - transform.position).magnitude;

        if (playerDistance > maxDistance) {
            if (reTarget) {
                reTarget = false; // when reach targetDistance set reTarget to true
                targetDistance = Random.Range(minDistance + 1, maxDistance + 1);
            }
        }

        if (playerDistance < minDistance) {
            timer -= Time.deltaTime;

            if (timer <= 0) {
                timer = 3;
                retreat = Random.Range(0, 2) == 1;
                targetDistance = Random.Range(minDistance + 1, maxDistance + 1);
            }
        } else {
            timer = 3;
        }
    }

    private void FixedUpdate() {
        float playerDistance = (playerTransform.position - transform.position).magnitude;

        if (playerDistance > targetDistance && !reTarget) {
            rb.linearVelocity = playerDirection * Speed;
        } else {
            if (!reTarget) {
                reTarget = true;
                rb.linearVelocity = Vector2.zero;
            }
        }

        if (playerDistance < minDistance && retreat) {

        }
    }
}
