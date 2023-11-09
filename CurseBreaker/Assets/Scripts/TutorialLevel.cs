using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel : MonoBehaviour
{
    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;

    public TextAsset csvFile;
    public BackgroundGrid backgroundGrid;
    
    public Animator animator;

    private int selectedLevel = 0; // The level you want to generate.
    
    private int rowIndex;
    private int columnIndex;

    private string currentMoveType = "horizontal";

    public bool firstMovementDone = false;
    public bool tutorialDone = false;
    
    private bool backgroundGenerated = false;

    public int gridSizeX = 10; //number of columns, width of the grid
    public int gridSizeY = 10; //number of rows, height of the grid

    private Transform[] currentMovableTiles;
    private Transform[,] movableTiles;
    public Vector3[,] initialTilePositions;

    private List<string> csvLines = new List<string>();

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();

        ReadLevelDataFromCSV();  
    }

    

    public void ReadLevelDataFromCSV()
    {
        using (var reader = new StreamReader(new MemoryStream(csvFile.bytes)))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

                // Ensure there's enough data in the line to avoid IndexOutOfRangeException
                if (values.Length < 6) continue;

                if (int.TryParse(values[0], out int level) && level == selectedLevel)
                {
                    // Use TryParse for safety.
                    if (int.TryParse(values[1], out int column) &&
                        int.TryParse(values[2], out int row) &&
                        int.TryParse(values[4], out gridSizeX) &&
                        int.TryParse(values[5], out gridSizeY))
                    {
                        string tileType = values[3];

                        // Initialize the array on first successful parse.
                        if (movableTiles == null)
                        {
                            movableTiles = new Transform[gridSizeX, gridSizeY];
                        }

                        GenerateTileFromCSV(column, row, tileType, gridSizeX, gridSizeY);
                    }
                    else
                    {
                        // Handle parsing errors or log for debugging
                    }
                }
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

    public void GenerateTileFromCSV(int column, int row, string tileType, int gridSizeX, int gridSizeY)
    {
        if (!backgroundGenerated)
        {
            backgroundGrid.GenerateBackgroundGrid(gridSizeX, gridSizeY);
            backgroundGenerated = true;
        }

        GameObject tilePrefab = GetTilePrefab(tileType);

        if (column < gridSizeX && row < gridSizeY)
        {
            // Cache the reference to the background grid cell
            Transform backgroundGridCell = backgroundGrid.backgroundGrid[column, row];

            if (backgroundGridCell != null)
            {
                // Get the position from the cached reference
                Vector3 position = backgroundGridCell.position;

                // Create the tile at the retrieved position
                MovableTile tile = Instantiate(tilePrefab, position, Quaternion.identity).GetComponent<MovableTile>();
                tile.Row = row;
                tile.Column = column;
                tile.TileType = tileType;

                tile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);

                
                movableTiles[column, row] = tile.transform;

                MovableTile tileData = tile.GetComponent<MovableTile>();
                if (tileData != null)
                {
                    tileData.Level = selectedLevel;
                    tileData.Row = row;
                    tileData.Column = column;
                    tileData.TileType = tileType;
                    tileData.GridSizeX = gridSizeX;
                    tileData.GridSizeY = gridSizeY;
                }
            }
        }
    }

    public void UpdateMovableTile(int column, int row, Transform newTile)
    {
        //empty cell 
        movableTiles[column, row] = null;
        // Update the cell with the new tile.
        movableTiles[column, row] = newTile;

        MovableTile tile = movableTiles[column, row].GetComponent<MovableTile>();

        if (tile != null)
        {
            tile.Row = row;
            tile.Column = column;
        }

    }

    public Transform[,] UpdateMovableTilesArray()
    {

        for (int row = 0; row < gridSizeY; row++)
        {
            for (int col = 0; col < gridSizeX; col++)
            {
                // Get the tile at the current position.
                Transform tile = movableTiles[col, row];

                if (tile != null && (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile")))
                {
                    // Update the movableTiles array.
                    movableTiles[col, row] = tile.transform;
                }
                else
                {
                    movableTiles[col, row] = null;
                }
            }
        }

        return movableTiles;
    }

    public Transform[] FindAdjacentMovableTilesInRow(int rowIndex)
    {
        Debug.Log("rowIndex in FindAdjacentMovableTilesInRow " + rowIndex);

        int numCols = movableTiles.GetLength(0); // Number of columns.
        Transform[] tilesInRow = new Transform[numCols]; // Only need a 1D array for a single row.

        if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(1))
        {
            for (int col = 0; col < numCols; col++)
            {
                // Only consider this tile if it's not null and has at least one adjacent tile.
                if (movableTiles[col, rowIndex] != null &&
                    ((col > 0 && movableTiles[col - 1, rowIndex] != null) ||
                     (col < numCols - 1 && movableTiles[col + 1, rowIndex] != null)))
                {
                    tilesInRow[col] = movableTiles[col, rowIndex];
                }
            }
        }

        return tilesInRow;
    }


    public Transform[] FindAdjacentMovableTilesInColumn(int columnIndex)
    {
        Debug.Log("columnIndex in FindAdjacentMovableTilesInColumn " + columnIndex);

        int numRows = movableTiles.GetLength(1); // Number of rows.
        Transform[] tilesInColumn = new Transform[numRows]; // Only need a 1D array for a single column.

        if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(0))
        {
            for (int row = 0; row < numRows; row++)
            {
                if (movableTiles[columnIndex, row] != null)
                {
                    // Check if the tile has an adjacent tile above or below.
                    bool hasAboveAdjacent = (row > 0 && movableTiles[columnIndex, row - 1] != null);
                    bool hasBelowAdjacent = (row < numRows - 1 && movableTiles[columnIndex, row + 1] != null);

                    if (hasAboveAdjacent || hasBelowAdjacent)
                    {
                        // Include this tile in the movable column.
                        tilesInColumn[row] = movableTiles[columnIndex, row];
                    }
                }
            }
        }

        return tilesInColumn;
    }


    public Transform[] TutorialLevelFindCurrentMovables()
    {
        if (!firstMovementDone)
        {
            currentMoveType = "horizontal";
            rowIndex = 5;

            currentMovableTiles = FindAdjacentMovableTilesInRow(rowIndex);
        }
        else if (firstMovementDone)
        {
            currentMoveType = "vertical";
            columnIndex = 5;

            currentMovableTiles = FindAdjacentMovableTilesInColumn(columnIndex);
        }

        return currentMovableTiles;
    }

    public Vector3[,] TutorialLevelGetInitialPositions(Transform[] currentMovableTiles)
    {
        // Initialize the array for initial positions
        initialTilePositions = new Vector3[gridSizeY, gridSizeX];

        foreach (Transform tile in currentMovableTiles)
        {
            if (tile != null)
            {
                MovableTile movableTile = tile.GetComponent<MovableTile>();
                if (movableTile != null)
                {
                    int row = movableTile.Row;
                    int col = movableTile.Column;
                    initialTilePositions[col, row] = tile.position;
                }
                else
                {
                    Debug.LogError("MovableTile component not found on tile.");
                }
            }
        }

        return initialTilePositions;
    }


    public void TutorialCompleted()
    {
        if(firstMovementDone)
        {
            GameObject tileToDestroy = movableTiles[6, 5].gameObject;       
            Destroy(tileToDestroy);
            Debug.Log("tutorial completed");
            tutorialDone = true;

            GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
            if (levelCompletedBox != null)
            {
                animator = levelCompletedBox.GetComponent<Animator>();
            }
            animator.SetTrigger("LevelEnd");
        }
        
    }

    public void ChangeMovementDone()
    {
        firstMovementDone = true;
        currentMoveType = "vertical";
        UpdateMovableTilesArray();
    }

    public void EmptyCurrentMovableArray()
    {
        //empty current movable tiles array
        for (int i = 0; i < currentMovableTiles.Length; i++)
        {
            currentMovableTiles[i] = null; 
        }
    }
    
}

