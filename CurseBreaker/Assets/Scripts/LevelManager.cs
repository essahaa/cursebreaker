using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
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
        // Assume the first element is the level name, the second is the difficulty, etc.
        level.LevelNumber = int.Parse(elements[0]);
        level.GridSizeX = int.Parse(elements[4]);
        level.GridSizeY = int.Parse(elements[5]);
        // Add more parsing as per your level design

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
    
}

