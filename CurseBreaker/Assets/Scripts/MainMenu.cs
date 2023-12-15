using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public HeartSystem heartSystem;
    public Cutscenes cutscenes;
    public Animator animator;
    private SceneLoader loader;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int currentLevel = PlayerPrefs.GetInt("currentLevel");
        if (currentScene.name == "MainMenu" && currentLevel <= 0)
        {
            GameObject levelSelectionObject = GameObject.Find("LevelSelectButton");
            levelSelectionObject.GetComponent<Image>().color = new Color32(140, 140, 140, 255);
            levelSelectionObject.GetComponent<Button>().interactable = false;
        }
        heartSystem = GameObject.Find("HeartBackground").GetComponent<HeartSystem>();
        heartSystem.UpdateHeartDisplay();
    }

    public void Play()
    {
        FindObjectOfType<AudioManager>().Play("musa");
        heartSystem = GameObject.Find("HeartBackground").GetComponent<HeartSystem>();
        loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();

        if (PlayerPrefs.GetInt("tutorialDone") != 1)
        {
            PlayTutorial();
        }
        else if (heartSystem.CanPlay())
        {
            UseLatestLevel();
            loader.LoadScene("Gameboard");
        }
        else
        {
            GameObject NoMoreLivesPanel = GameObject.Find("NoMoreLivesPanel");
            animator = NoMoreLivesPanel.GetComponent<Animator>();
            animator.SetTrigger("NoMoreHearts");
        }
        
        //loads next scene in build queue:
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayTutorial()
    {
        loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
        loader.LoadScene("Tutorial_level");
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
        loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
        loader.LoadScene("Gameboard");
    }

    public void LevelSelection()
    {
        loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
        loader.LoadScene("LevelSelection");
    }

    public void ChapterSelection()
    {
        loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
        loader.LoadScene("ChapterSelection");
    }


    public void GoToMainMenu()
    {
        loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
        loader.LoadScene("MainMenu");
    }
}
