using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{
    public void MuteToggle(bool muted)
    { 
        if (muted) 
        {
            AudioListener.volume = 1;
        }
        else
        {
        AudioListener.volume = 1;
        }
    }
}
