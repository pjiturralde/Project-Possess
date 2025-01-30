using UnityEngine;

public class PlayerController : MonoBehaviour {
    public SpriteRenderer bodySpriteRenderer;
    private float moveSpeed;
    Vector2 inputDir;
    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        bodySpriteRenderer = transform.Find("Body").GetComponent<SpriteRenderer>(); // dont change name or it brokey!
    }

    void Update() {
        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = inputDir.normalized;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (moveSpeed != GetComponent<PlayerStats>().MovementSpeed) {
            moveSpeed = GetComponent<PlayerStats>().MovementSpeed;
        }

        if (mousePos.x < transform.position.x) {
            bodySpriteRenderer.flipX = true;
        } else {
            bodySpriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate() {
        rb.linearVelocity = inputDir * (moveSpeed + GetComponent<PlayerStats>().ExtraSpeed);
    }
}
