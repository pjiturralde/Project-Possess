using TMPro;
using UnityEngine;

public class QuickTimeEventManager : MonoBehaviour {
    public static QuickTimeEventManager instance { get; private set; }
    public Transform pointA; // Reference to the starting point
    public Transform pointB; // Reference to the ending point
    public RectTransform safeZone; // Reference to the safe zone RectTransform
    private float moveSpeed = 300f; // Speed of the pointer movement
    private int difficulty = 0; // 0 is absolute easiest noobertson! 1 and up is spooky!
    private int numWinsNeeded = 1;
    private int numWins = 0;
    private float baseMoveSpeed = 0;
    private PlayerManager playerManager;
    private Possession possessionScript;
    private float timeRemaining; // time remaining to do quicktime event!

    private bool isActive;
    public RectTransform pointerTransform;
    public TextMeshPro timerText;
    private Vector3 targetPosition;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        isActive = false;
        targetPosition = pointB.localPosition;
        playerManager = PlayerManager.instance;
        possessionScript = playerManager.GetComponent<Possession>();
    }

    public void Activate(int difficulty) {
        numWins = 0;
        this.difficulty = difficulty;

        if (difficulty == 0) { // zero is the noobest difficulty
            baseMoveSpeed = 300f;
            safeZone.sizeDelta = new Vector2(50, safeZone.rect.height);
            numWinsNeeded = 2;
        } else if (difficulty == 1) {
            baseMoveSpeed = 450f;
            safeZone.sizeDelta = new Vector2(35, safeZone.rect.height);
            numWinsNeeded = 3;
        } else if (difficulty == 2) {
            baseMoveSpeed = 600f;
            safeZone.sizeDelta = new Vector2(25, safeZone.rect.height);
            numWinsNeeded = 3;
        }

        timeRemaining = 10.0f;
        safeZone.localPosition = new Vector2(Random.Range(pointA.localPosition.x + safeZone.sizeDelta.x / 2 + 40, pointB.localPosition.x - safeZone.sizeDelta.x / 2 - 40), safeZone.localPosition.y);
        pointerTransform.localPosition = new Vector2(Random.Range(pointA.localPosition.x + pointerTransform.sizeDelta.x / 2 + 40, pointB.localPosition.x - pointerTransform.sizeDelta.x / 2 - 40), safeZone.localPosition.y);

        foreach (Transform element in transform) {
            if (!element.gameObject.activeSelf) {
                element.gameObject.SetActive(true);
            }
        }

        isActive = true;
    }

    public void Deactivate() {
        foreach (Transform element in transform) {
            if (element.gameObject.activeSelf) {
                element.gameObject.SetActive(false);
            }
        }

        isActive = false;
    }

    void Update() {
        if (isActive) {
            timeRemaining -= Time.deltaTime / Time.timeScale;

            if (timeRemaining <= 0) {
                possessionScript.StopStealing();
                Deactivate();
            } 

            timerText.text = (Mathf.Round(timeRemaining * 10) / 10).ToString();

            // Move the pointer towards the target position
            pointerTransform.localPosition = Vector3.MoveTowards(pointerTransform.localPosition, targetPosition, moveSpeed * Time.deltaTime / Time.timeScale);

            // Change direction if the pointer reaches one of the points
            if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f) {
                moveSpeed = baseMoveSpeed + Random.Range(-100, 200);
                targetPosition = pointB.localPosition;
            } else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f) {
                moveSpeed = baseMoveSpeed + Random.Range(-100, 200);
                targetPosition = pointA.localPosition;
            }

            // Check for input
            if (Input.GetKeyDown(KeyCode.Space)) {
                CheckSuccess();
            }
        }
    }

    void CheckSuccess() {
        numWins++;

        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null)) {
            if (numWins == numWinsNeeded) {
                possessionScript.StealWeapon();
                Deactivate();
            } else {
                safeZone.localPosition = new Vector2(Random.Range(pointA.localPosition.x + safeZone.sizeDelta.x / 2 + 40, pointB.localPosition.x - safeZone.sizeDelta.x / 2 - 40), safeZone.localPosition.y);
            }
        } else {
            possessionScript.StopStealing();
            Deactivate();
        }
    }
}
