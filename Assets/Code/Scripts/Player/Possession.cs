using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using UnityEngine;

public class Possession : MonoBehaviour {
    public Material outline;
    public Material thickOutline;
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
    private float possessionCooldown = 0;
    public CinemachineCamera cinemachineCamera;
    public GameObject playerParticles;
    public TextMeshPro possessionTimer;
    public TextMeshPro possessionTimerText;

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
        if (possessionCooldown > 0) {
            possessionCooldown -= Time.deltaTime / Time.timeScale; // just in case ^-^ yeah bud i know it does nothing.. no need to tell me..

            possessionTimer.text = (Mathf.Round(possessionCooldown * 10) / 10).ToString();
        } else {
            if (possessionTimerText.gameObject.activeSelf) {
                possessionTimerText.gameObject.SetActive(false);
                possessionTimer.gameObject.SetActive(false);
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (!playerStats.isPossessing && weaponToSteal == null) {
                if (currentHit != hit.collider) {

                    if (currentHit != null) {
                        if (currentHit.CompareTag("FreeWeapon")) {
                            currentWeaponSpriteRenderer = currentHit.GetComponent<SpriteRenderer>();

                            currentWeaponSpriteRenderer.material = defaultMaterial;
                        } else {
                            foreach (Transform child in currentHit.transform) {
                                if (child.CompareTag("EnemyWeapon")) {
                                    currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();

                                    currentWeaponSpriteRenderer.material = defaultMaterial;
                                    break;
                                }
                            }
                        }
                    }

                    currentHit = hit.collider;

                    if (currentHit != null) {
                        SpriteRenderer enemyWeaponSpriteRenderer;

                        if (currentHit.CompareTag("FreeWeapon")) {
                            enemyWeaponSpriteRenderer = currentHit.GetComponent<SpriteRenderer>();

                            defaultMaterial = enemyWeaponSpriteRenderer.material;
                            enemyWeaponSpriteRenderer.material = thickOutline;
                        } else {
                            if (hit.collider.CompareTag("ArmedEnemy")) {
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
                }

                if (currentHit != null && Input.GetMouseButtonDown(0) && (currentHit.CompareTag("ArmedEnemy") || currentHit.CompareTag("FreeWeapon"))) {
                    if (currentHit.CompareTag("FreeWeapon")) {
                        currentWeaponSpriteRenderer = currentHit.GetComponent<SpriteRenderer>();

                        currentWeaponSpriteRenderer.material = defaultMaterial;
                    } else {
                        foreach (Transform child in currentHit.transform) {
                            if (child.CompareTag("EnemyWeapon")) {
                                currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();

                                currentWeaponSpriteRenderer.material = defaultMaterial;
                                break;
                            }
                        }
                    }

                    var composer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();

                    weaponToSteal = currentHit;
                    cinemachineCamera.Follow = weaponToSteal.transform;
                    composer.Damping.x = 0.3f;
                    composer.Damping.y = 0.3f;
                    Time.timeScale = 0.3f;
                    quickTimeEventManager.Activate(0);
                }
            }
        }
    }

    public void StealWeapon() {
        playerStats.isPossessing = true;
        playerStats.GetComponent<CircleCollider2D>().enabled = false;
        playerStats.transform.Find("Body").GetComponent<SpriteRenderer>().enabled = false; // pls don't change name of body

        int weaponIndex = 0;
        int damage = 0;
        int durability = 0;
        bool isShiny = false;

        if (weaponToSteal.CompareTag("FreeWeapon")) {
            currentWeaponSpriteRenderer = currentHit.GetComponent<SpriteRenderer>();
            FreeWeaponStats freeWeapon = currentHit.GetComponent<FreeWeaponStats>();

            weaponIndex = freeWeapon.weaponIndex;
            damage = freeWeapon.damage;
            durability = freeWeapon.durability;
            isShiny = freeWeapon.isShiny;

            currentWeaponSpriteRenderer.material = defaultMaterial;

            playerManager.transform.position = weaponToSteal.transform.position;

            Destroy(weaponToSteal.gameObject);
        } else {
            foreach (Transform child in weaponToSteal.transform) {
                if (child.CompareTag("EnemyWeapon")) {
                    currentWeaponSpriteRenderer = child.GetComponent<SpriteRenderer>();
                    EnemyWeapon enemyWeapon = child.GetComponent<EnemyWeapon>();

                    weaponIndex = enemyWeapon.weaponIndex;
                    damage = enemyWeapon.damage;
                    durability = enemyWeapon.durability;
                    isShiny = enemyWeapon.isShiny;

                    currentWeaponSpriteRenderer.material = defaultMaterial;

                    playerManager.transform.position = enemyWeapon.transform.position;
                    break;
                }
            }

            MeleeEnemyBehaviour meleeEnemyBehaviour = weaponToSteal.GetComponent<MeleeEnemyBehaviour>();
            meleeEnemyBehaviour.CancelInvoke();

            GameObject armedEnemy = weaponToSteal.gameObject;
            GameObject unarmedEnemy = unarmedMeleeEnemyPool.GetInstance();
            unarmedEnemy.transform.position = armedEnemy.transform.position;

            // copy over all their stats yo!
            unarmedEnemy.GetComponent<EnemyStats>().Health = armedEnemy.GetComponent<EnemyStats>().Health;

            armedMeleeEnemyPool.DisableInstance(armedEnemy);
        }

        GameObject playerWeapon = null;

        if (weaponIndex == 0) {
            playerWeapon = Instantiate(playerAxe, playerManager.transform);
        } else if (weaponIndex == 1) {
            playerWeapon = Instantiate(playerSword, playerManager.transform);
        } else {
            playerWeapon = Instantiate(playerSpear, playerManager.transform);
        }

        WeaponStats weaponStats = null;

        if (playerWeapon != null) {
            weaponStats = playerWeapon.GetComponent<WeaponStats>();
        }

        weaponStats.Damage = damage;
        weaponStats.Durability = durability;
        weaponStats.MaxDurability = durability;
        weaponStats.isShiny = isShiny;
        weaponStats.TriggerInvulnerability(0.5D);

        // IMPORTANT** CHANGE THE NEW ENEMIES HEALTH TO THE OLD ONES ALSO SET HP TO FULL WHEN SPAWNING IN THESE ENEMIES
/*        quickTimeEventManager.Activate();*/

        currentHit = null;
        weaponToSteal = null;

        var composer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();

        cinemachineCamera.Follow = playerManager.transform;
        composer.Damping.x = 0;
        composer.Damping.y = 0;
        Time.timeScale = 1;

        playerParticles.SetActive(false);
        durabilityManager.ActivateDurabilityBar();
    }

    public void StopStealing() {
        var composer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        possessionCooldown = 5; // 5 second cooldown after attempting :] #punishment!

        possessionTimer.gameObject.SetActive(true);
        possessionTimerText.gameObject.SetActive(true);

        cinemachineCamera.Follow = playerManager.transform;
        composer.Damping.x = 0;
        composer.Damping.y = 0;
        Time.timeScale = 1;

        currentHit = null;
        weaponToSteal = null;
    }
}
