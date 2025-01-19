using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed;
    Vector2 inputDir;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = inputDir.normalized;
    }

    private void FixedUpdate() {
        rb.linearVelocity = inputDir * moveSpeed;
    }
}
