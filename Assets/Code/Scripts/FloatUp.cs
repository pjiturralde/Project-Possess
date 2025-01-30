using UnityEngine;

public class FloatUp : MonoBehaviour {
    void Start() {
        Invoke(nameof(DestroyText), 0.5f);
    }

    // Update is called once per frame
    void Update() {
        transform.position = transform.position + new Vector3(0, 1, 0) * Time.deltaTime;
    }

    private void DestroyText() {
        Destroy(gameObject);
    }
}
