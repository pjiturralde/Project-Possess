using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyStats : MonoBehaviour {
    private RangedEnemyPool rangedEnemyPool;
    private ArmedMeleeEnemyPool armedMeleeEnemyPool;
    private UnarmedMeleeEnemyPool unarmedMeleeEnemyPool;
    private SpriteRenderer spriteRenderer;
    private PlayerManager playerManager;
    private ItemManager itemManager;
    private PlayerStats playerStats;
    private EnemyWeapon enemyWeapon;
    private WaveManager waveManager;
    public GameObject damagePopUpPrefab;

    private Material defaultMaterial;
    public Material damagedMaterial;

    // Drops
    public GameObject coinPrefab;
    public GameObject axePrefab;
    public GameObject swordPrefab;
    public GameObject spearPrefab;

    public GameObject fireParticles;

    // Core stats
    public float MaxHealth = 50;

    // Current stats
    public float Health;
    public float DamageMultiplier;
    public float AttackRate;
    public float MovementSpeed;
    public float ExtraSpeed;
    public float ReducedSpeed;
    public float Defense;
    public bool isOnFire;
    public bool isPetrified;

    private float fireTimer;
    private float petrifiedTimer;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.5D;

    public bool isInitialized;

    public void Initialize(float health) {
        MovementSpeed = 5;
        ExtraSpeed = 0;
        ReducedSpeed = 0;
        Defense = 0;
        isOnFire = false;
        isPetrified = false;
        petrifiedTimer = 0;
        fireTimer = 0;

        rangedEnemyPool = RangedEnemyPool.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        unarmedMeleeEnemyPool = UnarmedMeleeEnemyPool.instance;
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        itemManager = playerManager.GetComponent<ItemManager>();
        waveManager = WaveManager.instance;
        fireParticles.SetActive(false);

        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        defaultMaterial = new Material(Shader.Find("Sprites/Default"));

        MaxHealth = health;
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
        Initialize(MaxHealth);
    }

    public void SetOnFire() {
        if (!isOnFire) {
            isOnFire = true;
            fireParticles.SetActive(true);
            fireTimer = 3; // 3 seconds on fire!

            for (int i = 0; i < 3; i++) {
                Invoke(nameof(FireTick), 1 + i);
            }
        }
    }

    public void FireTick() {
        float damage = 2;

        GameObject damagePopUp = Instantiate(damagePopUpPrefab);

        TextMeshPro tmp = damagePopUp.GetComponent<TextMeshPro>();

        tmp.text = damage.ToString();

        damagePopUp.transform.position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

        Health -= (damage - (damage * Defense));

        spriteRenderer.material = damagedMaterial;
        Invoke(nameof(ResetMaterial), 0.1f);
    }

    public void Petrify() {
        if (!isPetrified) {
            isPetrified = true;
            petrifiedTimer = 3; // 3 seconds petrified! O:
        }
    }

    void Update() {
        if (isOnFire) {
            ExtraSpeed = 0.5f;

            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime;

                if (fireTimer < 0) {
                    isOnFire = false;
                    fireParticles.SetActive(false);
                }
            }
        } else {
            ExtraSpeed = 0;
        }

        if (isPetrified) {
            ReducedSpeed = 0.5f;
            Defense = 0.1f; // enemy now takes 10% less damage! D:

            if (petrifiedTimer > 0) {
                petrifiedTimer -= Time.deltaTime;

                if (petrifiedTimer < 0) {
                    isPetrified = false;
                }
            }
        } else {
            ReducedSpeed = 0;
            Defense = 0;
        }

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

    public bool TakeDamage(float damage) {
        if (Invulnerable) {
            Debug.Log("Player Invulnerable");
            return false;
        }

        SoundManager.PlaySound(SoundType.METAL_IMPACT, 1, 0.1f);

        LoseHealth(damage * playerStats.DamageMultiplier);
        TriggerInvulnerability();

        return true;
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.5D;
    }

    private void LoseHealth(float amount) {
        float damage = amount;

        GameObject damagePopUp = Instantiate(damagePopUpPrefab);

        TextMeshPro tmp = damagePopUp.GetComponent<TextMeshPro>();

        int critRoll = Random.Range(1, 101);

        if (critRoll <= playerStats.CritChance) {
            damage = damage * 2; // TIMES TWO DAMAGE!?!?!
            tmp.color = Color.red;
        }

        tmp.text = damage.ToString();

        damagePopUp.transform.position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

        Health -= (damage - (damage * Defense));

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
        int dropRoll = Random.Range(1, 101);

        if (dropRoll <= 25 + playerStats.Luck) { // 25% chance to drop coins + Luck and 25% chance to drop weapon -- Luck caps out :)
            GameObject droppedCoin = Instantiate(coinPrefab);
            droppedCoin.transform.position = transform.position;
        } else if (dropRoll >= 75 - playerStats.WeaponLuck) {
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
                weaponStats.difficulty = enemyWeapon.difficulty;
                weaponStats.isShiny = enemyWeapon.isShiny;

                weapon.transform.position = transform.position;
                weapon.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
            }
        }

        WeaponStats playerWeapon = null;

        foreach (Transform child in playerManager.transform) {
            if (child.CompareTag("PlayerWeapon")) {
                playerWeapon = child.GetComponent<WeaponStats>();
                break;
            }
        }

        if (itemManager.HasItem("VampireFangs")) {
            if (playerWeapon != null) {
                if (playerWeapon.Durability + 2 < playerWeapon.MaxDurability) { // adds 2 durability every kill, a pretty good deal huh
                    playerWeapon.Durability += 2;
                } else {
                    playerWeapon.Durability = playerWeapon.MaxDurability;
                }
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

        waveManager.enemiesKilled++;
        waveManager.totalEnemiesKilled++;
    }
}
