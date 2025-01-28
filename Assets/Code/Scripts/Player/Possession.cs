using Unity.Burst.Intrinsics;
using UnityEngine;

public class Possession : MonoBehaviour {
    public Material outline;
    private Material defaultMaterial;
    private Collider2D currentHit;
    private SpriteRenderer currentWeaponSpriteRenderer;
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private ArmedMeleeEnemyPool armedMeleeEnemyPool;
    private UnarmedMeleeEnemyPool unarmedMeleeEnemyPool;
    private QuickTimeEventManager quickTimeEventManager;
    private DurabilityManager durabilityManager;
    private Collider2D weaponToSteal;

    public GameObject playerAxe;
    public GameObject playerSword;
    public GameObject playerSpear;

    void Start() {
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        currentWeaponSpriteRenderer = null;
        armedMeleeEnemyPool = ArmedMeleeEnemyPool.instance;
        unarmedMeleeEnemyPool = UnarmedMeleeEnemyPool.instance;
        quickTimeEventManager = QuickTimeEventManager.instance;
        durabilityManager = DurabilityManager.instance;
        weaponToSteal = null;
    }

    void Update() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (!playerStats.isPossessing && weaponToSteal == null) {
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
                foreach (Transform child in currentHit.transform) {
                    if (child.CompareTag("EnemyWeapon")) {
                        currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();

                        currentWeaponSpriteRenderer.material = defaultMaterial;
                        break;
                    }
                }

                weaponToSteal = currentHit;
                quickTimeEventManager.Activate();
            }
        }
    }

    public void StealWeapon() {
        playerStats.isPossessing = true;
        playerStats.GetComponent<CircleCollider2D>().enabled = false;
        playerStats.transform.Find("Body").GetComponent<SpriteRenderer>().enabled = false; // pls don't change name of body

        foreach (Transform child in weaponToSteal.transform) {
            if (child.CompareTag("EnemyWeapon")) {
                currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();
                EnemyWeapon enemyWeapon = child.GetComponent<EnemyWeapon>();

                int weaponIndex = enemyWeapon.weaponIndex;

                if (weaponIndex == 0) {
                    Instantiate(playerAxe, playerManager.transform);
                } else if (weaponIndex == 1) {
                    Instantiate(playerSword, playerManager.transform);
                } else {
                    Instantiate(playerSpear, playerManager.transform);
                }

                currentWeaponSpriteRenderer.material = defaultMaterial;
                break;
            }
        }

        MeleeEnemyBehaviour meleeEnemyBehaviour = weaponToSteal.GetComponent<MeleeEnemyBehaviour>();
        meleeEnemyBehaviour.CancelInvoke();

        GameObject armedEnemy = weaponToSteal.gameObject;
        GameObject unarmedEnemy = unarmedMeleeEnemyPool.GetInstance();
        unarmedEnemy.transform.position = armedEnemy.transform.position;

        playerManager.transform.position = armedEnemy.transform.position;

        armedMeleeEnemyPool.DisableInstance(armedEnemy);

        // IMPORTANT** CHANGE THE NEW ENEMIES HEALTH TO THE OLD ONES ALSO SET HP TO FULL WHEN SPAWNING IN THESE ENEMIES
        quickTimeEventManager.Activate();

        currentHit = null;
        weaponToSteal = null;

        durabilityManager.ActivateDurabilityBar();
    }

    public void StopStealing() {
        currentHit = null;
        weaponToSteal = null;
    }
}
