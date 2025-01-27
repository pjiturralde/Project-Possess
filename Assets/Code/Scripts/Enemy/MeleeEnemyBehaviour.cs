using Unity.Jobs;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeEnemyBehaviour : MonoBehaviour {
    // References
    public Animator animator;
    public LayerMask layerMask; // For player (exclude)
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    public Transform playerTransform;
    public Rigidbody2D rb;
    private SpriteRenderer bodySpriteRenderer;
    private SpriteRenderer shoulderSpriteRenderer;
    private EnemyWeapon weapon;

    public float Speed = 5;
    private Vector2 playerDirection;
    private float minDistance = 1.2f; // From player
    private float enemyCircleDist = 1.5f; // Distance from enemy in front that dictates when to start circling around that enemy
    private float changeCirclingDirTimer; // Timer so that this enemy doesn't change circling directions too often
    private float attackTimer; // Timer for when to attack!! raaahhh!!!
    private bool isAttacking; // is he attacking or is he not cuzzy?
    private bool isWindingUp; // is he winding up for the attack?
    private Transform blockingObject; // Any object that blocks the path of this enemy
    private int numEnemies; // Number of enemies surrounding player that are within minimum distance
    private int maxEnemies = 8; // Max number of enemies allowed to surround player before enemies stop

    // Augment behaviour
    public float raycastDistance = 1; 
    public float raycastSeperationDist = 0.6f; // offset perpendicular to playerDirection
    private bool isCircling;
    private int perpendicularDir;

    private GameObject[] enemies;

    void Start() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        playerTransform = playerManager.transform;
        bodySpriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>(); // DO NOT CHANGE THESE NAMES D:
        shoulderSpriteRenderer = transform.Find("Shoulder").GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        isCircling = false;
        changeCirclingDirTimer = 0f;
        attackTimer = 0;
        weapon = GetComponentInChildren<EnemyWeapon>();
    }

    void Update() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        playerDirection = (playerTransform.position - transform.position).normalized;
        numEnemies = 0;

        if (!isAttacking && !isWindingUp) {
            if (playerDirection.x < 0 && !bodySpriteRenderer.flipX) {
                bodySpriteRenderer.flipX = true;
                shoulderSpriteRenderer.flipX = true;
                weapon.ChangeDirection();
            } else if (playerDirection.x >= 0 && bodySpriteRenderer.flipX) {
                bodySpriteRenderer.flipX = false;
                shoulderSpriteRenderer.flipX = false;
                weapon.ChangeDirection();
            }
        }

        foreach (GameObject enemy in enemies) {
            Vector2 direction = playerTransform.position - enemy.transform.position;
            float distance = direction.magnitude;

            if (distance < minDistance + 0.5f) {
                numEnemies++;
            }
        }

        if (isFrontClear() && isCircling == true) {
            isCircling = false;
        } else if (!isFrontClear() && isCircling == false) {
            if ((blockingObject.position - transform.position).magnitude <= enemyCircleDist) {
                if (numEnemies < maxEnemies) {
                    isCircling = true;
                }

                if (changeCirclingDirTimer <= 0) {
                    perpendicularDir = Random.Range(0, 2) == 0 ? 1 : -1;
                    changeCirclingDirTimer = 2;
                }
            }
        }

        if (isFrontClear()) {
            if (changeCirclingDirTimer > 0) {
                changeCirclingDirTimer -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate() {
        animator.SetFloat("Speed", 1); // set le speed to 1 for animation

        if (attackTimer >= 0 && !isAttacking) {
            attackTimer -= Time.fixedDeltaTime;
        }

        float radius = (playerTransform.position - transform.position).magnitude;
        // if too close, retreat, if far, circle around obstacles and get near

        if (radius <= minDistance - 0.5f) {
            rb.linearVelocity = -playerDirection * Speed;
        } else {
            if (!isCircling) {

                if (radius >= minDistance && numEnemies < maxEnemies) { // check if not within range of player and number of enemies around player is less than max enemies allowed
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

                    rb.linearVelocity = playerDirection * Speed + seperationForce * 3;
                } else {
                    rb.linearVelocity = Vector2.zero;
                    changeCirclingDirTimer = 0;

                    animator.SetFloat("Speed", 0); // set le speed to 0 for animation
                }
            } else {
                rb.linearVelocity = Vector2.zero;

                if (numEnemies >= maxEnemies) {
                    isCircling = false;
                }
                rb.MovePosition(calculateNextPosition());
            }
        }

        if (radius <= minDistance + 0.5f) {
            // ATTACK!
            if (attackTimer <= 0) {
                isWindingUp = true;
                float windUpTime = 1;

                attackTimer = 3 + windUpTime;

                weapon.PlayAttackAnimation(windUpTime);
                Invoke(nameof(startAttack), windUpTime - 0.01f);
                Invoke(nameof(stopAttacking), 1 + windUpTime);
                // DAMAGE PLAYER RAAHHH!!)
            }
        }

        if (isAttacking) {
            rb.linearVelocity = Vector3.zero;
            animator.SetFloat("Speed", 0);
        }
    }

    private void startAttack() {
        float radius = (playerTransform.position - transform.position).magnitude;

        isWindingUp = false;
        isAttacking = true;
        if (playerDirection.x < 0 && !bodySpriteRenderer.flipX) {
            bodySpriteRenderer.flipX = true;
            shoulderSpriteRenderer.flipX = true;
        } else if (playerDirection.x >= 0 && bodySpriteRenderer.flipX) {
            bodySpriteRenderer.flipX = false;
            shoulderSpriteRenderer.flipX = false;
        }

        if (radius <= minDistance + 0.4f) {
            playerStats.TakeDamage(10);
        }
    }

    private void stopAttacking() { // sets isAttacking to false
        isAttacking = false;
    }

    // Checks if front of the enemy is obstructed
    private bool isFrontClear() {
        Vector2 origin = transform.position;
        Vector2 offset0 = origin + playerDirection * .6f;
        Vector2 offset1 = origin + Vector2.Perpendicular(playerDirection).normalized * raycastSeperationDist + playerDirection * .8f;
        Vector2 offset2 = origin - Vector2.Perpendicular(playerDirection).normalized * raycastSeperationDist + playerDirection * .8f;

        RaycastHit2D hit0 = Physics2D.Raycast(offset0, playerDirection, raycastDistance, ~layerMask);
        RaycastHit2D hit1 = Physics2D.Raycast(offset1, playerDirection, raycastDistance, ~layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(offset2, playerDirection, raycastDistance, ~layerMask);

        Debug.DrawRay(offset0, playerDirection * raycastDistance);
        Debug.DrawRay(offset1, playerDirection * raycastDistance);
        Debug.DrawRay(offset2, playerDirection * raycastDistance);

        if (hit0.collider != null || hit1.collider != null ||  hit2.collider != null) {
            if (hit0.collider != null) {
                blockingObject = hit0.transform;
            } else if (hit1.collider != null) {
                blockingObject = hit1.transform;
            } else {
                blockingObject = hit2.transform;
            }

            return false;
        }

        return true;
    }

    // Calculates next orbit position when circling an obstacle
    private Vector2 calculateNextPosition() {
        float radius = (playerTransform.position - transform.position).magnitude;
        float currentAngle = Mathf.Atan2(-playerDirection.x, -playerDirection.y) * Mathf.Rad2Deg;

        currentAngle = Mathf.Abs(((currentAngle + 270 ) % 360) - 360);

        currentAngle += 200 * perpendicularDir / radius * Time.fixedDeltaTime; // PLEASE FIGURE THIS OUT
        currentAngle = currentAngle % 360;

        Vector2 nextPosition = new Vector2(playerTransform.position.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad), playerTransform.position.y + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad));

        return nextPosition;
    }
}