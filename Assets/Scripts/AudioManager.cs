using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip pouringClip;
    public AudioClip doorClip;
    public AudioClip backgroundMusic;
    public AudioClip analyzerStartClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}

