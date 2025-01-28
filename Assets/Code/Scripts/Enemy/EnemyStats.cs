using UnityEngine;

public class EnemyStats : MonoBehaviour {
    // Core stats
    public int MaxHealth = 50;

    // Current stats
    public int Health;
    public float DamageMultiplier;
    public float AttackRate;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.5D;

    public bool isInitialized;

    public void Initialize() {
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
        Debug.Log("Enemy has died");
        // Add death handling here
    }
}
