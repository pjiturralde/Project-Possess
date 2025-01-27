using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public Image healthBar;

    private PlayerManager playerManager;
    PlayerStats playerStats;
    private float healthAmount;

    private void Start() {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();

        healthAmount = playerStats.MaxHealth;
    }

    private void Update() {
        if (playerStats.Health != healthAmount) {
            healthAmount = playerStats.Health;
            healthBar.fillAmount = healthAmount / playerStats.MaxHealth;
        }
    }
}
