using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public HeartSystem heartSystem;
    public void Play()
    {
        heartSystem = GameObject.Find("HeartBackground").GetComponent<HeartSystem>();
        if (heartSystem.CanPlay())
        {
            if (PlayerPrefs.GetInt("selectedLevel") < 1)
            {
                SceneManager.LoadScene("Tutorial_level");
            }
            else
            {
                UseLatestLevel();
                SceneManager.LoadScene("Gameboard");
            }
        }
        else
        {
            Debug.Log("No hearts left! Watch an ad or wait.");
        }
        
        //loads next scene in build queue:
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene("Tutorial_level");
    }

    public void UseLatestLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel");
        PlayerPrefs.SetInt("selectedLevel", currentLevel);
    }

    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Prefs deleted");
    }

    public void Gameboard()
    {
        SceneManager.LoadScene("Gameboard");
    }

    public void LevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void ChapterSelection()
    {
        SceneManager.LoadScene("ChapterSelection");
    }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
