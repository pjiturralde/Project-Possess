using DG.Tweening;
using UnityEditor.EditorTools;
using UnityEngine;

public class EnemyAxe : MonoBehaviour {
    private PlayerManager playerManager;
    private MeleeEnemyBehaviour meleeEnemyBehaviour;
    private SpriteRenderer bodySpriteRenderer;
    private SpriteRenderer shoulderSpriteRenderer;
    private Vector3[] path;

    void Start() {
        path = GeneratePartCirclePoints(0.08f, 5);

        playerManager = PlayerManager.instance;
        meleeEnemyBehaviour = transform.parent.GetComponent<MeleeEnemyBehaviour>();
        bodySpriteRenderer = transform.parent.Find("Body").GetComponent<SpriteRenderer>(); // DO NOT CHANGE THESE NAMES D:
        shoulderSpriteRenderer = transform.parent.Find("Shoulder").GetComponent<SpriteRenderer>();

        // set first position and rotation
        transform.localPosition = new Vector2(0.07f, 0.02f);
        transform.localRotation = Quaternion.Euler(0, 0, 145);
    }

    public void PlayAttackAnimation(float windUpTime) { // wind up time for le epic attack!
        int xDirection = GetXDirection();

        transform.localPosition = new Vector2(0.07f * xDirection, 0.02f);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 + 55 * xDirection));

        Vector3[] editedPath = new Vector3[path.Length];

        for (int i = 0; i < path.Length; i++) {
            editedPath[i] = new Vector3(path[i].x * xDirection, path[i].y, path[i].z);
        }

        Sequence sequence1 = DOTween.Sequence();
        sequence1.Append(transform.DOLocalMove(new Vector2(0, 0.07f), windUpTime));
        sequence1.Append(transform.DOLocalPath(editedPath, 0.2f, PathType.CatmullRom).SetEase(Ease.OutCirc));
        System.Array.Reverse(editedPath);
        sequence1.AppendInterval(0.2f);

        for (int i = 0; i < editedPath.Length; i++) {
            if (i < editedPath.Length - 2) {
                editedPath[i] = editedPath[i];
            } else if (i == editedPath.Length - 2) {
                editedPath[i] = editedPath[i] - new Vector3(0, 0.01f, 0);
            } else {
                editedPath[i] = new Vector3(0.07f * xDirection, 0.02f, 0);
            }
        }

        sequence1.Append(transform.DOLocalPath(editedPath, 0.6f, PathType.CatmullRom).SetEase(Ease.InOutSine));

        Sequence sequence2 = DOTween.Sequence();
        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, 90 + 95 * xDirection), windUpTime));
        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, -380 * xDirection), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCirc));
        sequence2.AppendInterval(0.2f);
        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, 380 * xDirection), 0.6f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));

        Invoke(nameof(ReverseZOrder), windUpTime + 0.08f);
        Invoke(nameof(ReverseZOrder), windUpTime + 0.75f);
    }

    public void ChangeDirection() {
        int xDirection = GetXDirection();

        DOTween.Kill(transform);

        transform.localPosition = new Vector2(0.15f * xDirection, 0);
        transform.localRotation = Quaternion.Euler(0, 0, -90 + (80 * xDirection));

        transform.DOLocalMove(new Vector2(0.07f * xDirection, 0.02f), 0.2f);
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
