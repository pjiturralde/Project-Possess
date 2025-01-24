using UnityEngine;

public class RangedEnemyAttack : MonoBehaviour {
    private ProjectilePoolManager poolManager;
    private PlayerManager playerManager;
    private float projectileSpeed;
    private float cooldown;

    void Start(){
        poolManager = ProjectilePoolManager.instance;
        playerManager = PlayerManager.instance;
        projectileSpeed = 4;
        cooldown = 2f;
    }

    void Update() {
        // predictions, predictions!
        // calculate le angle to shoot the player le perfectly!
        if (cooldown > 0) {
            cooldown -= Time.deltaTime;

            if (cooldown <= 0) {
                cooldown = 2f;

                Vector3 playerDirection = (playerManager.transform.position - transform.position).normalized;
                Vector3 playerVelocity = playerManager.GetComponent<Rigidbody2D>().linearVelocity;

                float playerMoveAngle = Vector2.Angle(playerVelocity, -playerDirection) * Mathf.Deg2Rad;

                float shootAngle = Mathf.Asin(Mathf.Sin(playerMoveAngle) * playerVelocity.magnitude / projectileSpeed);

                float randomizedShootAngle = Random.Range(0, shootAngle);

                GameObject projectile = poolManager.GetInstance();
                projectile.transform.position = transform.position;

                projectile.GetComponent<Projectile>().SetDirection(playerDirection);
            }
        }
    }
}
