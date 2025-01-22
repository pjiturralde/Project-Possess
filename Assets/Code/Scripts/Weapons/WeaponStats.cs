using UnityEngine;

public class WeaponStats : MonoBehaviour {
    // Core stats
    public int MaxDurability = 50;
    public float cooldown;

    // Current stats
    public float Durability;
    public float Damage;

    // Cooldowns
    private bool Invulnerable;
    private double invulnerabilityDuration = 0.5D;

    void Start() {
        // Initialize starting values
        Durability = MaxDurability;
        Invulnerable = false;
        Damage = 10;
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

        LoseDurability(damage);
        TriggerInvulnerability();
    }

    private void TriggerInvulnerability() {
        Invulnerable = true;
        invulnerabilityDuration = 0.5D;
    }

    private void LoseDurability(float amount) {
        Durability -= amount;

        if (Durability <= 0) {
            Break();
        }
    }

    private void Break() {
        Debug.Log("Weapon has been broken");
        // Add break handling here
    }
}
