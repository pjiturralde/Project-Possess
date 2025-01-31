using UnityEngine;

public enum SoundType {
    AXE,
    BONE_IMPACT,
    METAL_IMPACT,
    SWORD,
    SPEAR,
    PROJECTILE
}

public class SoundManager : MonoBehaviour {
    public static SoundManager instance { get; private set; }

    public AudioSource SFXSource;
    public AudioSource musicSource;

    public AudioClip music1;
    public AudioClip music2;
    public AudioClip music3;
    [SerializeField] private AudioClip[] soundList;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        musicSource.clip = music1;
        musicSource.Play();
    }

    public static void PlaySound(SoundType sound, float pitchBase, float volume = 1) {
        instance.SFXSource.pitch = pitchBase + (float)Random.Range(-10, 15) / 100;
        instance.SFXSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
