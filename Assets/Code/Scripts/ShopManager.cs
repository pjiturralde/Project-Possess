using NUnit.Framework.Internal;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    public static ShopManager instance { get; private set; }

    private PlayerManager playerManager;

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
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        playerManager = PlayerManager.instance;
    }

    public void SpawnWizard() {
        Vector3 wizardPos = playerManager.transform.position + new Vector3(-4, 4, 0);
        usedPassives = new GameObject[3];
        usedConsumables = new GameObject[2];

        GameObject wizardInstance = Instantiate(wizardPrefab);
        wizardInstance.transform.position = wizardPos;

        for (int i = 0; i < 3; i++) {
            bool reroll = true;

            GameObject passivePrefab = null;

            while (reroll) {
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
            }

            GameObject passiveInstance = Instantiate(passivePrefab);

            passiveInstance.transform.position = wizardPos + new Vector3(6 + i, 0, 0);
        }

        for (int i = 0; i < 2; i++) {
            bool reroll = true;

            GameObject consumablePrefab = null;

            while (reroll) {
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
            }

            GameObject consumableInstance = Instantiate(consumablePrefab);

            consumableInstance.transform.position = wizardPos + new Vector3(3 + i, 0, 0);
        }
    }

    public void DespawnWizard() {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        GameObject wizard = GameObject.FindGameObjectWithTag("Wizard");

        Destroy(wizard);

        for (int i = 0; i < items.Length; i++) {
            Destroy(items[i]);
        }
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
