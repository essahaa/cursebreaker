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
    //public TextAsset csvFile;
    //private List<LevelData> levels = new List<LevelData>();
    /*
    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject keyTilePrefab;
    public GameObject arrowPrefab;
    public GameObject arrow;
    */
    private GameObject dialogBubble;
    private Image charImage;
    private Sprite curedCharSprite;

    private int tapCounter;
    private bool countTaps = false;

    private string dialogue1_1 = "You have completed all of my levels!";
    private string dialogue1_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue2_1 = "You have completed all of my levels!";
    private string dialogue2_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue3_1 = "You have completed all of my levels!";
    private string dialogue3_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue4_1 = "You have completed all of my levels!";
    private string dialogue4_2 = "Thank you breaking my curse! I feel much better.";
    private string dialogue5_1 = "You have completed all of my levels!";
    private string dialogue5_2 = "Thank you breaking my curse! I feel much better.";

    private void Start()
    {
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();
        //LoadLevels();
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
    public void LoadLevel(int levelNumber)
    {
        if (levels.Count == 0 || levels.Count < levelNumber)
        {
            LoadLevels();
        }
        else if(levels.Count > 0)
        {
            LevelData levelToLoad = GetLevelData(levelNumber);
            GenerateLevel(levelToLoad);
        }     
    }

    void LoadLevels()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV TextAsset is not assigned.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        levels = new List<LevelData>(); // Initialize the levels list

        // First, create levels without tiles
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.Trim().Equals(";;;;;;;;"))
            {
                CreateLevelIfNotExists(line);
            }
        }

        // Then, add tiles to each level
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.Trim().Equals(";;;;;;;;"))
            {
                AddTileToLevel(line);
            }
        }
    }

    void CreateLevelIfNotExists(string line)
    {
        string[] elements = line.Split(';');
        if (elements.Length >= 8 && int.TryParse(elements[0], out int levelNumber) && levels.All(l => l.LevelNumber != levelNumber))
        {
            LevelData newLevel = new LevelData
            {
                LevelNumber = levelNumber,
                GridSizeX = int.Parse(elements[4]),
                GridSizeY = int.Parse(elements[5]),
                Tiles = new List<MovableTileData>()
            };
            levels.Add(newLevel);
        }
    }

    void AddTileToLevel(string line)
    {
        string[] elements = line.Split(';');
        if (elements.Length >= 8 && int.TryParse(elements[0], out int levelNumber))
        {
            LevelData level = levels.FirstOrDefault(l => l.LevelNumber == levelNumber);
            if (level != null)
            {
                MovableTileData tile = ParseTileData(elements);
                level.Tiles.Add(tile);
            }
        }
    }

    MovableTileData ParseTileData(string[] tileElements)
    {
        MovableTileData tileData = new MovableTileData();

        tileData.Column = int.Parse(tileElements[1]);
        tileData.Row = int.Parse(tileElements[2]);
        tileData.TileType = tileElements[3];
        tileData.IsLocked = tileElements[6].ToLower().Equals("true");
        tileData.IsKey = tileElements[7].ToLower().Equals("true");

        return tileData;
    }

    public LevelData GetLevelData(int levelNumber)
    {
        Debug.Log("Searching for level: " + levelNumber);
        LevelData foundLevel = levels.FirstOrDefault(level => level.LevelNumber == levelNumber);

        if (foundLevel != null)
        {
            Debug.Log("Found level: " + foundLevel.LevelNumber);
        }
        else
        {
            Debug.LogWarning("Level not found: " + levelNumber);
        }

        return foundLevel;
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
        public int Column;
        public int Row;
        public string TileType;
        public bool IsLocked;
        public bool IsKey;
    }

    void GenerateLevel(LevelData levelData)
    {
        BackgroundGrid backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();

        if (!movableTileGrid.backgroundGenerated)
        {
            backgroundGrid.GenerateBackgroundGrid(levelData.GridSizeX, levelData.GridSizeY);
            movableTileGrid.ChangeBgGenerated();
            GenerateArrowPrefab();
        }

        foreach (MovableTileData tileData in levelData.Tiles)
        {
            GenerateTile(tileData, levelData.GridSizeX, levelData.GridSizeY, backgroundGrid);
        }
    }

    void GenerateTile(MovableTileData tileData, int gridSizeX, int gridSizeY, BackgroundGrid backgroundGrid)
    {
        int column = tileData.Column;
        int row = tileData.Row;
        string tileType = tileData.TileType;
        bool isLocked = tileData.IsLocked;
        bool isKey = tileData.IsKey;

        if (column < gridSizeX && row < gridSizeY)
        {
            // Check if the position exists in the backgroundGrid array
            if (backgroundGrid.backgroundGrid[column, row] != null)
            {
                // Get the position from the backgroundGrid array
                Vector3 position = backgroundGrid.backgroundGrid[column, row].position;
                GameObject tilePrefab = GetTilePrefab(tileType);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
                
                // add individual Animator controllers
                tile.AddComponent<Animator>();
                Animator animator = tile.GetComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(GetAnimationController(tileType));

                MovableTile tileComponent = tile.GetComponent<MovableTile>();
                if (tileComponent != null)
                {
                    tileComponent.Column = tileData.Column;
                    tileComponent.Row = tileData.Row;
                    tileComponent.TileType = tileData.TileType;
                    tileComponent.IsLocked = tileData.IsLocked;
                    tileComponent.IsKey = tileData.IsKey;
                }

                // Check if the lock tile is being created
                if (isLocked)
                {
                    GameObject lockTile = Instantiate(lockTilePrefab, position, Quaternion.identity);
                    lockTile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
                    lockTile.transform.SetParent(tile.transform);
                }
                if (isKey)
                {
                    GameObject keyTile = Instantiate(keyTilePrefab, position, Quaternion.identity);
                    keyTile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
                    keyTile.transform.SetParent(tile.transform);
                }

                movableTileGrid.UpdateMovableTile(column, row, tile.transform);
                movableTileGrid.UpdateMovableTilesArray();
            }
        }
    }

    private GameObject GetTilePrefab(string tileType)
    {
        // Choose the appropriate prefab based on the tileType.
        switch (tileType)
        {
            case "Normal":
                return movableTilePrefab;
            case "Evil":
                return evilTilePrefab;
            default:
                return movableTilePrefab;
        }
    }

    private string GetAnimationController(string tileType)
    {
        switch (tileType)
        {
            case "Normal":
                return "TileController";
            case "Evil":
                return "EvilTileController";
            default:
                return null;
        }
    }

    private void GenerateArrowPrefab()
    {
        Vector3 arrowposition = new Vector3(0f, 3f, 0);
        arrow = Instantiate(arrowPrefab, arrowposition, Quaternion.identity);
    }

    public void RotateArrow(int rotation)
    {
        arrow.transform.rotation = Quaternion.Euler(0, 0, rotation);
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

    public void PlayCharacterCompleteSequence(int charIndex)
    {
        GameObject levelCompletedBox = GameObject.FindWithTag("LevelCompletedBox");

        countTaps = true;
        tapCounter = 1;

        dialogBubble = GameObject.FindWithTag("DialogBox");
        SpriteRenderer image = dialogBubble.GetComponent<SpriteRenderer>();
        image.enabled = true;
        Debug.Log("image enabled: " + image);

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
                        animator.SetTrigger("ContinueButtonEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
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
                        animator.SetTrigger("ContinueButtonEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        break;
                }
                break;
            case 3:
                switch (tapCounter)
                {
                    case 1:
                        tmp.text = dialogue3_1;
                        break;
                    case 2:
                        tmp.text = dialogue3_2;
                        break;
                    default:
                        animator.SetTrigger("ContinueButtonEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        break;
                }
                break;
            case 4:
                switch (tapCounter)
                {
                    case 1:
                        tmp.text = dialogue4_1;
                        break;
                    case 2:
                        tmp.text = dialogue4_2;
                        break;
                    default:
                        animator.SetTrigger("ContinueButtonEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        break;
                }
                break;
            case 5:
                switch (tapCounter)
                {
                    case 1:
                        tmp.text = dialogue5_1;
                        break;
                    case 2:
                        tmp.text = dialogue5_2;
                        break;
                    default:
                        animator.SetTrigger("ContinueButtonEnd");
                        Destroy(dialogBubble);
                        countTaps = false;
                        movableTileGrid.DestroyExistingMovableTiles();
                        break;
                }
                break;
        }
    }
}

