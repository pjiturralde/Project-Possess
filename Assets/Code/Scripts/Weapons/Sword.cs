using DG.Tweening;
using UnityEngine;

public class Sword : MonoBehaviour {
    private Transform player;
    private Vector3[] path = new Vector3[6];
    private Quaternion lookAtRotation;
    private WeaponStats stats;
    private bool holdingClick;
    private bool attacking;
    private bool canAttack;
    private bool canHit;
    private float cdTimer;
    public float scale;

    void Start() {
        player = transform.parent;
        attacking = false;
        canHit = false;
        stats = GetComponent<WeaponStats>();
        cdTimer = 0;
        canAttack = true;
        scale = 0.07f;
        holdingClick = false;

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
            if ((mousePosition - transform.position).magnitude > 0.1f) {
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }

        Vector3[] newPath = new Vector3[path.Length];

        if (Input.GetMouseButtonDown(0) && !holdingClick) {
            holdingClick = true;
        }

        if (Input.GetMouseButtonUp(0) && holdingClick) {
            holdingClick = false;
        }

        if (holdingClick) {
            if (canAttack) {
                for (int i = 0; i < path.Length; i++) {
                    newPath[i] = Quaternion.AngleAxis(angle - 90, Vector3.forward) * path[i] * scale;
                    path[i] = new Vector3(path[i].x * -1, path[i].y, 0);
                }

                attacking = true;
                Invoke(nameof(StartAllowingHit), 0.1f);
                Invoke(nameof(StopAllowingHit), 0.5f);
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

    void StartAllowingHit() {
        canHit = true;
    }

    void StopAllowingHit() {
        canHit = false;
    }

    void StopAttacking() {
        attacking = false;
        cdTimer = stats.cooldown;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if ((collision.CompareTag("ArmedEnemy") || collision.CompareTag("RangedEnemy") || collision.CompareTag("UnarmedEnemy")) && canHit) {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();

            enemy.TakeDamage(stats.Damage);
            Debug.Log("DAMAGE:");
            Debug.Log(stats.Damage);
        }
    }
}
