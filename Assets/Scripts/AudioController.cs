using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource BgmSource;
    public AudioSource soundFx;

    public static AudioController instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }
    
    public void PlaySingle(AudioClip clip)
    {
        soundFx.clip = clip;
        soundFx.Play();
    }

    public void Death(AudioClip clip)
    {
        BgmSource.Stop();
        BgmSource.clip = clip;
        BgmSource.Play();
    }
}
