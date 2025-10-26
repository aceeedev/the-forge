using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioSource sfxSource;   // sound effects


    public static void PlayClick()
    {
        sfxSource.Play();
    }
}
