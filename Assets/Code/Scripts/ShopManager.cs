using NUnit.Framework.Internal;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    public static ShopManager instance { get; private set; }

    private PlayerManager playerManager;
    private WaveManager waveManager;

    public GameObject wizardPrefab;
    public GameObject[] passivePrefabs;
    public GameObject[] consumablePrefabs;
    public GameObject healthPotion;
    private GameObject[] usedPassives;
    private GameObject[] usedConsumables;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        playerManager = PlayerManager.instance;
        waveManager = WaveManager.instance;
    }

    public void SpawnWizard() {
        Vector3 wizardPos = playerManager.transform.position + new Vector3(-4, 4, 0);
        usedPassives = new GameObject[3];
        usedConsumables = new GameObject[2];

        GameObject wizardInstance = Instantiate(wizardPrefab);
        wizardInstance.transform.position = wizardPos;

        GameObject healthPotionInstance = Instantiate(healthPotion);

        healthPotionInstance.transform.position = wizardPos + new Vector3(10, 0, 0);

        Item healthItem = healthPotionInstance.GetComponent<Item>();
        healthItem.Cost += 5 * (waveManager.waveNumber - 1);



        for (int i = 0; i < 3; i++) {
            bool reroll = true;

            GameObject passivePrefab = null;

            int timesRolled = 0;

            while (reroll) {
                timesRolled++;
                int randomItem = Random.Range(0, passivePrefabs.Length);

                Item item = passivePrefabs[randomItem].GetComponent<Item>();

                ItemManager itemManager = playerManager.GetComponent<ItemManager>();

                if (!HasItem(passivePrefabs[randomItem], usedPassives)) {
                    if (itemManager.CountItemOccurrences(item.Name) < itemManager.allItems[item.Name]) {
                        passivePrefab = passivePrefabs[randomItem];
                        usedPassives[i] = passivePrefab;
                        reroll = false;
                    }
                }

                if (timesRolled >= 10) {
                    break;
                }
            }

            if (timesRolled >= 10) {
                break;
            }

            GameObject passiveInstance = Instantiate(passivePrefab);

            passiveInstance.transform.position = wizardPos + new Vector3(6 + i, 0, 0);

            Item passiveItem = passiveInstance.GetComponent<Item>();
            passiveItem.Cost += 5 * (waveManager.waveNumber - 1);
        }

        int timesRolled2 = 0;

        for (int i = 0; i < 2; i++) {
            bool reroll = true;

            GameObject consumablePrefab = null;

            while (reroll) {
                timesRolled2++;
                int randomItem = Random.Range(0, consumablePrefabs.Length);

                Item item = consumablePrefabs[randomItem].GetComponent<Item>();

                ItemManager itemManager = playerManager.GetComponent<ItemManager>();

                if (!HasItem(consumablePrefabs[randomItem], usedConsumables)) {
                    if (itemManager.CountItemOccurrences(item.Name) < itemManager.allItems[item.Name]) {
                        consumablePrefab = consumablePrefabs[randomItem];
                        usedConsumables[i] = consumablePrefab;
                        reroll = false;
                    }
                }

                if (timesRolled2 >= 10) {
                    break;
                }
            }

            if (timesRolled2 >= 10) {
                break;
            }

            GameObject consumableInstance = Instantiate(consumablePrefab);

            consumableInstance.transform.position = wizardPos + new Vector3(3 + i, 0, 0);

            Item consumableItem = consumableInstance.GetComponent<Item>();
            consumableItem.Cost += 5 * (waveManager.waveNumber - 1);
        }
    }

    public void DespawnWizard() {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        GameObject wizard = GameObject.FindGameObjectWithTag("Wizard");

        Destroy(wizard);

        for (int i = 0; i < items.Length; i++) {
            Destroy(items[i]);
        }

        GameObject[] weapons = GameObject.FindGameObjectsWithTag("FreeWeapon");

        for (int i = 0; i < weapons.Length; i++) {
            Destroy(weapons[i]);
        }

        waveManager.isInRecess = false;
    }

    private bool HasItem(GameObject prefab, GameObject[] currentItems) {
        bool hasItem = false;

        for (int i = 0; i < currentItems.Length; i++) {
            if (currentItems[i] == prefab) {
                hasItem = true;
                break;
            }
        }

        return hasItem;
    }
}
