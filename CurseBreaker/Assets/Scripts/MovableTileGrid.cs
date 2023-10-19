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

    public float movableTileSize = 1.0f; // Adjust the size of movable tiles.

    public int startColumn = 0; // Set the starting column index.
    public int startRow = 0;    // Set the starting row index.

    public Transform[,] movableTiles; // Change to a Transform[,] array.

    void Start()
    {
        backgroundGrid = GameObject.Find("BackgroundGridManager").GetComponent<BackgroundGrid>();
        movableTiles = new Transform[backgroundGrid.gridSizeX, backgroundGrid.gridSizeY]; // Use the size of the background grid.

        GenerateMovableTiles();

    }

    public void GenerateMovableTiles()
    {
        // Calculate the starting position for the entire grid
        float startX = -backgroundGrid.backgroundTileSize * (backgroundGrid.gridSizeX - 1) / 2;
        float startY = -backgroundGrid.backgroundTileSize * (backgroundGrid.gridSizeY - 1) / 2;

        for (int x = 1; x <= 3; x++)
        {
            for (int y = 1; y <= 3; y++)
            {
                Vector2 position = new Vector2(
                    startX + x * backgroundGrid.backgroundTileSize,
                    startY + y * backgroundGrid.backgroundTileSize
                );

                GameObject tile = Instantiate(movableTilePrefab, position, Quaternion.identity);
                tile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1); //scales the tile sprite
                movableTiles[x, y] = tile.transform;

                // Assign row and column indices to the tile
                tile.GetComponent<MovableTile>().Row = y;
                tile.GetComponent<MovableTile>().Column = x;
                tile.GetComponent<MovableTile>().TileType = "Normal";
            }
        }

        Vector2 position2 = new Vector2(
                    startX + 1 * backgroundGrid.backgroundTileSize,
                    startY + 1 * backgroundGrid.backgroundTileSize
                );
        // Instantiate the new tile prefab at the specified position.
        GameObject newTile = Instantiate(evilTilePrefab, position2, Quaternion.identity);
        newTile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1); //scales the tile sprite

        // Destroy the old movableTilePrefab you want to replace.
        Destroy(movableTiles[1, 1].gameObject);

        // Update the movableTiles array to reference the new tile's transform.
        movableTiles[1, 1] = newTile.transform;

        // Assign the row and column indices to the new tile.
        newTile.GetComponent<MovableTile>().Row = 1;
        newTile.GetComponent<MovableTile>().Column = 1;
        newTile.GetComponent<MovableTile>().TileType = "Evil";


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
        GenerateMovableTiles();
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
                    GameObject destroyTile = tile.gameObject; // Get the GameObject.
                    Destroy(destroyTile);
                }
               
            }
        }

        return movableTiles;
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



}

