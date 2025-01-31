using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Spear : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private ItemManager itemManager;
    private SoundManager soundManager;
    private WeaponStats weaponStats;
    private LineRenderer line;

    private float ATTACK_RANGE; // how far the attack should go
    private float attackTime; // how long it takes for the attack animation to complete
    private float timeHeld; // how long left click is held down
    public bool attacking; // whether the attack animation is playing or not
    private bool canHit;
    private bool onCooldown; // whether the attack is on cooldown or not

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        itemManager = playerManager.GetComponent<ItemManager>();
        weaponStats = GetComponent<WeaponStats>();
        line = GetComponent<LineRenderer>();

        ATTACK_RANGE = 5;
        attackTime = 1 - playerStats.AttackRate;
        timeHeld = 0;
        attacking = false;
        onCooldown = false;
        canHit = false;
    }

    // Update is called once per frame
    void Update() {
        // Establish variables
        attackTime = 1 - playerStats.AttackRate;
        float charge = Mathf.Clamp(timeHeld * (1 + playerStats.AttackRate), 0, 1); // how charged up the attack is (0%->100%)
        Vector2 weaponPosition = transform.position;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distanceToMouse = mouseWorldPosition - weaponPosition;
        Vector2 directionToMouse = distanceToMouse.normalized;
        float angleToMouse = Mathf.Atan2(distanceToMouse.y, distanceToMouse.x) * 180 / Mathf.PI - 90;
        Vector2 targetPosition = weaponPosition + directionToMouse * ATTACK_RANGE * charge;

        if (!attacking) {
            // Rotate weapon towards mouse
            if ((mouseWorldPosition - (Vector2)transform.position).magnitude > 0.1f) {
                transform.eulerAngles = new Vector3(0, 0, angleToMouse);
            }
            // If holding left click
            if (Input.GetMouseButton(0) && !onCooldown) {
                timeHeld += Time.deltaTime;
                line.enabled = true;
                line.SetPosition(0, weaponPosition);
                line.SetPosition(1, targetPosition);
                line.startColor = new Color(charge, charge, charge, 0f);
                line.endColor = new Color(charge, charge, charge, charge);
            } 
            // If left click is released
            else if (timeHeld != 0) {
                timeHeld = 0;
                attacking = true;
                onCooldown = true;
                line.enabled = false;
                playerManager.transform.DOMove(targetPosition, attackTime).SetEase(Ease.InOutBack);
                Invoke(nameof(AllowHit), 0.3f);
                Invoke(nameof(StopHit), attackTime - 0.1f);
                Invoke(nameof(StopAttacking), attackTime);
                Invoke(nameof(EndCooldown),weaponStats.cooldown);
            }
        }
    }

    // OnTriggerStay2D is called when a collider stays within trigger -- PISCES IMA TRY THIS WAY! LOL WHY IS THERE TWO!?!
    private void OnTriggerExit2D(Collider2D collision) {
        if ((collision.CompareTag("ArmedEnemy") || collision.CompareTag("RangedEnemy") || collision.CompareTag("UnarmedEnemy")) && canHit) {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            if (enemy.TakeDamage(weaponStats.Damage)) {
                if (itemManager.HasItem("GlassBlade")) {
                    weaponStats.Durability -= 2; // a flat 2 damage per hit hmm
                }

                if (itemManager.HasItem("FrighteningFlame")) {
                    enemy.SetOnFire();
                }

                if (itemManager.HasItem("PetrifyingPebble")) {
                    enemy.Petrify();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((collision.CompareTag("ArmedEnemy") || collision.CompareTag("RangedEnemy") || collision.CompareTag("UnarmedEnemy")) && canHit) {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            if (enemy.TakeDamage(weaponStats.Damage)) {
                if (itemManager.HasItem("GlassBlade")) {
                    weaponStats.Durability -= 2; // a flat 2 damage per hit hmm
                }

                if (itemManager.HasItem("FrighteningFlame")) {
                    enemy.SetOnFire();
                }

                if (itemManager.HasItem("PetrifyingPebble")) {
                    enemy.Petrify();
                }
            }
        }
    }

    private void AllowHit() {
        canHit = true;
        SoundManager.PlaySound(SoundType.SPEAR, 1, 0.1f);
    }

    private void StopHit() {
        canHit = false;
    }

    private void StopAttacking() {
        attacking = false;
    }

    private void EndCooldown() {
        onCooldown = false;
    }
}

