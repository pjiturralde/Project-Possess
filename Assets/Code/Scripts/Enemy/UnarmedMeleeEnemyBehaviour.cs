using UnityEngine;

public class UnarmedMeleeEnemyBehaviour : MonoBehaviour {
    private PlayerManager playerManager;
    private Transform playerTransform;
    public Rigidbody2D rb;
    private SpriteRenderer bodySpriteRenderer;
    public Animator animator;

    public float Speed = 5;
    private Vector2 playerDirection;
    private float timer; // Timer to roll to retreat
    private float runTime; // HOW LONG THE MAGE HAS BEEN RUNNING!
    public float maxRunTime = 3; // HOW LONG THE MAGE CAN RUN!
    private float timerStart = 2;
    public float minDistance = 6; // If player gets closer than this roll to move
    public float maxDistance = 12;
    private float targetDistance; // Between minDistance and maxDistance
    private bool retreating;
    private bool reTarget;

    private GameObject[] enemies;

    void Start() {
        playerManager = PlayerManager.instance;
        playerTransform = playerManager.transform;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        rb = GetComponent<Rigidbody2D>();
        bodySpriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>(); // DO NOT CHANGE THESE NAMES D:
        timer = timerStart;
        reTarget = true;
        runTime = maxRunTime;
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

        if (playerDistance < minDistance && !retreating) {
            timer -= Time.deltaTime;

            if (timer <= 0 && !retreating) {
                timer = timerStart;
                retreating = Random.Range(0, 2) == 1;
                targetDistance = Random.Range(minDistance + 1, maxDistance + 1);
            }
        } else {
            timer = timerStart;
        }

        if (rb.linearVelocity.x < 0 && !bodySpriteRenderer.flipX) {
            bodySpriteRenderer.flipX = true;
        } else if (rb.linearVelocity.x > 0 && bodySpriteRenderer.flipX) {
            bodySpriteRenderer.flipX = false;
        } else if (rb.linearVelocity.x == 0) {
            if (playerDirection.x < 0 && !bodySpriteRenderer.flipX) {
                bodySpriteRenderer.flipX = true;
            } else if (playerDirection.x >= 0 && bodySpriteRenderer.flipX) {
                bodySpriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate() {
        animator.SetFloat("Speed", 0); // set le speed to 0 for animation

        float playerDistance = (playerTransform.position - transform.position).magnitude;

        rb.linearVelocity = Vector2.zero;

        if (playerDistance > targetDistance && !reTarget) {
            runTime = maxRunTime;
            rb.linearVelocity = playerDirection * Speed + calculateSeperationForce(6, 4);
            animator.SetFloat("Speed", 1);
        } else if (playerDistance <= targetDistance) {
            if (!reTarget) {
                reTarget = true;
            }
        }

        if (playerDistance <= targetDistance && retreating) {
            rb.linearVelocity = -playerDirection * Speed + calculateSeperationForce(2, 3);
            animator.SetFloat("Speed", 1);

            if (runTime > 0) {
                runTime -= Time.deltaTime;

                if (runTime <= 0) {
                    runTime = maxRunTime;
                    retreating = false;
                }
            }
        } else {
            if (retreating) {
                retreating = false;
            }
        }
    }

    private Vector2 calculateSeperationForce(float applyForceDistance, float seperationFactor) {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Vector2 seperationForce = Vector2.zero;

        foreach (GameObject enemy in enemies) {
            if (enemy != gameObject) {
                Vector2 direction = transform.position - enemy.transform.position;
                float distance = direction.magnitude;

                if (distance < applyForceDistance) {
                    seperationForce += direction.normalized / distance;
                }
            }
        }

        return seperationForce * seperationFactor;
    }
}
