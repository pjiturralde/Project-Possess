using UnityEngine;

public class PlayerStats : MonoBehaviour {
    // Player controller reference
    public PlayerController controller;

    private SpriteRenderer spriteRenderer;
    public Material defaultMaterial;
    public Material damagedMaterial;

    // Current weapon reference -- idk why i said this ignore
    // add later once weapons added

    // Core stats
    public int MaxHealth = 50;
    public float MovementSpeed = 300.0f;

    // Current stats
    public float Health;
    public float DamageMultiplier;
    public float AttackRate;
    public int Money;
    public int CritChance;
    public bool isPossessing;
    // MAYBE ADD DEFENSE LATER (WHEN POSSESSING WEAPONS)

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.2D;

    void Start() {
        spriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        // Initialize starting values
        Health = MaxHealth;
        Invulnerable = false;
        DamageMultiplier = 1;
        AttackRate = 1;
        CritChance = 5; // represents 5%
        isPossessing = false;
        Money = 0;

        // Set speed on player controller
        controller.moveSpeed = MovementSpeed;
    }

    void Update() {
        //Debug.Log(Money);
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

    // Health management
    public void Heal(int amount) {
        if (Health == MaxHealth) {
            return;
        }

        // Check if amount heals above max health
        if (Health + amount > MaxHealth) {
            Health = MaxHealth;
        } else {
            Health += amount;
        }
    }

    public void TakeDamage(float damage) {
        if (Invulnerable) {
            Debug.Log("Player Invulnerable");
            return;
        }

        spriteRenderer.material = damagedMaterial;
        Invoke(nameof(ResetMaterial), 0.1f);

        LoseHealth(damage);
        TriggerInvulnerability();
    }

    private void ResetMaterial() {
        spriteRenderer.material = defaultMaterial;
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.2D;
    }

    private void LoseHealth(float amount) {
        Health -= amount;
        
        if (Health <= 0) {
            Die();
        }
    }

    private void Die() {
        Debug.Log("Player has died");
        // Add death handling here
    }

    // Money management
    public void AddMoney(int amount) {
        Money += amount;
    }

    public bool SpendMoney(int amount) {
        if (Money >= amount) {
            Money -= amount;
            return true;
        } else {
            // Not enough moneh
            return false;
        }
    }
}
