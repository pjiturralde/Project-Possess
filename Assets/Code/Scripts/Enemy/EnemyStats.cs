using TMPro;
using UnityEngine;

public class EnemyStats : MonoBehaviour {
    private RangedEnemyPool rangedEnemyPool;
    private ArmedMeleeEnemyPool armedMeleeEnemyPool;
    private UnarmedMeleeEnemyPool unarmedMeleeEnemyPool;
    private SpriteRenderer spriteRenderer;
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private EnemyWeapon enemyWeapon;
    public GameObject damagePopUpPrefab;

    private Material defaultMaterial;
    public Material damagedMaterial;

    // Drops
    public GameObject coinPrefab;
    public GameObject axePrefab;
    public GameObject swordPrefab;
    public GameObject spearPrefab;

    // Core stats
    public int MaxHealth = 50;

    // Current stats
    public float Health;
    public float DamageMultiplier;
    public float AttackRate;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.5D;

    public bool isInitialized;

    public void Initialize() {
        rangedEnemyPool = RangedEnemyPool.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        unarmedMeleeEnemyPool = UnarmedMeleeEnemyPool.instance;
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();

        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        defaultMaterial = new Material(Shader.Find("Sprites/Default"));

        Health = MaxHealth;
        Invulnerable = false;
        DamageMultiplier = 1;
        AttackRate = 1;
        
        if (gameObject.CompareTag("ArmedEnemy")) {
            foreach (Transform t in  transform) {
                if (t.CompareTag("EnemyWeapon")) {
                    enemyWeapon = t.GetComponent<EnemyWeapon>();
                }
            }
        }
        isInitialized = true;
    }

    private void Awake() {
        isInitialized = false;
    }

    void Start() {
        // Initialize starting values
        Initialize();
    }

    void Update() {
        // Checks if invulnerability frames are up
        HandleInvulnerability();
    }

    public void HandleInvulnerability() {
        if (Invulnerable) {
            invulnerabilityDuration -= Time.deltaTime;
            if (invulnerabilityDuration <= 0) {
                Invulnerable = false;
            }
        }
    }

    public void TakeDamage(float damage) {
        if (Invulnerable) {
            Debug.Log("Player Invulnerable");
            return;
        }

        GameObject damagePopUp = Instantiate(damagePopUpPrefab);

        TextMeshPro tmp = damagePopUp.GetComponent<TextMeshPro>();

        int critRoll = Random.Range(1, 101);

        if (critRoll <= playerStats.CritChance) {
            damage = damage * 2; // TIMES TWO DAMAGE!?!?!
            tmp.color = Color.red;
        }

        tmp.text = damage.ToString();

        damagePopUp.transform.position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

        LoseHealth(damage * playerStats.DamageMultiplier);
        TriggerInvulnerability();
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.5D;
    }

    private void LoseHealth(float amount) {
        Health -= amount;

        spriteRenderer.material = damagedMaterial;
        Invoke(nameof(ResetMaterial), 0.1f);

        if (gameObject.CompareTag("RangedEnemy")) {
            RangedEnemyBehaviour rangedEnemyBehaviour = gameObject.GetComponent<RangedEnemyBehaviour>();
            rangedEnemyBehaviour.Knockback(-(playerManager.transform.position - transform.position).normalized);
        } else if (gameObject.CompareTag("ArmedEnemy")) {
            MeleeEnemyBehaviour meleeEnemyBehaviour = gameObject.GetComponent<MeleeEnemyBehaviour>();
            meleeEnemyBehaviour.Knockback(-(playerManager.transform.position - transform.position).normalized);
        } else if (gameObject.CompareTag("UnarmedEnemy")) {
            UnarmedMeleeEnemyBehaviour unarmedMeleeEnemyBehaviour = gameObject.GetComponent<UnarmedMeleeEnemyBehaviour>();
            unarmedMeleeEnemyBehaviour.Knockback(-(playerManager.transform.position - transform.position).normalized);
        }

        if (Health <= 0) {
            Die();
        }
    }

    private void ResetMaterial() {
        spriteRenderer.material = defaultMaterial;
    }

    private void Die() {
        Debug.Log("Enemy has died");
        int dropRoll = Random.Range(1, 4);

        if (dropRoll == 1) { // 25% chance to drop coins and 25% chance to drop weapon
            GameObject droppedCoin = Instantiate(coinPrefab);
            droppedCoin.transform.position = transform.position;
        } else if (dropRoll == 2) {
            if (gameObject.CompareTag("ArmedEnemy")) {
                GameObject weapon = null;

                if (enemyWeapon.weaponIndex == 0) { // AXE IS 0, SWORD IS 1, SPEAR IS 2 RANDOMIZE!
                    weapon = Instantiate(axePrefab);
                } else if (enemyWeapon.weaponIndex == 1) {
                    weapon = Instantiate(swordPrefab);
                } else if (enemyWeapon.weaponIndex == 2) {
                    weapon = Instantiate(spearPrefab);
                }

                FreeWeaponStats weaponStats = weapon.GetComponent<FreeWeaponStats>();
                weaponStats.damage = enemyWeapon.damage;
                weaponStats.durability = enemyWeapon.durability;
                weaponStats.isShiny = enemyWeapon.isShiny;

                weapon.transform.position = transform.position;
                weapon.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
            }
        }

        TriggerInvulnerability();

        if (gameObject.CompareTag("RangedEnemy")) {
            rangedEnemyPool.DisableInstance(gameObject);
        } else if (gameObject.CompareTag("ArmedEnemy")) {
            armedMeleeEnemyPool.DisableInstance(gameObject);
        } else if (gameObject.CompareTag("UnarmedEnemy")) {
            unarmedMeleeEnemyPool.DisableInstance(gameObject);
        }
    }
}
