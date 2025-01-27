using UnityEngine;

public class SpriteManager : MonoBehaviour {
    public static SpriteManager instance { get; private set; }
    public Sprite enemyAxeSprite;
    public Sprite enemySwordSprite;
    public Sprite enemySpearSprite;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Sprite GetSprite(string name) {
        if (name == "EnemyAxe") {
            return enemyAxeSprite;
        } else if (name == "EnemySword") {
            return enemySwordSprite;
        } else if (name == "EnemySpear") {
            return enemySpearSprite;
        }

        return null;
    }
}
