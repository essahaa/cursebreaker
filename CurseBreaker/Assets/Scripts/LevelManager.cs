using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;

public class LevelManager : MonoBehaviour
{
    private MovableTileGrid movableTileGrid;
    public TextAsset csvFile; // Reference to your CSV file in Unity (assign it in the Inspector).
    private List<string> csvLines = new List<string>(); // Store CSV lines in a list.

    private Animator animator;
    private GameObject restartButton;

    private GameObject dialogBubble;
    private Image dialogBubbleImage;
    public Sprite bubbleLeft;
    public Sprite bubbleRight;

    private Image charImage;
    private Sprite curedCharSprite;

    private int selectedLevel;

    private int tapCounter;
    private bool countTaps = false;

    private string dialogue1_1 = "You have completed all of my levels!";
    private string dialogue1_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue1_3 = "Please cure me Lynara!";
    private string dialogue1_4 = "Let's break Sir Sausages curse!";
    private string dialogue2_1 = "You have completed all of my levels!";
    private string dialogue2_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue2_3 = "Please cure me Lynara!";
    private string dialogue2_4 = "Let's break Dr. Owls curse!";
    private string dialogue3_1 = "You have completed all of my levels!";
    private string dialogue3_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue3_3 = "Please cure me Lynara!";
    private string dialogue3_4 = "Let's break Mr Melon Cat's curse!";
    private string dialogue4_1 = "You have completed all of my levels!";
    private string dialogue4_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue4_3 = "Please cure me Lynara!";
    private string dialogue4_4 = "Let's break the Great Ox's curse!";

    private void Start()
    {
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();

        selectedLevel = PlayerPrefs.GetInt("selectedLevel");
        ReadCSV();

        
        ShowLevelText();       
        
    }

