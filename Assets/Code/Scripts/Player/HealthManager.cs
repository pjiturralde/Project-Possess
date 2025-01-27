using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
    public static HealthManager instance { get; private set; }
    public Image healthBar;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
