using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterSelectionController : MonoBehaviour
{
    public GameObject[] levelButtons;

    void Awake()
    {
        levelButtons = GameObject.FindGameObjectsWithTag("SelectLevelButton");
    }

    public GameObject starDisplayPrefab;

    private void Start()
    {
        SetupStarsForLevels();
    }

    private void SetupStarsForLevels()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Transform starPlaceholder = levelButtons[i].transform.Find("StarPlaceholder");
            if (starPlaceholder != null)
            {
                GameObject starDisplay = Instantiate(starDisplayPrefab, starPlaceholder.position, Quaternion.identity, starPlaceholder);
                int starsEarned = PlayerPrefs.GetInt("Level_" + i + "_Stars", 0);
                starDisplay.GetComponent<StarDisplayUpdater>().UpdateStars(starsEarned);
                Debug.Log("Tän pitäis toimii?");
            }
        }
    }
}