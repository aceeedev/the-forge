using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager inst;

    public AudioSource sfxSource;   // sound effects

    private float defaultSFXPitch;

    void Awake()
    {
        // live laugh love singleton pattern
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        defaultSFXPitch = sfxSource.pitch;
    }


    public void PlayClick()
    {
        sfxSource.pitch = Random.Range(defaultSFXPitch * 0.9f, defaultSFXPitch * 1.1f);

        sfxSource.Play();
    }
}
