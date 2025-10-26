using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource; // background music
    public AudioSource sfxSource;   // sound effects


    public void PlayClick()
    {
        sfxSource.Play();
    }
}
