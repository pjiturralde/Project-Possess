using UnityEngine;

public class MainMenuMusic : MonoBehaviour {
    public AudioSource musicSource;
    public AudioClip music;

    void Start() {
        musicSource.clip = music;
        musicSource.Play();
    }
}
