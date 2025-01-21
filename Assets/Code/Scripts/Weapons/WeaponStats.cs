using UnityEngine;

public class WeaponStats : MonoBehaviour {
    // Core stats
    public int MaxDurability = 50;

    // Current stats
    public int Durability;
    public float Damage;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.5D;

    void Start() {
        // Initialize starting values
        Durability = MaxDurability;
        Invulnerable = false;
        Damage = 10;
        AttackRate = 1;
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

        //LoseDurability
        TriggerInvulnerability();
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.5D;
    }

    private void LoseDurability(int amount) {
        Durability -= amount;

        if (Durability <= 0) {
            Break();
        }
    }

    private void Break() {
        Debug.Log("Enemy has died");
        // Add break handling here
    }
}
