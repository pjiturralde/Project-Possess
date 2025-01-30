using DG.Tweening;
using UnityEngine;

public class WeaponStats : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private DurabilityManager durabilityManager;
    private GameObject playerParticles;

    private SpriteRenderer spriteRenderer;
    public Material defaultMaterial;
    public Material damagedMaterial;

    // Core stats
    public int MaxDurability;
    public float cooldown;

    // Current stats
    public float Durability;
    public float Damage;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.2D;

    void Start() {
        // Initialize starting values
        durabilityManager = DurabilityManager.instance;
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        playerParticles = playerManager.transform.Find("PlayerParticles").gameObject;
        Invulnerable = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
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
            Debug.Log("Weapon Invulnerable");
            return;
        }

        spriteRenderer.material = damagedMaterial;
        Invoke(nameof(ResetMaterial), 0.1f);

        LoseDurability(damage);
        TriggerInvulnerability();
    }
    private void ResetMaterial() {
        spriteRenderer.material = defaultMaterial;
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.2D;
    }

    public void TriggerInvulnerability(double invulnerableTime) {
        Invulnerable = true;
        invulnerabilityDuration = invulnerableTime;
    }

    private void LoseDurability(float amount) {
        Durability -= amount;

        if (Durability <= 0) {
            Break();
        }
    }

    private void Break() {
        playerStats.isPossessing = false;
        playerStats.GetComponent<CircleCollider2D>().enabled = true;
        playerStats.transform.Find("Body").GetComponent<SpriteRenderer>().enabled = true; // pls don't change name of body

        durabilityManager.DeactivateDurabilityBar();
        playerParticles.SetActive(true);
        Destroy(gameObject);
    }
}
