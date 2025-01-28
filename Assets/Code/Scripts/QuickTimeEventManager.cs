using UnityEngine;

public class QuickTimeEventManager : MonoBehaviour {
    public static QuickTimeEventManager instance { get; private set; }
    public Transform pointA; // Reference to the starting point
    public Transform pointB; // Reference to the ending point
    public RectTransform safeZone; // Reference to the safe zone RectTransform
    public float moveSpeed = 100f; // Speed of the pointer movement

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
    }

    public void Activate() {
        foreach (Transform t in transform) {

        }
    }

    void Update() {
        if (isActive) {
            // Move the pointer towards the target position
            pointerTransform.localPosition = Vector3.MoveTowards(pointerTransform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

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
            Debug.Log("Success!");
        } else {
            Debug.Log("Fail!");
        }
    }
}
