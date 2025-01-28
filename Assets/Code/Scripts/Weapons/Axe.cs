using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class Axe : MonoBehaviour
{
    private Vector3[] path = new Vector3[6];
    private Quaternion lookAtRotation;
    private WeaponStats stats;
    private float damage;
    private bool attacking;
    private bool canAttack;
    private bool holdingClick;
    private float cdTimer;
    private float radius;

    void Start() {
        attacking = false;
        damage = 10;
        stats = GetComponent<WeaponStats>();
        cdTimer = 0;
        canAttack = true;
        holdingClick = false;
        radius = 0.1f;

        path = GenerateCirclePoints(radius, 10);
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

        if (Input.GetMouseButtonDown(0) && !holdingClick) {
            holdingClick = true;
        }

        if (Input.GetMouseButtonUp(0) && holdingClick) {
            holdingClick = false;
        }

        if (holdingClick) {
            if (canAttack) {
                for (int i = 0; i < path.Length; i++) {
                    newPath[i] = Quaternion.AngleAxis(angle - 90, Vector3.forward) * path[i];
                    //path[i] = new Vector3(path[i].x * -1, path[i].y, 0);
                }

                attacking = true;
                canAttack = false;

                Sequence sequence1 = DOTween.Sequence();
                sequence1.Append(transform.DOLocalMove(Quaternion.AngleAxis(angle - 90, Vector3.forward) * new Vector3(0, -radius, 0), 0.3f));
                sequence1.Append(transform.DOLocalPath(newPath, 0.6f, PathType.CatmullRom));
                sequence1.Append(transform.DOLocalMove(Quaternion.AngleAxis(angle - 90, Vector3.forward) * new Vector3(0, 0, 0), 0.3f));

                Sequence sequence2 = DOTween.Sequence();
                sequence2.AppendInterval(0.3f);
                sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, 360), 0.6f, RotateMode.LocalAxisAdd));

                Invoke(nameof(StopAttacking), 1.2f);
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

    private Vector3[] GenerateCirclePoints(float radius, int numSegments) {
        Vector3[] points = new Vector3[numSegments+1];

        for (int i = 0; i < numSegments+1; i++) {
            float angle = i * Mathf.PI * 2 / numSegments;
            float x = Mathf.Sin(angle) * radius;
            float y = -Mathf.Cos(angle) * radius;
            if (i == numSegments-1) {
                Debug.Log(angle * Mathf.Rad2Deg);
                Debug.Log(x);
                Debug.Log(y);
            }
            points[i] = new Vector3(x, y, 0);
        }

        return points;
    } 
}
