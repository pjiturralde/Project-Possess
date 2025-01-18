using UnityEngine;

public class PlayerStats : MonoBehaviour {
    // Player controller reference
    public PlayerController controller;

    // Current weapon reference
    // add later once weapons added

    // Core stats
    public int MaxHealth = 50;
    public float MovementSpeed = 300.0f;

    // Current stats
    public int Health;
    public float DamageMultiplier;
    public float AttackRate;
    public int Money;
    public float CritChance;
    public bool isPossessing;
    // MAYBE ADD DEFENSE LATER (WHEN POSSESSING WEAPONS)

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.5D;

    void Start() {
        // Initialize starting values
        Health = MaxHealth;
        Invulnerable = false;
        DamageMultiplier = 1;
        AttackRate = 1;
        CritChance = 1;
        isPossessing = false;
        Money = 0;

        // Set speed on player controller
        controller.moveSpeed = MovementSpeed;
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

        //LoseHealth
        TriggerInvulnerability();
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.5D;
    }

    private void LoseHealth(int amount) {
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
