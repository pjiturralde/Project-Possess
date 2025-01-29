using UnityEngine;

public class Pickup : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    public int value;

    private void Start() {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerWeapon")) {

            playerStats.AddMoney(value);
            Destroy(gameObject);
        }
    }
}
