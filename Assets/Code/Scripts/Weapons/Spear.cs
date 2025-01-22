using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine;

public class Spear : MonoBehaviour {
    private WeaponStats stats;
    private float damage;
    private float cooldown;
    private float ATTACK_RANGE; // how far the attack should go
    private float attackTime; // how long it takes for the attack animation to complete
    private LineRenderer line;
    private float timeHeld;
    private bool attacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        stats = GetComponent<WeaponStats>();
        damage = stats.Damage;
        cooldown = stats.cooldown;
        ATTACK_RANGE = 5;
        attackTime = cooldown / 2; // attack animation speed is half the charge speed, increasing or decreasing with cooldown time
        line = GetComponent<LineRenderer>();
        timeHeld = 0;
        attacking = false;
    }

    // Update is called once per frame
    void Update() {
        damage = stats.Damage;
        cooldown = stats.cooldown;
        attackTime = cooldown / 2;
        // Establish variables
        float charge = Mathf.Clamp(timeHeld / cooldown, 0, 1); // how charged up the attack is (0%->100%)
        Vector2 weaponPosition = transform.position;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distanceToMouse = mouseWorldPosition - weaponPosition;
        Vector2 directionToMouse = distanceToMouse.normalized;
        float angleToMouse = Mathf.Atan2(distanceToMouse.y, distanceToMouse.x) * 180 / Mathf.PI - 90;
        Vector2 targetPosition = weaponPosition + directionToMouse * ATTACK_RANGE * charge;

        if (!attacking) {
            // Rotate weapon towards mouse
            transform.eulerAngles = new Vector3(0, 0, angleToMouse);

            //If holding left click
            if (Input.GetMouseButton(0)) {
                timeHeld += Time.deltaTime;
                line.enabled = true;
                line.SetPosition(1, new Vector2(0, ATTACK_RANGE * charge / 2));
            } 
            //If left click is released
            else if (timeHeld != 0) {
                attacking = true;
                Invoke(nameof(StopAttacking), attackTime);
                line.enabled = false;
                transform.DOMove(targetPosition, attackTime); //!!!NEED TO MOVE CHARACTER NOT WEAPON!!!
                timeHeld = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy") && attacking) {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            enemy.TakeDamage(damage);
        }
    }

    private void StopAttacking() {
        attacking = false;
    }
}

