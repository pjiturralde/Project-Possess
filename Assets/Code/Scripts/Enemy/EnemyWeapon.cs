using DG.Tweening;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
    private PlayerManager playerManager;
    private MeleeEnemyBehaviour meleeEnemyBehaviour;
    private SpriteManager spriteManager;
    private SpriteRenderer bodySpriteRenderer;
    private SpriteRenderer shoulderSpriteRenderer;
    private SpriteRenderer weaponSpriteRenderer;
    private Sequence mySequence1;
    private Sequence mySequence2;
    public int weaponIndex; // AXE IS 0, SWORD IS 1, SPEAR IS 2 RANDOMIZE!
    private string[] weaponArray = new string[3];
    private Vector3 weaponOffset;
    private int xDirection;
    private Vector3[] path;

    // ref
    public GameObject sparkleParticles;

    // ACTUAL STATS!!
    public int damage;
    public int durability;
    public int difficulty;
    public bool isShiny = false;

    public bool isInitialized;

    public void Initialize() { // BALANCE HERE LIKE IDK HOW TO BALANCE THIS GARBAGE!
        if (weaponIndex == 0) { // AXE
            weaponOffset = Vector3.zero;

            damage = 25;
            durability = 100;
            difficulty = 1;
        } else if (weaponIndex == 1) { // SWORD
            weaponOffset = new Vector3(-0.045f, 0.02f);

            damage = 25;
            durability = 75;
            difficulty = 1;
        } else if (weaponIndex == 2) { // SPEAR
            weaponOffset = new Vector3(-0.045f, 0.02f);

            damage = 30;
            durability = 50;
            difficulty = 1;
        }

        isShiny = false;

        path = GeneratePartCirclePoints(0.08f, 5);

        spriteManager = SpriteManager.instance;
        playerManager = PlayerManager.instance;
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        meleeEnemyBehaviour = transform.parent.GetComponent<MeleeEnemyBehaviour>();
        bodySpriteRenderer = transform.parent.Find("Body").GetComponent<SpriteRenderer>(); // DO NOT CHANGE THESE NAMES D:
        shoulderSpriteRenderer = transform.parent.Find("Shoulder").GetComponent<SpriteRenderer>();

        // set first position and rotation
        transform.localPosition = new Vector2(0.07f + weaponOffset.x, 0.02f + weaponOffset.y);
        transform.localRotation = Quaternion.Euler(0, 0, 145);

        weaponArray[0] = "EnemyAxe";
        weaponArray[1] = "EnemySword";
        weaponArray[2] = "EnemySpear";

        weaponSpriteRenderer.sprite = spriteManager.GetSprite(weaponArray[weaponIndex]);

        ChangeDirection();
        isInitialized = true;
    }

    public void MakeShiny() {
        GameObject shiny = transform.Find("ShinyParticles").gameObject;
        shiny.SetActive(true);
        isShiny = true;
    }

    public void SetWeapon(int weaponIndex) {
        this.weaponIndex = weaponIndex;
        weaponSpriteRenderer.sprite = spriteManager.GetSprite(weaponArray[weaponIndex]);

        if (weaponIndex == 0) {
            weaponOffset = Vector3.zero;
        } else if (weaponIndex == 1) {
            weaponOffset = new Vector3(-0.045f, 0.02f);
        } else if (weaponIndex == 2) {
            weaponOffset = new Vector3(-0.045f, 0.02f);
        }

        xDirection = GetXDirection();

        transform.localPosition = new Vector2((0.07f + weaponOffset.x) * xDirection, 0.02f + weaponOffset.y);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 + 55 * xDirection));
    }

    private void Awake() {
        isInitialized = false;
    }

    void Start() {
        Initialize();
    }

    public void PlayAttackAnimation(float windUpTime) { // wind up time for le epic attack!
        xDirection = GetXDirection();

        transform.localPosition = new Vector2((0.07f + weaponOffset.x) * xDirection, 0.02f + weaponOffset.y);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 + 55 * xDirection));

        mySequence1 = DOTween.Sequence();
        mySequence1.Append(transform.DOLocalMove(new Vector2(weaponOffset.x * xDirection, 0.07f), windUpTime));

        mySequence2 = DOTween.Sequence();
        mySequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, 90 + 95 * xDirection), windUpTime));

        Invoke(nameof(ReverseZOrder), windUpTime + 0.08f);
        Invoke(nameof(ReverseZOrder), windUpTime + 0.75f);
        Invoke(nameof(UpdateSwingPosition), windUpTime);
    }

    private void UpdateSwingPosition() {
        meleeEnemyBehaviour.UpdateXDirection();
        xDirection = GetXDirection();

        transform.localPosition = new Vector2((0.07f + weaponOffset.x) * xDirection, 0.02f + weaponOffset.y);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 + 55 * xDirection));

        Sequence sequence1 = DOTween.Sequence();

        Vector3[] editedPath = new Vector3[path.Length];

        for (int i = 0; i < path.Length; i++) {
            editedPath[i] = new Vector3((path[i].x + weaponOffset.x) * xDirection, path[i].y + weaponOffset.y, path[i].z);
        }

        sequence1.Append(transform.DOLocalPath(editedPath, 0.2f, PathType.CatmullRom).SetEase(Ease.OutCirc));
        System.Array.Reverse(editedPath);
        sequence1.AppendInterval(0.2f);

        for (int i = 0; i < editedPath.Length; i++) {
            if (i < editedPath.Length - 2) {
                editedPath[i] = editedPath[i];
            } else if (i == editedPath.Length - 2) {
                editedPath[i] = editedPath[i] - new Vector3(0, 0.01f, 0);
            } else {
                editedPath[i] = new Vector3((0.07f + weaponOffset.x) * xDirection, 0.02f + weaponOffset.y, 0);
            }
        }

        sequence1.Append(transform.DOLocalPath(editedPath, 0.6f, PathType.CatmullRom).SetEase(Ease.InOutSine));

        Sequence sequence2 = DOTween.Sequence();

        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, -390 * xDirection), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCirc));
        sequence2.AppendInterval(0.2f);
        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, 390 * xDirection), 0.6f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));
    }

    public void UpdateXDirection() {
        xDirection = GetXDirection();
    }

    public void ChangeDirection() {
        UpdateXDirection();

        DOTween.Kill(transform);

        transform.localPosition = new Vector2((0.15f + weaponOffset.x) * xDirection, weaponOffset.y);
        transform.localRotation = Quaternion.Euler(0, 0, -90 + (80 * xDirection));

        transform.DOLocalMove(new Vector2((0.07f + weaponOffset.x) * xDirection, 0.02f + weaponOffset.y), 0.2f);
        transform.DOLocalRotate(new Vector3(0, 0, 90 + 55 * xDirection), 0.2f);
    }

    private int GetXDirection() { // returns an int pertaining to the direction the enemy is facing
        int xDirection;

        if (!bodySpriteRenderer.flipX) {
            xDirection = 1;
        } else {
            xDirection = -1;
        }

        return xDirection;
    }

    public void ResetZOrder() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        renderer.sortingOrder = 0;
    }

    private void ReverseZOrder() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        int order = renderer.sortingOrder == 0 ? 2 : 0;
        renderer.sortingOrder = order;
    }

    private void SetZOrder(int order) {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        renderer.sortingOrder = order;
    }

    // generates array of vector3s representing part of a circle
    private Vector3[] GeneratePartCirclePoints(float radius, int numSegments) {
        Vector3[] points = new Vector3[numSegments + 1];

        for (int i = 0; i < numSegments + 1; i++) {
            float angle = i * Mathf.PI * 1.1f / numSegments;
            float x = Mathf.Sin(angle) * radius * 2f;
            float y = Mathf.Cos(angle) * radius - 0.01f;
            if (i == numSegments - 1) {
                Debug.Log(angle * Mathf.Rad2Deg);
                Debug.Log(x);
                Debug.Log(y);
            }
            points[i] = new Vector3(x, y, 0);
        }

        return points;
    }
}