    private void Update()
    {
        if (countTaps && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                tapCounter++;
                ShowNextSpeechBubble();
            }
        }
    }

    public void LoadSceneAndLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("selectedLevel", levelNumber);
        SceneManager.LoadScene("Gameboard");
    }

    public void LoadLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("selectedLevel", levelNumber);
        selectedLevel = levelNumber;

        ReadLevelDataFromCSV();
    }

    public void ReadCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        csvLines.AddRange(lines);
    }

    public void ReadLevelDataFromCSV()
    {
        selectedLevel = PlayerPrefs.GetInt("selectedLevel");
        FirebaseAnalytics.LogEvent("level_started", "level_number", selectedLevel.ToString());
        bool arraySizeSet = false; // Add a flag to track if array size is set.
        bool noMoreLevels = true; // Flag to check if there are no more levels.

        if (csvLines.Count > 0)
        {
            foreach (string line in csvLines)
            {
                string[] values = line.Split(';'); // Split each line into values.

                // Check if the current line corresponds to the target level.
                if (values.Length >= 1 && int.TryParse(values[0], out int level) && level == selectedLevel)
                {
                    noMoreLevels = false; // We found a matching level.
                    // Parse data from the CSV line.
                    int column = int.Parse(values[1]);
                    int row = int.Parse(values[2]);
                    string tileType = values[3];
                    int gridSizeX = int.Parse(values[4]);
                    int gridSizeY = int.Parse(values[5]);
                    string isLockedStr = values[6].ToLower(); // Convert to lowercase to handle case-insensitivity
                    bool isLocked = isLockedStr == "true";
                    bool isKey = values[7].ToLower() == "true";

                    // Set the array size only once.
                    if (!arraySizeSet)
                    {
                        movableTileGrid.SetMovableTilesArray(gridSizeX, gridSizeY);
                        arraySizeSet = true; // Update the flag.
                    }
                    movableTileGrid.GenerateTileFromCSV(column, row, tileType, gridSizeX, gridSizeY, isLocked, isKey);
                }
            }
        }
        if (noMoreLevels)
        {
            Debug.Log("No more levels in the CSV file.");
            GameObject gameCompleteBox = GameObject.Find("GameComplete");
            animator = gameCompleteBox.GetComponent<Animator>();
            animator.SetTrigger("GameCompleted");
            GameObject restartButton = GameObject.Find("RestartButton");
            restartButton.SetActive(false);
            GameObject levelText = GameObject.Find("ShowLevelText");
            levelText.SetActive(false);
        }
    }

    public void UpdateProgression(int completedLevel)
    {
        int currentLevel = PlayerPrefs.GetInt("currentLevel"); //latest level in progression
        int newCurrentLevel = completedLevel + 1;
        if (newCurrentLevel > currentLevel)
        {
            PlayerPrefs.SetInt("currentLevel", newCurrentLevel);
        }

        int currentCharacter = PlayerPrefs.GetInt("currentCharacter");
        switch (newCurrentLevel)
        {
            case 6: case 16: case 26: case 36: case 46:
                UpdateCharacter(currentCharacter);
                break;
        }
    }

    public void GetSideCharacter()
    {
        int selectedLevel = PlayerPrefs.GetInt("selectedLevel");
        int currentCharacter = PlayerPrefs.GetInt("currentCharacter");
        GameObject charObject = GameObject.FindWithTag("SideCharacter");
        charImage = charObject.GetComponent<Image>();
        Sprite[] spritesheet = null;
        Sprite[] altSpritesheet = null;
        string spriteName = "";
        string altSpriteName = "";
        Sprite charSprite = null;
        int charIndex = 0;

        if(selectedLevel <= 5)
        {
            charIndex = 0;
        }else if(selectedLevel > 5 && selectedLevel <= 15)
        {
            charIndex = 1;
        }else if(selectedLevel > 15 && selectedLevel <= 25)
        {
            charIndex = 2;
        }else if(selectedLevel > 25 && selectedLevel <= 35)
        {
            charIndex = 3;
        }else if(selectedLevel > 35 && selectedLevel <= 45)
        {
            charIndex = 4;
        }

        switch(charIndex)
        {
            case 0:
                //bunny sprites
                spritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                spriteName = "sideCharacters-0_3";
                altSpritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                altSpriteName = "sideCharacters-0_4";
                break;
            case 1:
                //dog sprites
                spritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                spriteName = "sideCharacters-1_1";
                altSpritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                altSpriteName = "sideCharacters-1_5";
                break;
            case 2:
                //owl sprites
                spritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                spriteName = "sideCharacters-0_2";
                altSpritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                altSpriteName = "sideCharacters-0_2";
                break;
            case 3:
                //cat sprites
                spritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                spriteName = "sideCharacters-1_10";
                altSpritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                altSpriteName = "sideCharacters-1_2";
                break;
            case 4:
                //ox sprites
                spritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                spriteName = "sideCharacters-1_9";
                altSpritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                altSpriteName = "sideCharacters-0_1";
                break;
        }

        if(charIndex >= 0)
        {
            foreach(Sprite sprite in spritesheet)
            {
                if(sprite.name == spriteName)
                {
                    charSprite = sprite;
                }
            }
            foreach (Sprite sprite in altSpritesheet)
            {
                if (sprite.name == altSpriteName)
                {
                    curedCharSprite = sprite;
                }
            }
        }

        if(currentCharacter > charIndex)
        {
            charImage.sprite = curedCharSprite;
        }else
        {
            charImage.sprite = charSprite;
        }
    }

    private void UpdateCharacter(int characterIndex)
    {
        int newCharacterIndex = characterIndex + 1;
        PlayerPrefs.SetInt("currentCharacter", newCharacterIndex);
    }

    private void TemporarySpriteChange(int charIndex)
    {
        UpdateCharacter(charIndex);
        GetSideCharacter();
        PlayerPrefs.SetInt("currentCharacter", charIndex);
    }

    public void PlayCharacterCompleteSequence()
    {
        //GameObject levelCompletedBox = GameObject.FindWithTag("LevelCompletedBox");

        countTaps = true;
        tapCounter = 1;

        dialogBubble = GameObject.FindWithTag("DialogBox");
        dialogBubbleImage = dialogBubble.GetComponent<Image>();
        dialogBubbleImage.enabled = true;

        charImage.sprite = curedCharSprite;
        ShowNextSpeechBubble();
    }

    public void ShowNextSpeechBubble()
    {
        TextMeshProUGUI tmp = dialogBubble.GetComponentInChildren<TextMeshProUGUI>();
        int charIndex = PlayerPrefs.GetInt("currentCharacter");

        GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
        Animator animator = levelCompletedBox.GetComponent<Animator>();

        switch (charIndex)
        {
            case 1:
                switch(tapCounter)
                {
                    case 1:
                        dialogBubbleImage.sprite = bubbleRight;
                        tmp.text = dialogue1_1;
                        break;
                    case 2:
                        tmp.text = dialogue1_2;
                        break;
                    case 3:
                        tmp.text = "";
                        animator.SetBool("CharacterDown", true);
                        TemporarySpriteChange(charIndex);
                        animator.SetBool("CharacterDown", false);
                        tmp.text = dialogue1_3;
                        break;
                    case 4:
                        dialogBubbleImage.sprite = bubbleLeft;
                        tmp.text = dialogue1_4;
                        break;
                    default:
                        tmp.text = "";
                        animator.SetTrigger("ContinueButtonEnd");
                        dialogBubbleImage.enabled = false;
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        ShowLevelText();
                        break;
                }
                break;
            case 2:
                switch (tapCounter)
                {
                    case 1:
                        dialogBubbleImage.sprite = bubbleRight;
                        tmp.text = dialogue2_1;
                        break;
                    case 2:
                        tmp.text = dialogue2_2;
                        break;
                    case 3:
                        tmp.text = "";
                        animator.SetBool("CharacterDown", true);
                        TemporarySpriteChange(charIndex);
                        animator.SetBool("CharacterDown", false);
                        tmp.text = dialogue2_3;
                        break;
                    case 4:
                        dialogBubbleImage.sprite = bubbleLeft;
                        tmp.text = dialogue2_4;
                        break;
                    default:
                        tmp.text = "";
                        animator.SetTrigger("ContinueButtonEnd");
                        dialogBubbleImage.enabled = false;
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        ShowLevelText();
                        break;
                }
                break;
            case 3:
                switch (tapCounter)
                {
                    case 1:
                        dialogBubbleImage.sprite = bubbleRight;
                        tmp.text = dialogue2_1;
                        break;
                    case 2:
                        tmp.text = dialogue2_2;
                        break;
                    case 3:
                        tmp.text = "";
                        animator.SetBool("CharacterDown", true);
                        TemporarySpriteChange(charIndex);
                        animator.SetBool("CharacterDown", false);
                        tmp.text = dialogue2_3;
                        break;
                    case 4:
                        dialogBubbleImage.sprite = bubbleLeft;
                        tmp.text = dialogue2_4;
                        break;
                    default:
                        tmp.text = "";
                        animator.SetTrigger("ContinueButtonEnd");
                        dialogBubbleImage.enabled = false;
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        ShowLevelText();
                        break;
                }
                break;
            case 4:
                switch (tapCounter)
                {
                    case 1:
                        dialogBubbleImage.sprite = bubbleRight;
                        tmp.text = dialogue2_1;
                        break;
                    case 2:
                        tmp.text = dialogue2_2;
                        break;
                    default:
                        tmp.text = "";
                        animator.SetTrigger("ContinueButtonEnd");
                        dialogBubbleImage.enabled = false;
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        break;
                }
                break;
        }
    }

    public void ShowLevelText()
    {
        GameObject textObject = GameObject.Find("ShowLevelText");
        // Update TextMeshPro UI
        if (textObject != null)
        {
            TextMeshProUGUI textComponentFromOtherObject = textObject.GetComponent<TextMeshProUGUI>();
            if (textComponentFromOtherObject != null)
            {
                textComponentFromOtherObject.text = selectedLevel.ToString();

            }
        }
    }

    public void ShowLevelFailedText()
    {
        GameObject textObject = GameObject.Find("MoveText");
        // Update TextMeshPro UI
        if (textObject != null)
        {
            TextMeshProUGUI textComponentFromOtherObject = textObject.GetComponent<TextMeshProUGUI>();
            if (textComponentFromOtherObject != null)
            {
                textComponentFromOtherObject.text = "Yellow tile dropped.";

            }
        }
    }
}

