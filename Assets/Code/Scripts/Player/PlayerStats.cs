using UnityEngine;

public class PlayerStats : MonoBehaviour {
    // Player controller reference
    public PlayerController controller;
    private ItemManager itemManager;

    private SpriteRenderer spriteRenderer;
    public Material defaultMaterial;
    public Material damagedMaterial;

    // Current weapon reference -- idk why i said this ignore
    // add later once weapons added

    // Core stats
    public int MaxHealth = 50;
    public float MovementSpeed = 6;

    // Current stats
    public float Health;
    public float DamageMultiplier;
    public float AttackRate;
    public int Money;
    public int CritChance; // 1 is 1% crit chance
    public int Luck; // 1 adds 1% chance for gold to drop :D
    public int WeaponLuck; // 1 adds 1% chance for weapon to drop ^_^
    public float Defense; // adds durability yo! -- .1 is 10% less damage
    public float ExtraSpeed; // adds extra speed :/

    private float extraSpeedTimer;

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
        AttackRate = 0; // 0.1 is 10% less cooldown, yes i know very confusing idk
        CritChance = 5; // represents 5%
        isPossessing = false;
        Money = 1000;
        Luck = 0;
        ExtraSpeed = 0;
        WeaponLuck = 0;
        Defense = 0;
        itemManager = GetComponent<ItemManager>();
        extraSpeedTimer = 2; // Escape plan item will run for 2 seconds when hit
    }

    void Update() {
        //Debug.Log(Money);
        // Checks if invulnerability frames are up
        HandleInvulnerability();

        if (ExtraSpeed > 0) {
            if (extraSpeedTimer > 0) {
                extraSpeedTimer -= Time.deltaTime;

                if (extraSpeedTimer <= 0) {
                    extraSpeedTimer = 2;
                    ExtraSpeed = 0;
                }
            }
        }
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

        if (itemManager.HasItem("EscapePlan")) {
            if (ExtraSpeed == 0) {
                ExtraSpeed = 0.5f;
            }
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

        if (itemManager.HasItem("SpareSkull")) { // woahh spare skull >_>
            itemManager.RemoveItem("SpareSkull");
            Health = MaxHealth;
        } else {
            // DIE!
        }
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
