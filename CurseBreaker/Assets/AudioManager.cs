using UnityEngine.Audio;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    void Awake()
    {

        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
        }
    }

    // Update is called once per frame
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;

        s.source.Play();
    }

    
    public void MuteSound(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // Set the volume to zero to mute the sound
        s.source.volume = 0f;
    }

    public void UnMuteSound(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // Set the volume to zero to mute the sound
        s.source.volume = 0.2f;
    }
    //FindObjectOfType<AudioManager>().StopPlaying("sound string name");


}

