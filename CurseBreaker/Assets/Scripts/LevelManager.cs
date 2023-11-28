using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private MovableTileGrid movableTileGrid;

    private GameObject dialogBubble;
    public GameObject dialogBubblePrefab;
    private Image charImage;
    private Sprite curedCharSprite;

    private int tapCounter;
    private bool countTaps = false;

    private string dialogue1_1 = "I am cured! yay!";
    private string dialogue1_2 = "Yst. Terv. Jänö";
    private string dialogue2_1 = "I am also cured! yay!";
    private string dialogue2_2 = "Yst. Terv. Dr Dog Sausage";

    private void Start()
    {
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();
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
    /*
    public TextAsset csvFile; // Reference to your CSV file in Unity (assign it in the Inspector)
    private List<LevelData> levels = new List<LevelData>();

    void Start()
    {
        LoadLevels();
    }

    void LoadLevels()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV TextAsset is not assigned.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            LevelData level = ParseLevelData(line);
            levels.Add(level);
        }
    }

    LevelData ParseLevelData(string line)
    {
        // Split the line by commas and parse each element
        string[] elements = line.Split(';');
        LevelData level = new LevelData();

        if (elements.Length >= 1 && int.TryParse(elements[0], out int levelNumber))
        {
            level.LevelNumber = levelNumber;
            level.GridSizeX = int.Parse(elements[4]);
            level.GridSizeY = int.Parse(elements[5]);


        }

        MovableTileData tile = ParseTileData(elements);
        level.Tiles.Add(tile);

        return level;
    }

    MovableTileData ParseTileData(string[] tileElements)
    {       
        MovableTileData tile = new MovableTileData();
        // Parse the tileElements to fill in the tile data
        tile.Column = int.Parse(tileElements[1]);
        tile.Row = int.Parse(tileElements[2]);
        tile.TileType = tileElements[3];
        tile.IsLocked = tileElements[6].ToLower().Equals("true");
        tile.IsKey = tileElements[7].ToLower().Equals("true");
 
        return tile;
    }


    public LevelData GetLevelData(int levelNumber)
    {
        // You can add more robust checking here (e.g., if levelNumber is within the range)
        return levels.FirstOrDefault(level => level.LevelNumber == levelNumber);
    }


    public class LevelData
    {
        public int LevelNumber;
        public int GridSizeX;
        public int GridSizeY;
        public List<MovableTileData> Tiles;
    }

    public class MovableTileData
    {
        public int Level;
        public int Row;
        public int Column;
        public string TileType;
        public bool IsLocked;
        public bool IsKey;
    }
    */

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
            case 5: case 20: case 30: case 40: case 50:
                UpdateCharacter(currentCharacter);
                break;
        }
    }

    public void getSideCharacter()
    {
        int charIndex = PlayerPrefs.GetInt("currentCharacter");
        GameObject charObject = GameObject.FindWithTag("SideCharacter");
        charImage = charObject.GetComponent<Image>();
        Sprite[] spritesheet = null;
        Sprite[] altSpritesheet = null;
        string spriteName = "";
        string altSpriteName = "";
        Sprite charSprite = null;

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

        charImage.sprite = charSprite;
    }

    private void UpdateCharacter(int characterIndex)
    {
        int newCharacterIndex = characterIndex + 1;
        PlayerPrefs.SetInt("currentCharacter", newCharacterIndex);
    }

    public void PlayCharacterCompleteSequence(int charIndex)
    {
        GameObject levelCompletedBox = GameObject.FindWithTag("LevelCompletedBox");

        countTaps = true;
        tapCounter = 1;

        Vector3 dialogPosition = new Vector3(0, -4f, 0);
        dialogBubble = Instantiate(dialogBubblePrefab, dialogPosition, Quaternion.identity, levelCompletedBox.transform);
        dialogBubble.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        charImage.sprite = curedCharSprite;
        ShowNextSpeechBubble();
    }

    public void ShowNextSpeechBubble()
    {
        TextMeshPro tmp = dialogBubble.GetComponentInChildren<TextMeshPro>();
        tmp.text = "";
        int charIndex = PlayerPrefs.GetInt("currentCharacter");

        GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
        Animator animator = levelCompletedBox.GetComponent<Animator>();

        switch (charIndex)
        {
            case 1:
                switch(tapCounter)
                {
                    case 1:
                        tmp.text = dialogue1_1;
                        break;
                    case 2:
                        tmp.text = dialogue1_2;
                        break;
                    default:
                        animator.SetTrigger("LevelEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        getSideCharacter();
                        break;
                }
                break;
            case 2:
                switch (tapCounter)
                {
                    case 1:
                        tmp.text = dialogue2_1;
                        break;
                    case 2:
                        tmp.text = dialogue2_2;
                        break;
                    default:
                        animator.SetTrigger("LevelEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        getSideCharacter();
                        break;
                }
                break;
        }
    }
}

