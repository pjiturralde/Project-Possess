using DG.Tweening;
using UnityEngine;

public class Sword : MonoBehaviour {
    private Transform player;
    private Vector3[] path = new Vector3[6];
    private Quaternion lookAtRotation;
    private WeaponStats stats;
    private float damage;
    private bool attacking;
    private bool canAttack;
    private float cdTimer;

    void Start() {
        player = transform.parent;
        attacking = false;
        damage = 10;
        stats = GetComponent<WeaponStats>();
        cdTimer = 0;
        canAttack = true;

        path = new Vector3[] {
            new Vector3(-0.5f, 0, 0),
            new Vector3(-1f, 1.5f, 0),
            new Vector3(0, 3, 0),
            new Vector3(1f, 1.5f, 0),
            new Vector3(0.5f, 0, 0),
            new Vector3(0, 0, 0)
        };
    }

    void Update() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
        Vector3 mouseDirection = mousePosition - transform.position;

        float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;

        if (!attacking) {
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        Vector3[] newPath = new Vector3[path.Length];

        if (Input.GetMouseButtonDown(0)) {
            if (canAttack) {
                for (int i = 0; i < path.Length; i++) {
                    newPath[i] = Quaternion.AngleAxis(angle - 90, Vector3.forward) * path[i];
                    path[i] = new Vector3(path[i].x * -1, path[i].y, 0);
                }

                attacking = true;
                canAttack = false;
                transform.DOLocalPath(newPath, 0.6f, PathType.CatmullRom);

                Invoke(nameof(StopAttacking), 0.6f);
            }
        }

        if (cdTimer > 0) {
            cdTimer -= Time.deltaTime;

            if (cdTimer <= 0) {
                canAttack = true;
            }
        }
    }

    void StopAttacking() {
        attacking = false;
        cdTimer = stats.cooldown;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy") && attacking) {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();

            enemy.TakeDamage(damage);
        }
    }
}
