using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour {
    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }
}
