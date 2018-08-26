using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    public AudioClip clipBeginning, clipChomp, clipDeath, clipEatGhost;

    private AudioSource audio;

    public static AudioManager _singleton
    {
        get
        {
            if (singleton == null) { singleton = GetSingleton(); }
            return singleton;
        }
    }
    private static AudioManager singleton;

    static AudioManager GetSingleton() { return FindObjectOfType<AudioManager>(); }

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public static void PlayAudio(AudioClip clip)
    {
        _singleton.audio.PlayOneShot(clip);
    }

}
