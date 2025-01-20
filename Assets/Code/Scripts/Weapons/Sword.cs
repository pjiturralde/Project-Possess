using DG.Tweening;
using UnityEngine;

public class Sword : MonoBehaviour {
    private Transform player;
    private int slashDirection;
    private Vector3[] path = new Vector3[6];

    void Start() {
        slashDirection = -1;
        player = transform.parent;

        path = new Vector3[] {
            new Vector3(0.5f, 0, 0),
            new Vector3(1f, 1.5f, 0),
            new Vector3(0, 2, 0),
            new Vector3(-1f, 1.5f, 0),
            new Vector3(-0.5f, 0, 0),
            new Vector3(0, 0, 0)
        };
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            transform.DOLocalPath(path, 0.6f, PathType.CatmullRom);
            transform.localEulerAngles = new Vector3(0, 0, 45 * slashDirection);
            transform.DORotate(new Vector3(0, 0, -90 * slashDirection), 0.6f, RotateMode.LocalAxisAdd);
            for (int i = 0; i < path.Length; i++) {
                path[i] = new Vector3(path[i].x * -1, path[i].y, path[i].z);
            }
            slashDirection *= -1;
        }
    }
}
