using UnityEngine;

public class RangedEnemyAttack : MonoBehaviour {
    private ProjectilePoolManager poolManager;
    private PlayerManager playerManager;
    private Rigidbody2D rb;
    private float projectileSpeed;
    private float cooldown;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        poolManager = ProjectilePoolManager.instance;
        playerManager = PlayerManager.instance;
        projectileSpeed = 7;
        cooldown = 2f;
    }

    void Update() {
        // predictions, predictions!
        // calculate le angle to shoot the player le perfectly!
        if (rb.linearVelocity == Vector2.zero) {
            if (cooldown > 0) {
                cooldown -= Time.deltaTime;

                if (cooldown <= 0) {
                    cooldown = 2f;
                    Vector2 playerPosition = playerManager.transform.position;
                    Vector2 enemyPosition = transform.position;

                    Vector2 playerDirection = (playerPosition - enemyPosition).normalized;
                    Vector2 playerVelocity = playerManager.GetComponent<Rigidbody2D>().linearVelocity;

                    GameObject instance = poolManager.GetInstance();
                    Projectile projectile = instance.GetComponent<Projectile>();
                    projectile.transform.position = enemyPosition;

                    Vector2 predictedPlayerPos = predictedPosition(enemyPosition, projectileSpeed, playerPosition, playerVelocity);
                    Vector2 predictedPlayerDir = (predictedPlayerPos - enemyPosition).normalized;

                    float predictedPlayerDirAngle = Vector2.SignedAngle(playerDirection, predictedPlayerDir);

                    // epically randomize !!
                    float randomizedShootAngle = predictedPlayerDirAngle < 0 ? Random.Range(predictedPlayerDirAngle, 0) : Random.Range(0, predictedPlayerDirAngle);

                    // set speed before direction
                    projectile.SetSpeed(projectileSpeed);
                    projectile.SetDirection(Quaternion.AngleAxis(randomizedShootAngle, Vector3.forward) * playerDirection);
                }
            }
        }
    }

    private Vector2 predictedPosition(Vector2 startPos, float projectileSpeed, Vector2 targetPos, Vector2 targetVelocity) {
        Vector2 playerDirection = targetPos - startPos;

        float a = Vector2.Dot(targetVelocity, targetVelocity) - (projectileSpeed * projectileSpeed);

        float b = 2 * Vector2.Dot(targetVelocity, playerDirection);
        float c = Vector2.Dot(playerDirection, playerDirection);

        float p = -b / (2 * a);
        float q = (float)Mathf.Sqrt((b * b) - 4 * a * c) / (2 * a);

        float t1 = p - q;
        float t2 = p + q;
        float t;

        if (t1 > t2 && t2 > 0) {
            t = t2;
        } else {
            t = t1;
        }

        Vector2 aimSpot = targetPos + targetVelocity * t;
        
        return aimSpot;
    }
}
