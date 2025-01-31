using UnityEngine;

public class WizardLookAt : MonoBehaviour {
    PlayerManager playerManager;
    SpriteRenderer wizardRenderer;

    void Start () {
        playerManager = PlayerManager.instance;
        wizardRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        Vector2 playerDirection = (playerManager.transform.position - transform.position);

        if (playerDirection.x < 0 && !wizardRenderer.flipX) {
            wizardRenderer.flipX = true;
        } else if (playerDirection.x >= 0 && wizardRenderer.flipX) {
            wizardRenderer.flipX = false;
        }
    }
}
