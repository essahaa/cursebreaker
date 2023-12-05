using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public string description;

    public AudioClip clip;
    public float duration;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;

    [HideInInspector]
    public bool isPlaying; // Flag to track whether the sound is currently playing
}
