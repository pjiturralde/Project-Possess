using UnityEngine;

public class ScrollMenu : MonoBehaviour {
    private int mapSizeX;
    private int mapSizeY;

    void Start () {
        mapSizeX = 132;
        mapSizeY = 87;
    }

    void Update() {
        transform.position += new Vector3(1, 0, 0) * Time.deltaTime;

        Debug.Log(transform.position.x);

        if (transform.position.x >= mapSizeX / 2) {
            transform.position = new Vector3(transform.position.x - mapSizeX, transform.position.y, -10);
        }

        if (transform.position.x <= -mapSizeX / 2) {
            transform.position = new Vector3(transform.position.x + mapSizeX, transform.position.y, -10);
        }
    }
}
