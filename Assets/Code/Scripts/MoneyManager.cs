using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;

    // mods! ban everyone -- references
    public TextMeshPro tmp;

    void Start () {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
    }

    void Update() {
        if (!tmp.text.Equals(playerStats.Money.ToString())) {
            tmp.text = playerStats.Money.ToString();
        }
    }
}
