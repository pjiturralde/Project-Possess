using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public static HealthManager instance { get; private set; }

    public Image healthBar;

    private PlayerManager playerManager;
    PlayerStats playerStats;
    private float healthAmount;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

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
