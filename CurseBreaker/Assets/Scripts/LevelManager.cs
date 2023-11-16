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
    public MovableTileGrid movableTileGrid;

    public TextAsset csvFile; // Reference to your CSV file in Unity (assign it in the Inspector)
    private List<LevelData> allLevelsData;

    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;
    public GameObject lockTilePrefab;


    void Awake()
    {
        allLevelsData = new List<LevelData>(); // Initialization of the list
    }

    void Start()
    {
        ParseCSV();
    }

    public GameObject GetTile(string tileType)
    {
        switch (tileType)
        {
            case "Normal":
                return movableTilePrefab;
            case "Evil":
                return evilTilePrefab;
            default:
                return null;
        }
    }

    public void LoadLevel(int levelNumber)
    {
        LevelData levelData = allLevelsData.FirstOrDefault(l => l.LevelNumber == levelNumber);
        if (levelData != null)
        {
            movableTileGrid.InitializeArray(levelData.GridSizeX, levelData.GridSizeY);

            // Let MovableTileGrid handle the tile generation and the grid initialization.
            foreach (MovableTileData tile in levelData.Tiles)
            {
                movableTileGrid.GenerateTileFromCSV(tile.Column, tile.Row, tile.TileType, levelData.GridSizeX, levelData.GridSizeY, tile.IsLocked, tile.IsKey);
            }
        }
        else
        {
            Debug.LogError("Level data not found for level: " + levelNumber);
            // Handle level not found, maybe load a default level or show an error message.
        }
    }


    private void ParseCSV()
    {
        Debug.Log("parse start");

        using (var reader = new StreamReader(new MemoryStream(csvFile.bytes)))
        {
            LevelData currentLevel = null;
            int currentLevelNumber = -1;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

                // Ensure there's enough data in the line to avoid IndexOutOfRangeException
                if (values.Length < 8) continue;

                
                

                if (values.Length > 0 && int.TryParse(values[0], out int levelNumber))
                {
                    // Check if we've moved on to a new level
                    if (currentLevel == null || currentLevelNumber != levelNumber)
                    {
                        Debug.Log("current level number " + currentLevelNumber);
                        // Parse grid sizes once per level
                        int gridSizeX = int.Parse(values[4]);
                        int gridSizeY = int.Parse(values[5]);

                        // Create a new LevelData instance with grid sizes
                        currentLevel = new LevelData(levelNumber, gridSizeX, gridSizeY);
                        allLevelsData.Add(currentLevel);
                        currentLevelNumber = levelNumber;
                    }

                    // Create a new MovableTileData instance without grid sizes
                    MovableTileData movableTileData = new MovableTileData
                    {
                        Column = int.Parse(values[1]),
                        Row = int.Parse(values[2]),
                        TileType = values[3],
                        IsLocked = values[6].ToLower().Equals("true"),
                        IsKey = values[7].ToLower().Equals("true")
                    };
                    // Add the new tile to the current level
                    currentLevel.Tiles.Add(movableTileData);
                }
            }
        } 

    }


    [System.Serializable]
    public class LevelData
    {
        public int LevelNumber;
        public int GridSizeX;
        public int GridSizeY;
        public List<MovableTileData> Tiles;

        public LevelData(int levelNumber, int gridSizeX, int gridSizeY)
        {
            LevelNumber = levelNumber;
            GridSizeX = gridSizeX;
            GridSizeY = gridSizeY;
            Tiles = new List<MovableTileData>();
        }

    }

    [System.Serializable]
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
}

