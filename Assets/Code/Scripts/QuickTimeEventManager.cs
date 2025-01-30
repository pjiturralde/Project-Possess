using UnityEngine;

public class QuickTimeEventManager : MonoBehaviour {
    public static QuickTimeEventManager instance { get; private set; }
    public Transform pointA; // Reference to the starting point
    public Transform pointB; // Reference to the ending point
    public RectTransform safeZone; // Reference to the safe zone RectTransform
    private float moveSpeed = 300f; // Speed of the pointer movement
    private int difficulty = 0; // 0 is absolute easiest noobertson! 1 and up is spooky!
    private PlayerManager playerManager;
    private Possession possessionScript;

    private bool isActive;
    public RectTransform pointerTransform;
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
        this.difficulty = difficulty;

        if (difficulty == 0) {
            safeZone.sizeDelta = new Vector2(50, safeZone.rect.height);
        }

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
            // Move the pointer towards the target position
            pointerTransform.localPosition = Vector3.MoveTowards(pointerTransform.localPosition, targetPosition, moveSpeed * Time.deltaTime / Time.timeScale);

            // Change direction if the pointer reaches one of the points
            if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f) {
                targetPosition = pointB.localPosition;
            } else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f) {
                targetPosition = pointA.localPosition;
            }

            // Check for input
            if (Input.GetKeyDown(KeyCode.Space)) {
                CheckSuccess();
            }
        }
    }

    void CheckSuccess() {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null)) {
            possessionScript.StealWeapon();
            Deactivate();
        } else {
            possessionScript.StopStealing();
            Deactivate();
        }
    }
}
