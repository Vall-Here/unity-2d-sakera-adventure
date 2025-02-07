
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("-- Audio Sources --")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    [Header("-- Sound Effects --")]
    public AudioClip backgroundMusic;
    // public AudioClip AttackMelee;
    // public AudioClip AttackRange;




    protected override void Awake() {
        base.Awake();
        bgmSource.clip = backgroundMusic;
        bgmSource.Play();
    }

    public void PlayItemActionSound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // public void playAttack(){
    //     sfxSource.clip = AttackMelee;
    //     sfxSource.PlayOneShot(AttackMelee);
    // }
}
