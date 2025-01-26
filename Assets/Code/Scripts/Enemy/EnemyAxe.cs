using DG.Tweening;
using UnityEditor.EditorTools;
using UnityEngine;

public class EnemyAxe : MonoBehaviour {
    private PlayerManager playerManager;
    private MeleeEnemyBehaviour meleeEnemyBehaviour;
    private SpriteRenderer enemyRenderer;
    private Vector3[] path;

    void Start() {
        path = GeneratePartCirclePoints(0.11f, 5);
        playerManager = PlayerManager.instance;
        meleeEnemyBehaviour = transform.parent.GetComponent<MeleeEnemyBehaviour>();
        enemyRenderer = meleeEnemyBehaviour.GetComponent<SpriteRenderer>();

        // set first position and rotation
        transform.localPosition = new Vector2(0.07f, 0.02f);
        transform.localRotation = Quaternion.Euler(0, 0, 145);
    }

    public void PlayAttackAnimation() {
        transform.localPosition = new Vector2(0, 0.05f);
        transform.localRotation = Quaternion.Euler(0, 0, 145);

        Sequence sequence1 = DOTween.Sequence();
        sequence1.Append(transform.DOLocalPath(path, 0.4f, PathType.CatmullRom));
        System.Array.Reverse(path);
        sequence1.Append(transform.DOLocalPath(path, 0.4f, PathType.CatmullRom));
        System.Array.Reverse(path);

        Sequence sequence2 = DOTween.Sequence();
        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, -370), 0.4f, RotateMode.LocalAxisAdd));
        sequence2.Append(transform.DOLocalRotate(new Vector3(0, 0, 370), 0.4f, RotateMode.LocalAxisAdd));

        Invoke(nameof(ReverseZOrder), 0.2f);
        Invoke(nameof(ReverseZOrder), 0.45f);
    }

    public void ChangeDirection() {
        int xDirection;

        DOTween.Kill(transform);
        if (!enemyRenderer.flipX) {
            xDirection = 1;
        } else {
            xDirection = -1;
        }

        transform.localPosition = new Vector2(0.15f * xDirection, 0);
        transform.localRotation = Quaternion.Euler(0, 0, -90 + (90 * xDirection));

        transform.DOLocalMove(new Vector2(0.07f * xDirection, 0.02f), 0.2f);
        transform.DOLocalRotate(new Vector3(0, 0, 90 + 55 * xDirection), 0.2f);
    }

    private void ReverseZOrder() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        int order = renderer.sortingOrder == 0 ? 1 : 0;
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
            float angle = i * Mathf.PI * 1.5f / numSegments;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
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
