using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
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

    private void CompleteCharacter(int currentCharacter)
    {

    }

    private void UpdateCharacter(int characterIndex)
    {
        int newCharacterIndex = characterIndex + 1;
        PlayerPrefs.SetInt("currentCharacter", newCharacterIndex);
    }
}

