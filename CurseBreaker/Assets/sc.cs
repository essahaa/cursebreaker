using UnityEngine;

public class SoundController : MonoBehaviour
{
    private bool isMuted = false;

    public void ToggleMuteSounds(bool isMuted)
    {
        this.isMuted = isMuted;

        if (isMuted)
        {
            DeafenSounds();
        }
        else
        {
            UnDeafenSounds();
        }
    }

    private void DeafenSounds()
    {
        FindObjectOfType<AudioManager>().DeafenSound("youfail");
        FindObjectOfType<AudioManager>().DeafenSound("musa");
        FindObjectOfType<AudioManager>().DeafenSound("winner");
        FindObjectOfType<AudioManager>().DeafenSound("riddedred");
        FindObjectOfType<AudioManager>().DeafenSound("liik");
    }

    private void UnDeafenSounds()
    {
        FindObjectOfType<AudioManager>().UnDeafenSound("youfail");
        FindObjectOfType<AudioManager>().UnDeafenSound("musa");
        FindObjectOfType<AudioManager>().UnDeafenSound("winner");
        FindObjectOfType<AudioManager>().UnDeafenSound("riddedred");
        FindObjectOfType<AudioManager>().UnDeafenSound("liik");
    }
}
