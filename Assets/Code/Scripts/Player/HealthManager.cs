using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public Image health;

    private PlayerManager playerManager;
    PlayerStats playerStats;
    private float healthAmount;

    private void Start() {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();

        healthAmount = playerStats.MaxHealth;
    }

    private void Update() {
        if (!playerStats.isPossessing) {
            if (playerStats.Health != healthAmount) {
                healthAmount = playerStats.Health;
                health.fillAmount = healthAmount / playerStats.MaxHealth;
            }
        }
    }
}
