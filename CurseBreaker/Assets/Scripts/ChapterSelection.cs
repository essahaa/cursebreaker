using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this at the top of your script
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ChapterSelectionController : MonoBehaviour
{
    public GameObject[] levelButtons;
    public MovableTileGrid movableTileGrid;
    private int currentLevel; //latest completed level
    private int selectedCharacter;
    public HeartSystem heartSystem;

    void Awake()
    {
        levelButtons = GameObject.FindGameObjectsWithTag("SelectLevelButton");
    }

    public GameObject starDisplayPrefab;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");

        CheckLevelProgression();
        SetupStarsForLevels();
    }

    public void setSelectedCharacter(int index)
    {
        Debug.Log("setting char " + index);
        PlayerPrefs.SetInt("selectedCharacter", index);
    }

    private void CheckLevelProgression()
    {
        int[] levelsToGenerate = null;

        switch (selectedCharacter)
        {
            case 0:
                levelsToGenerate = new int[] { 1, 2, 3, 4, 5 };
                break;
            case 1:
                levelsToGenerate = new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                break;
            case 2:
                levelsToGenerate = new int[] { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
                break;
            case 3:
                levelsToGenerate = new int[] { 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 };
                break;
            case 4:
                levelsToGenerate = new int[] { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45 };
                break;
        }

        foreach (GameObject button in levelButtons)
        {
            GameObject parent = button.transform.parent.gameObject;
            string name = parent.name;
            char lastCharacter = name[name.Length - 1];
            int i = (int)char.GetNumericValue(lastCharacter);

            if (levelsToGenerate.Length > i && levelsToGenerate[i] > 0 && levelsToGenerate[i] <= currentLevel)
            {
                Button buttonComponent = button.GetComponent<Button>();

                GameObject levelSelector = GameObject.FindWithTag("LevelSelector");
                buttonComponent.onClick.AddListener(() => HandleLevelSelection(levelsToGenerate[i]));

                foreach (Transform child in button.transform)
                {
                    if (child.name.Contains("Text"))
                    {
                        TextMeshProUGUI textInput = child.GetComponent<TextMeshProUGUI>();
                        textInput.text = levelsToGenerate[i].ToString();
                    }
                }
            }
            else
            {
                parent.SetActive(false);
            }
        }
    }

    private void HandleLevelSelection(int levelNumber)
    {
        heartSystem = GameObject.Find("HeartBackground").GetComponent<HeartSystem>();
        if (heartSystem.CanPlay())
        {
            movableTileGrid.LoadSceneAndLevel(levelNumber);
        }
        else
        {
            Debug.Log("No hearts left! Watch an ad or wait.");
        }
            
    }

    public void SetupStarsForLevels()
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

    public void AssignStarsToCurrentLevel()
    {
        levelButtons = GameObject.FindGameObjectsWithTag("SelectLevelButton");
        foreach (GameObject button in levelButtons)
        {
            // Extract the level number from the button
            Debug.Log("Jtainvaikka");
            TextMeshProUGUI levelText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (levelText != null && int.TryParse(levelText.text, out int levelNumber))
            {
                Debug.Log("levelNumber in CS" + levelNumber);
                // Check if this button's level number matches the current level
                if (levelNumber == currentLevel)
                {
                    // Get the number of stars earned for this level
                    int starsEarned = PlayerPrefs.GetInt("Level_" + levelNumber + "_Stars", 0);
                    Debug.Log("currentLevel in CS" + currentLevel);
                    // Find the star placeholder in this button
                    Transform starPlaceholder = button.transform.Find("StarPlaceholder");
                    if (starPlaceholder != null)
                    {
                        // Instantiate or update the star display with the earned stars
                        GameObject starDisplay = Instantiate(starDisplayPrefab, starPlaceholder.position, Quaternion.identity, starPlaceholder);
                        starDisplay.GetComponent<StarDisplayUpdater>().UpdateStars(starsEarned);
                    }
                    break; // Exit the loop as we've found and updated the current level's button
                }
            }
        }
    }

}