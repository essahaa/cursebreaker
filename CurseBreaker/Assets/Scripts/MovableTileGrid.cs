using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MovableTileGrid : MonoBehaviour
{
    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;
    public BackgroundGrid backgroundGrid;

    public TextAsset csvFile; // Reference to your CSV file in Unity (assign it in the Inspector).

    public float movableTileSize = 1.0f; // Adjust the size of movable tiles.

    public int startColumn = 0; // Set the starting column index.
    public int startRow = 0;    // Set the starting row index.

    private int targetLevel = 2; // The level you want to generate.

    public Transform[,] movableTiles; // Change to a Transform[,] array.

    void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();

        movableTiles = new Transform[backgroundGrid.gridSizeX, backgroundGrid.gridSizeY]; // Use the size of the background grid.

        backgroundGrid.GenerateBackgroundGrid(backgroundGrid.gridSizeX, backgroundGrid.gridSizeY);

        ReadLevelDataFromCSV(csvFile);

    }

    public void ReadLevelDataFromCSV(TextAsset csvText)
    {
        string[] csvLines = csvText.text.Split('\n'); // Split the CSV file into lines.

        foreach (string line in csvLines)
        {
            string[] values = line.Split(';'); // Split each line into values.

            // Check if the current line corresponds to the target level.
            if (values.Length >= 1 && int.TryParse(values[0], out int level) && level == targetLevel)
            {
                Debug.Log("level " + values[0] + "targetlevel " + targetLevel);
                // Parse data from the CSV line.
                int column = int.Parse(values[1]);
                int row = int.Parse(values[2]);
                string tileType = values[3];
                int gridSizeX = int.Parse(values[4]);
                int gridSizeY = int.Parse(values[5]);

                GenerateTileFromCSV(column, row, tileType, gridSizeX, gridSizeY);
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

    void GenerateTileFromCSV(int column, int row, string tileType, int gridSizeX, int gridSizeY)
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        GameObject tilePrefab = GetTilePrefab(tileType);

        if (column < gridSizeX && row < gridSizeY)
        {
            // Calculate the initial position for the grid with an offset
            Vector2 gridCenter = new Vector2(
                (gridSizeX - 1) * backgroundGrid.backgroundTileSize * 0.5f - (2 * backgroundGrid.backgroundTileSize),
                (gridSizeY - 1) * backgroundGrid.backgroundTileSize * 0.5f - (2 * backgroundGrid.backgroundTileSize)
            );

            // Calculate the position for the current tile relative to the grid center
            Vector2 position = new Vector2(
                (column - 2) * backgroundGrid.backgroundTileSize,
                (row - 2) * backgroundGrid.backgroundTileSize
            ) + gridCenter;

            GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
            tile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
            movableTiles[column, row] = tile.transform;

            MovableTile tileData = tile.GetComponent<MovableTile>();
            tileData.Level = targetLevel;
            tileData.Row = row;
            tileData.Column = column;
            tileData.TileType = tileType;
            tileData.GridSizeX = gridSizeX;
            tileData.GridSizeY = gridSizeY;
        }
    }



    public void DestroyExistingMovableTiles()
    {
        // Clear the references in the movableTiles array.
        for (int x = 0; x < backgroundGrid.gridSizeX; x++)
        {
            for (int y = 0; y < backgroundGrid.gridSizeY; y++)
            {
                movableTiles[x, y] = null;
            }
        }

        // Find and destroy game objects with the "MovableTile" and "EvilTile" tags.
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("MovableTile");
        objectsToDestroy = objectsToDestroy.Concat(GameObject.FindGameObjectsWithTag("EvilTile")).ToArray();

        // Loop through and destroy each GameObject
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }

        // Generate new movable tiles (and evil tiles if needed).
        ReadLevelDataFromCSV(csvFile);
    }

    public Transform[,] GetMovableTiles()
    {
        return movableTiles;
    }

    public void EmptyMovableTilesArrayRowOrColumn(Transform[,] currentMovableTiles)
    {
        //empty row or column that has been moved
        for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
        {
            for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
            {
                Transform cTile = currentMovableTiles[i, j];
                if (cTile != null && (cTile.CompareTag("MovableTile") || cTile.CompareTag("EvilTile")))
                {
                    movableTiles[i, j] = null;
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

        Transform tile = movableTiles[column, row];

        if (tile != null)
        {
            // Get the MovableTile component of the tile.
            MovableTile movableTileComponent = tile.GetComponent<MovableTile>();
            movableTileComponent.Row = row;
            movableTileComponent.Column = column;
        }

    }

    public Transform[,] UpdateMovableTilesArray()
    {

        for (int row = 0; row < backgroundGrid.gridSizeY; row++)
        {
            for (int col = 0; col < backgroundGrid.gridSizeX; col++)
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

                if (tile != null && (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile")) && CheckNeighbours(col, row) != true)
                {
                    //no neighbors found, destroy tile
                    Debug.Log("destroy tile " + col + " , " + row);

                    movableTiles[col, row] = null;

                    GameObject tileToDestroy = tile.gameObject; // Get the GameObject.
                    Destroy(tileToDestroy);

                    if(tile.CompareTag("MovableTile"))
                    {
                        Debug.Log("level failed");
                    }
                    else
                    {
                        if(CountEvilTiles() == 0)
                        {
                            Debug.Log("level completed, evil tiles count: " + CountEvilTiles());
                        }
                        
                    }
                }
               
            }
        }

        return movableTiles;
    }

    private int CountEvilTiles()
    {
        int count = 0;

        for (int row = 0; row < backgroundGrid.gridSizeY; row++)
        {
            for (int col = 0; col < backgroundGrid.gridSizeX; col++)
            {
                Transform movableTile = movableTiles[col, row];

                if (movableTile != null)
                {
                    MovableTile movableTileComponent = movableTile.GetComponent<MovableTile>();

                    if (movableTileComponent != null && movableTileComponent.TileType == "Evil")
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }


    private bool CheckNeighbours(int col, int row)
    {
        bool hasMovableTile = false;

        // Check the right neighbor
        if (col < backgroundGrid.gridSizeX - 1)
        {
            Transform rightNeighbor = movableTiles[col + 1, row];
            if (rightNeighbor != null && (rightNeighbor.CompareTag("MovableTile") || rightNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        // Check the left neighbor
        if (col > 0)
        {
            Transform leftNeighbor = movableTiles[col - 1, row];
            if (leftNeighbor != null && (leftNeighbor.CompareTag("MovableTile") || leftNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        // Check the upper neighbor
        if (row < backgroundGrid.gridSizeY - 1)
        {
            Transform upperNeighbor = movableTiles[col, row + 1];
            if (upperNeighbor != null && (upperNeighbor.CompareTag("MovableTile") || upperNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        // Check the lower neighbor
        if (row > 0)
        {
            Transform lowerNeighbor = movableTiles[col, row - 1];
            if (lowerNeighbor != null && (lowerNeighbor.CompareTag("MovableTile") || lowerNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        return hasMovableTile;
    }

    public Transform[,] FindAdjacentMovableTilesInRow(int rowIndex)
    {
        Debug.Log("rowindex in findmovable " + rowIndex);

        int numRows = movableTiles.GetLength(1);
        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        Transform[,] tilesInRow = new Transform[numCols, numRows];

        bool foundAdjacentTile = false; // Flag to track if an adjacent tile has been found.

        if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(1))
        {
            for (int col = 0; col < numCols; col++)
            {
                if (movableTiles[col, rowIndex] != null)
                {
                    // Check if the tile has an adjacent tile to the left or right.
                    bool hasLeftAdjacent = (col > 0 && movableTiles[col - 1, rowIndex] != null);
                    bool hasRightAdjacent = (col < numCols - 1 && movableTiles[col + 1, rowIndex] != null);

                    if (hasLeftAdjacent || hasRightAdjacent)
                    {
                        // Include this tile in the movable row.
                        tilesInRow[col, rowIndex] = movableTiles[col, rowIndex];
                        foundAdjacentTile = true; // Set the flag to true.

                    }
                    else if (foundAdjacentTile)
                    {
                        // Exclude this tile as it's alone after an adjacent tile.
                        tilesInRow[col, rowIndex] = null;
                    }
                }
            }
        }

        return tilesInRow;
    }




    // Function to find and return movable tiles in a specified column.
    public Transform[,] FindAdjacentMovableTilesInColumn(int columnIndex)
    {
        Debug.Log("colindex in findmovable " + columnIndex);

        int numRows = movableTiles.GetLength(1); // Assuming movableTiles is in [col, row] format.
        int numCols = movableTiles.GetLength(0);
        Transform[,] tilesInColumn = new Transform[numCols, numRows];

        bool foundAdjacentTile = false; // Flag to track if an adjacent tile has been found.

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
                        tilesInColumn[columnIndex, row] = movableTiles[columnIndex, row];
                        foundAdjacentTile = true; // Set the flag to true.
                    }
                    else if (foundAdjacentTile)
                    {
                        // Exclude this tile as it's alone after an adjacent tile.
                        tilesInColumn[columnIndex, row] = null;
                    }
                }
            }
        }

        return tilesInColumn;
    }



    // Function to find and return movable tiles in a specified row.
    public Transform[,] FindAllMovableTilesInRow(int rowIndex)
    {
        Debug.Log("rowindex in findmovable " + rowIndex);

        int numRows = movableTiles.GetLength(1);
        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        Transform[,] tilesInRow = new Transform[numCols, numRows];

        if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(1))
        {
            for (int col = 0; col < numCols; col++)
            {

                tilesInRow[col, rowIndex] = movableTiles[col, rowIndex] != null ? movableTiles[col, rowIndex].transform : null;

            }
        }

        return tilesInRow;
    }


    // Function to find and return movable tiles in a specified column.
    public Transform[,] FindAllMovableTilesInColumn(int columnIndex)
    {
        Debug.Log("colindex in findmovable " + columnIndex);

        int numRows = movableTiles.GetLength(1); // Assuming movableTiles is in [col, row] format.
        int numCols = movableTiles.GetLength(0);
        Transform[,] tilesInColumn = new Transform[numCols, numRows];

        if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(0))
        {
            for (int row = 0; row < numRows; row++)
            {

                tilesInColumn[columnIndex, row] = movableTiles[columnIndex, row] != null ? movableTiles[columnIndex, row].transform : null;

            }
        }

        return tilesInColumn;
    }

    public bool IsMovableTilesGroupConnected()
    {
        // Create a 2D array to keep track of visited tiles.
        bool[,] visited = new bool[backgroundGrid.gridSizeX, backgroundGrid.gridSizeY];

        // Find the first MovableTile as the starting point for traversal.
        Transform startTile = null;
        for (int x = 0; x < backgroundGrid.gridSizeX; x++)
        {
            for (int y = 0; y < backgroundGrid.gridSizeY; y++)
            {
                Transform tile = movableTiles[x, y];
                if (tile != null && (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile")))
                {
                    startTile = tile;
                    break;
                }
            }
            if (startTile != null) break;
        }

        if (startTile == null) return true; // No MovableTiles are present, group is still "connected."

        // Perform a depth-first search (DFS) to traverse and mark connected tiles.
        bool result = DepthFirstSearch(startTile, visited);

        // Check if there are any unvisited MovableTiles.
        for (int x = 0; x < backgroundGrid.gridSizeX; x++)
        {
            for (int y = 0; y < backgroundGrid.gridSizeY; y++)
            {
                Transform tile = movableTiles[x, y];
                if (tile != null && (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile")) && !visited[x, y])
                {
                    result = false; // There's an unvisited tile, group is not connected.
                    break;
                }
            }
            if (!result) break;
        }

        if (!result)
        {
            Debug.Log("Game Over: MovableTiles group is not connected.");
        }

        return result;
    }

    private bool DepthFirstSearch(Transform tile, bool[,] visited)
    {
        int x = tile.GetComponent<MovableTile>().Column;
        int y = tile.GetComponent<MovableTile>().Row;

        visited[x, y] = true; // Mark the tile as visited.

        bool result = true; // Initialize the result as true.

        // Check neighbors and perform DFS on connected MovableTiles.
        if (x > 0 && !visited[x - 1, y])
        {
            Transform leftNeighbor = movableTiles[x - 1, y];
            if (leftNeighbor != null && (leftNeighbor.CompareTag("MovableTile") || leftNeighbor.CompareTag("EvilTile")))
            {
                result &= DepthFirstSearch(leftNeighbor, visited);
            }
        }

        if (x < backgroundGrid.gridSizeX - 1 && !visited[x + 1, y])
        {
            Transform rightNeighbor = movableTiles[x + 1, y];
            if (rightNeighbor != null && (rightNeighbor.CompareTag("MovableTile") || rightNeighbor.CompareTag("EvilTile")))
            {
                result &= DepthFirstSearch(rightNeighbor, visited);
            }
        }

        if (y > 0 && !visited[x, y - 1])
        {
            Transform lowerNeighbor = movableTiles[x, y - 1];
            if (lowerNeighbor != null && (lowerNeighbor.CompareTag("MovableTile") || lowerNeighbor.CompareTag("EvilTile")))
            {
                result &= DepthFirstSearch(lowerNeighbor, visited);
            }
        }

        if (y < backgroundGrid.gridSizeY - 1 && !visited[x, y + 1])
        {
            Transform upperNeighbor = movableTiles[x, y + 1];
            if (upperNeighbor != null && (upperNeighbor.CompareTag("MovableTile") || upperNeighbor.CompareTag("EvilTile")))
            {
                result &= DepthFirstSearch(upperNeighbor, visited);
            }
        }

        return result;
    }



}

