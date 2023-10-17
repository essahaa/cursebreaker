using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public BackgroundGrid backgroundGrid;

    public TextAsset csvFile; // Reference to your CSV file in Unity (assign it in the Inspector).

    public int targetLevel = 1; // The level you want to generate.

    public GameObject movableTilePrefab; // The prefab for the movable tiles.
    public GameObject evilTilePrefab;

    void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();

        string[] csvLines = csvFile.text.Split('\n'); // Split the CSV file into lines.

        foreach (string line in csvLines)
        {
            string[] values = line.Split(';'); // Split each line into values.

            // Check if the current line corresponds to the target level.
            if (values.Length >= 1 && int.TryParse(values[0], out int level) && level == targetLevel)
            {
                // Parse data from the CSV line.
                int column = int.Parse(values[1]);
                int row = int.Parse(values[2]);
                float x = float.Parse(values[3]);
                float y = float.Parse(values[4]);
                string tileType = values[5];
                int gridSizeX = int.Parse(values[6]);
                int gridSizeY = int.Parse(values[7]);

                GameObject tilePrefab = GetTilePrefab(tileType);

                // Instantiate movable tiles based on the data.
                Vector3 position = new Vector3(x, y, 0f);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

                // Attach the level tile data to the game object.
                MovableTile tileData = tile.AddComponent<MovableTile>();
                tileData.Level = level;
                tileData.Row = row;
                tileData.Column = column;
                tileData.TileType = tileType;
                tileData.GridSizeX = gridSizeX;
                tileData.GridSizeY = gridSizeY;

                backgroundGrid.GenerateBackgroundGrid(gridSizeX, gridSizeY);
            }
        }
    }

    GameObject GetTilePrefab(string tileType)
    {
        // Choose the appropriate prefab based on the tileType.
        switch (tileType)
        {
            case "Normal":
                return movableTilePrefab;
            case "Evil":
                return evilTilePrefab;
            // Add more cases for other tile types as needed.
            default:
                return movableTilePrefab; // Default to a fallback prefab.
        }
    }
}

