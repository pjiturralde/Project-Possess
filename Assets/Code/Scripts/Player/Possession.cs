using UnityEngine;

public class Possession : MonoBehaviour {
    public Material outline;
    private Material defaultMaterial;
    private Collider2D currentHit;
    private SpriteRenderer currentWeaponSpriteRenderer;
    private PlayerManager playerManager;
    private PlayerStats playerStats;

    public GameObject playerAxe;
    public GameObject playerSword;
    public GameObject playerSpear;

    void Start() {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        currentWeaponSpriteRenderer = null;
    }

    void Update() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (!playerStats.isPossessing) {
            if (currentHit != hit.collider) {

                if (currentHit != null) {
                    foreach (Transform child in currentHit.transform) {
                        if (child.CompareTag("EnemyWeapon")) {
                            currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();

                            currentWeaponSpriteRenderer.material = defaultMaterial;
                            break;
                        }
                    }
                }

                currentHit = hit.collider;

                if (currentHit != null) {
                    SpriteRenderer enemyWeaponSpriteRenderer;

                    if (hit.collider.CompareTag("Enemy")) {
                        foreach (Transform child in hit.transform) {
                            if (child.CompareTag("EnemyWeapon")) {
                                enemyWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();

                                defaultMaterial = enemyWeaponSpriteRenderer.material;
                                enemyWeaponSpriteRenderer.material = outline;
                                break;
                            }
                        }
                    }
                }
            }

            if (currentHit != null && Input.GetMouseButtonDown(0)) {
                playerStats.isPossessing = true;

                Debug.Log("HELLOOOO??");
                Instantiate(playerAxe, playerManager.transform);

                foreach (Transform child in currentHit.transform) {
                    if (child.CompareTag("EnemyWeapon")) {
                        currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();

                        currentWeaponSpriteRenderer.material = defaultMaterial;
                        break;
                    }
                }

                currentHit = null;
            }
        }
    }
}
