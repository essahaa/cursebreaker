using UnityEngine;
using UnityEngine.UI;
public class Soundcontroller : MonoBehaviour
{
    private bool isMuted = false;

    public Sprite soundOnImage;
    public Sprite soundOffImage;

    public void ToggleMuteSounds(bool isMuted)
    {
        this.isMuted = isMuted;

        if (isMuted)
        {
            FindObjectOfType<AudioManager>().buttonImage.sprite = soundOffImage;
            FindObjectOfType<AudioManager>().buttonSprite = soundOffImage;
            DeafenSounds();
        }
        else
        {
            FindObjectOfType<AudioManager>().buttonImage.sprite = soundOnImage;
            FindObjectOfType<AudioManager>().buttonSprite = soundOnImage;
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
