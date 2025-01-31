using UnityEngine;
using UnityEngine.UI;

public class DurabilityManager : MonoBehaviour {
    public static DurabilityManager instance { get; private set; }
    public Transform durabilityBar;
    public Image durability;

    private PlayerManager playerManager;
    WeaponStats weaponStats;
    private float durabilityAmount;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        playerManager = PlayerManager.instance;
    }

    private void Update() {
        if (weaponStats != null) {
            if (weaponStats.Durability != durabilityAmount) {
                durabilityAmount = weaponStats.Durability;
                durability.fillAmount = durabilityAmount / weaponStats.MaxDurability;
            }
        }
    }

    // only activate when playerStats.isPossessing
    public void ActivateDurabilityBar() {
        foreach (Transform element in durabilityBar) {
            if (!element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
            }
        }

        GameObject weapon = null;

        foreach (Transform child in playerManager.transform) {
            if (child.CompareTag("PlayerWeapon")) {
                weapon = child.gameObject;
            }
        }

        if (weapon != null) {
            weaponStats = weapon.GetComponent<WeaponStats>();
        }
    }

    public void DeactivateDurabilityBar() {
        foreach (Transform element in durabilityBar) {
            if (element.gameObject.activeSelf) {
                element.gameObject.SetActive(false);
            }
        }

        weaponStats = null;
    }
}
