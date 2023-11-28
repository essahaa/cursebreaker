using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add this at the top of your script

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

            TextMeshProUGUI levelText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (levelText != null)
            {
                if (int.TryParse(levelText.text, out int levelNumber))
                {
                    if (starPlaceholder != null)
                    {
                        GameObject starDisplay = Instantiate(starDisplayPrefab, starPlaceholder.position, Quaternion.identity, starPlaceholder);
                        int starsEarned = PlayerPrefs.GetInt("Level_" + levelNumber + "_Stars", 0);
                        starDisplay.GetComponent<StarDisplayUpdater>().UpdateStars(starsEarned);
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse level number from TextMeshPro on button: " + i);
                }
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on button: " + i);
            }
        }
    }
}