using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    private ShopManager shopManager;
    private Collider2D currentHit;
    private SpriteRenderer currentItemSpriteRenderer;

    public Material outline;
    private Material defaultMaterial;

    public Dictionary<string, int> allItems; // represents name and max number of allowed items to have
    private List<string> currentItems;
    private List<string> allItemNames;

    public GameObject itemSummary;
    public TextMeshPro itemWorthText;
    public TextMeshPro itemDescriptionText;
    public TextMeshPro itemTitleText;
    public GameObject damageIndicator;


    void Start() {
        currentItemSpriteRenderer = null;
        currentHit = null;
        currentItems = new List<string>();
        playerManager = PlayerManager.instance;
        playerStats = playerManager.GetComponent<PlayerStats>();
        shopManager = ShopManager.instance;

        allItems = new Dictionary<string, int> {
            {"SharpeningStone", 5},
            {"HarpyFeather", 10},
            {"RabbitFoot", 5},
            {"GlassBlade", 1},
            {"FrighteningFlame", 1},
            {"PetrifyingPebble", 1},
            {"TurtleShell", 5},
            {"SpareSkull", 1},
            {"VampireFangs", 1},
            {"SwiftBoot", 10},
            {"EscapePlan", 1},
            {"ShinyShamrock", 12},
            {"ForgedDeed", 1},
            {"MilitaryMagnet", 12},
            {"SpiritConductor", 2},
            {"TransmutationDevice", 1}
        };

        allItemNames = new List<string>(allItems.Keys);
    }

    void Update() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (currentHit != hit.collider) {

            if (currentHit != null) {
                if (currentHit.CompareTag("Item") || currentHit.CompareTag("Wizard")) {
                    currentItemSpriteRenderer = currentHit.GetComponent<SpriteRenderer>();

                    currentItemSpriteRenderer.material = defaultMaterial;
                }
            }

            currentHit = hit.collider;

            if (currentHit != null) {
                SpriteRenderer enemyWeaponSpriteRenderer;

                if (hit.collider.CompareTag("Item") || hit.collider.CompareTag("Wizard")) {
                    enemyWeaponSpriteRenderer = hit.collider.GetComponent<SpriteRenderer>();

                    defaultMaterial = enemyWeaponSpriteRenderer.material;
                    enemyWeaponSpriteRenderer.material = outline;
                }
            }
        }

        if (hit.collider == null) {
            if (itemSummary.activeSelf) {
                itemSummary.SetActive(false);
            }
        } else {
            if (hit.collider.CompareTag("Item")) {
                if (!itemSummary.activeSelf) {
                    itemSummary.SetActive(true);
                }
                itemSummary.transform.position = mousePos + new Vector2(0, 2);

                Item item = hit.collider.GetComponent<Item>();

                itemWorthText.text = item.Cost.ToString();
                itemDescriptionText.text = item.Description.ToString();

                string itemTitle = Regex.Replace(item.Name, "(?<!^)([A-Z])", " $1");

                itemTitleText.text = itemTitle;
            }
        }

        if (currentHit != null && Input.GetMouseButtonDown(0)) {
            if (currentHit.CompareTag("Wizard")) {
                shopManager.DespawnWizard();
            }

            if (currentHit.CompareTag("Item")) {
                Item item = currentHit.GetComponent<Item>();

                string itemName = item.Name;

                if (item.Cost <= playerStats.Money || HasItem("ForgedDeed")) {
                    currentItemSpriteRenderer = currentHit.GetComponent<SpriteRenderer>();

                    currentItemSpriteRenderer.material = defaultMaterial;

                    if (HasItem("ForgedDeed")) {
                        RemoveItem("ForgedDeed"); 
                    } else {
                        playerStats.Money -= item.Cost;
                    }

                    if (HasItem("TransmutationDevice")) {
                        RemoveItem("TransmutationDevice");

                        bool ableToEquip = false;

                        int index = 0;

                        while (ableToEquip == false) {
                            int randomIndex = Random.Range(0, allItemNames.Count);

                            if (CountItemOccurrences(allItemNames[randomIndex]) < allItems[allItemNames[randomIndex]]) {
                                ableToEquip = true;
                                index = randomIndex;
                            }
                        }

                        itemName = allItemNames[index];
                    }

                    currentItems.Add(itemName);

                    if (itemName == "SharpeningStone") {
                        playerStats.DamageMultiplier += 0.1f; // adds 10% damage :]
                    } else if (itemName == "HarpyFeather") {
                        playerStats.AttackRate += 0.05f; // lowers cooldown by 5%
                    } else if (itemName == "RabbitFoot") {
                        playerStats.CritChance += 3; // 3% increase in crit chance
                    } else if (itemName == "GrassBlade") {
                        playerStats.DamageMultiplier += 0.5f; // adds a whopping 50% damage O::: but careful you get hurt when you attack now!
                    } else if (itemName == "SwiftBoot") {
                        playerStats.MovementSpeed += 0.1f; // flat increase of 0.1 :0:
                    } else if (itemName == "TurtleShell") {
                        playerStats.Defense += 0.1f; // 10% damage reduction (in weapon form) :]
                        playerStats.MovementSpeed -= 0.05f; // flat decrease of 0.05 ;)
                    } else if (itemName == "ShinyShamrock") {
                        playerStats.Luck += 2; // adds 2% higher chance for gold to drop :]
                    } else if (itemName == "MilitaryMagnet") {
                        playerStats.WeaponLuck += 2; // adds 2% chance for weapon to drop :D
                    }

                    Destroy(currentHit.gameObject);
                } else {
                    GameObject dmgIndInstance = Instantiate(damageIndicator);
                    dmgIndInstance.transform.position = mousePos;

                    TextMeshPro label = dmgIndInstance.GetComponent<TextMeshPro>();
                    label.text = "You're Broke";
                    label.color = Color.red;
                }
            }
        }
    }

    public int CountItemOccurrences(string name) {
        int count = 0;

        for (int i = 0; i < currentItems.Count; i++) {
            if (currentItems[i] == name) {
                count++;
            }
        }

        return count;
    }

    public bool HasItem(string name) {
        bool hasItem = false;

        for (int i = 0; i < currentItems.Count; i++) {
            if (currentItems[i] == name) {
                hasItem = true;
                break;
            }
        }

        return hasItem;
    }

    public void RemoveItem(string name) {
        currentItems.Remove(name);
    }
}
