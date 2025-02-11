using DG.Tweening;
using UnityEngine;

public class WeaponStats : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private DurabilityManager durabilityManager;
    private GameObject playerParticles;
    private ItemManager itemManager;

    private SpriteRenderer spriteRenderer;
    public Material defaultMaterial;
    public Material damagedMaterial;

    // Core stats
    public int MaxDurability;
    public float cooldown;

    // Current stats
    public float Durability;
    public float Damage;
    public bool isShiny;
    public float Speed;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.2D;

    void Start() {
        // Initialize starting values
        durabilityManager = DurabilityManager.instance;
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        itemManager = playerManager.GetComponent<ItemManager>();
        playerParticles = playerManager.transform.Find("PlayerParticles").gameObject;
        Invulnerable = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        GameObject shiny = transform.Find("ShinyParticles").gameObject;

        if (isShiny) {
            shiny.SetActive(true);
        }
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

        if (itemManager.HasItem("EscapePlan")) {
            if (playerStats.ExtraSpeed == 0) {
                playerStats.ExtraSpeed = 0.5f;
            }
        }

        spriteRenderer.material = damagedMaterial;
        Invoke(nameof(ResetMaterial), 0.1f);

        SoundManager.PlaySound(SoundType.METAL_IMPACT, 1.1f, 0.1f);

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

    public void LoseDurability(float amount) {
        Durability -= amount - (amount * playerStats.Defense); // Defense 0.1 is 10% damage reduction

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
