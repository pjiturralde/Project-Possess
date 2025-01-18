using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBehaviour : MonoBehaviour {

    public float Speed = 5;
    public Transform playerTransform;
    public Rigidbody2D rb;
    private Vector2 playerDirection;
    private float minDistance = 3; // From player
    private float mindDistance = 1.5f; // from ENEMY
    public LayerMask layerMask;
    private float timer;
    private Transform blocking;
    private int numEnemies;

    // Augment behaviour
    public float raycastDistance = 20;
    public float raycastSeperationDist = 2; // From center of enemy
    private bool isCircling;
    private int perpendicularDir;

    GameObject[] enemies;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        isCircling = false;
        timer = 0f;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void Update() {
        numEnemies = 0;

        foreach (GameObject enemy in enemies) {
            if (enemy != gameObject) {
                Vector2 direction = playerTransform.position - enemy.transform.position;
                float distance = direction.magnitude;

                if (distance < minDistance) {
                    numEnemies++;
                }
            }
        }

        playerDirection = (playerTransform.position - transform.position).normalized;
        if (directionClear() && isCircling == true) {
            isCircling = false;
        } else if (!directionClear() && isCircling == false) {
            if ((blocking.position - transform.position).magnitude <= mindDistance) {
                if (numEnemies < 12) {
                    isCircling = true;
                }

                if (timer <= 0) {
                    perpendicularDir = Random.Range(0, 2) == 0 ? 1 : -1;
                    timer = 2;
                }
            }
        }

        if (directionClear()) {
            if (timer > 0) {
                timer -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate() {
        if (!isCircling) {
            float radius = (playerTransform.position - transform.position).magnitude;
            if (radius >= minDistance && numEnemies < 12) {
                Vector2 seperationForce = Vector2.zero;
                float minsDistance = 10;

                foreach (GameObject enemy in enemies) {
                    if (enemy != gameObject) {
                        Vector2 direction = transform.position - enemy.transform.position;
                        float distance = direction.magnitude;

                        if (distance < minsDistance) {
                            seperationForce += direction.normalized / distance;
                        }
                    }
                }

                rb.linearVelocity = playerDirection * Speed + seperationForce;
            } else {
                rb.linearVelocity = Vector2.zero;
                timer = 0;
            }
        } else {
            rb.linearVelocity = Vector2.zero;

            if (numEnemies >= 12) {
                isCircling = false;
                Debug.Log(numEnemies);
            }
            rb.MovePosition(calculateNextPosition());
        }
    }

    private bool directionClear() {
        Vector2 origin = transform.position;
        Vector2 offset1 = origin + Vector2.Perpendicular(playerDirection).normalized * raycastSeperationDist + playerDirection * .5f;
        Vector2 offset2 = origin - Vector2.Perpendicular(playerDirection).normalized * raycastSeperationDist + playerDirection * .5f;

        RaycastHit2D hit1 = Physics2D.Raycast(offset1, playerDirection, raycastDistance, ~layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(offset2, playerDirection, raycastDistance, ~layerMask);

        Debug.DrawRay(offset1, playerDirection * raycastDistance);
        Debug.DrawRay(offset2, playerDirection * raycastDistance);

        if (hit1.collider != null ||  hit2.collider != null) {
            if (hit1.collider != null) {
                blocking = hit1.transform;
            } else {
                blocking = hit2.transform;
            }

            return false;
        }

        return true;
    }

    private Vector2 calculateNextPosition() {
        // first find orbit percent
        float radius = (playerTransform.position - transform.position).magnitude;
        float currentAngle = Mathf.Atan2(-playerDirection.x, -playerDirection.y) * Mathf.Rad2Deg;

        currentAngle = Mathf.Abs(((currentAngle + 270 ) % 360) - 360);

        currentAngle += 200 * perpendicularDir / radius * Time.fixedDeltaTime; // PLEASE FIGURE THIS OUT
        currentAngle = currentAngle % 360;

        Vector2 nextPosition = new Vector2(playerTransform.position.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad), playerTransform.position.y + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad));

        return nextPosition;
    }
}
