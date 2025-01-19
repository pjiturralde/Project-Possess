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
    private bool retreating;
    private bool reTarget;

    private GameObject[] enemies;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        timer = 3;
        reTarget = true;
        retreating = false;
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

            if (timer <= 0 && !retreating) {
                timer = 3;
                retreating = Random.Range(0, 2) == 1;
                targetDistance = Random.Range(minDistance + 1, maxDistance + 1);
            }
        } else {
            timer = 3;
        }
    }

    private void FixedUpdate() {
        float playerDistance = (playerTransform.position - transform.position).magnitude;

        if (playerDistance > targetDistance && !reTarget) {
            rb.linearVelocity = playerDirection * Speed + calculateSeperationForce();
            Debug.Log(rb.linearVelocity);
        } else if (playerDistance <= targetDistance) {
            if (!reTarget) {
                reTarget = true;
                rb.linearVelocity = Vector2.zero;
            }
        }

        if (playerDistance <= targetDistance && retreating) {
            rb.linearVelocity = -playerDirection * Speed + calculateSeperationForce();
        } else {
            if (retreating) {
                retreating = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private Vector2 calculateSeperationForce() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Vector2 seperationForce = Vector2.zero;
        float applyForceDistance = 3;

        foreach (GameObject enemy in enemies) {
            if (enemy != gameObject) {
                Vector2 direction = transform.position - enemy.transform.position;
                float distance = direction.magnitude;

                if (distance < applyForceDistance) {
                    seperationForce += direction.normalized / distance;
                }
            }
        }

        return seperationForce * 3;
    }
}
