using UnityEngine;
public class Soundcontroller : MonoBehaviour
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

    public void DeafenSounds()
    {
        FindObjectOfType<AudioManager>().DeafenSound("youfail");
        FindObjectOfType<AudioManager>().DeafenSound("musa");
        FindObjectOfType<AudioManager>().DeafenSound("winner");
        FindObjectOfType<AudioManager>().DeafenSound("riddedred");
        FindObjectOfType<AudioManager>().DeafenSound("liik");
    }

    public void UnDeafenSounds()
    {
        FindObjectOfType<AudioManager>().UnDeafenSound("youfail");
        FindObjectOfType<AudioManager>().UnDeafenSound("musa");
        FindObjectOfType<AudioManager>().UnDeafenSound("winner");
        FindObjectOfType<AudioManager>().UnDeafenSound("riddedred");
        FindObjectOfType<AudioManager>().UnDeafenSound("liik");
    }
}
