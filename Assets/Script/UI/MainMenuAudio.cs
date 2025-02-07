using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    [Header("-- Audio Sources --")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    [Header("-- Sound Effects --")]
    public AudioClip backgroundMusic;

    private void Awake() {
        bgmSource.clip = backgroundMusic;
        bgmSource.Play();
    
    }

}


