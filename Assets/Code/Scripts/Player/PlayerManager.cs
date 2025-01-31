using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager instance { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
