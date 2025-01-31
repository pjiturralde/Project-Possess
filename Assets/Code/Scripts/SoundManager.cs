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

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] soundList;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(SoundType sound, float volume = 1) {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
