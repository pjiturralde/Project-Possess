using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;

    // mods! ban everyone
    public GameObject text;
    public TextMeshPro tmp;


    void Start () {
        tmp = text.GetComponent<TextMeshPro>();
    }

    void Update() {
        if (!tmp.text.Equals(playerStats.Money.ToString())) {
            tmp.text = playerStats.Money.ToString();
        }
    }
}
