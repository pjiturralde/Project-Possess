using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed;
    Vector2 inputDir;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = inputDir.normalized;

        rb.linearVelocity = inputDir * moveSpeed;
    }
}
