using UnityEngine;

public class EnemyStats : MonoBehaviour {
    private RangedEnemyPool rangedEnemyPool;
    private ArmedMeleeEnemyPool armedMeleeEnemyPool;
    private UnarmedMeleeEnemyPool unarmedMeleeEnemyPool;
    private SpriteRenderer spriteRenderer;
    private PlayerManager playerManager;

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

        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        
        Health = MaxHealth;
        Invulnerable = false;
        DamageMultiplier = 1;
        AttackRate = 1;
        isInitialized = true;
    }

    private void Awake() {
        isInitialized = false;
    }

    void Start() {
        // Initialize starting values
        rangedEnemyPool = RangedEnemyPool.instance;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        unarmedMeleeEnemyPool = UnarmedMeleeEnemyPool.instance;
        playerManager = PlayerManager.instance;

        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        Health = MaxHealth;
        Invulnerable = false;
        DamageMultiplier = 1;
        AttackRate = 1;
        isInitialized = true;
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

        LoseHealth(damage);
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
        if (gameObject.CompareTag("RangedEnemy")) {
            rangedEnemyPool.DisableInstance(gameObject);
        } else if (gameObject.CompareTag("ArmedEnemy")) {
            armedMeleeEnemyPool.DisableInstance(gameObject);
        } else if (gameObject.CompareTag("UnarmedEnemy")) {
            unarmedMeleeEnemyPool.DisableInstance(gameObject);
        }
    }
}
