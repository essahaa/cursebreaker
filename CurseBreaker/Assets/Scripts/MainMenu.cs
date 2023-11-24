using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        if(PlayerPrefs.GetInt("selectedLevel") < 1)
        {
            SceneManager.LoadScene("Tutorial_level");
        }else
        {
            SceneManager.LoadScene("Gameboard");
        }
        
        //loads next scene in build queue:
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene("Tutorial_level");
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
    public void LevelCompleted()
    {
        SceneManager.LoadScene("LevelCompleted");
    }
}
