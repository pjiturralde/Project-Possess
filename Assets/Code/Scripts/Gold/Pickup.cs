using UnityEngine;

public class Pickup : MonoBehaviour {
    public int value;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            PlayerStats player = collision.GetComponent<PlayerStats>();

            player.AddMoney(value);
            Destroy(gameObject);
        }
    }
}
