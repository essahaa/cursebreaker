using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTileGrid : MonoBehaviour
{
    public GameObject movableTilePrefab;
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

    void GenerateMovableTiles()
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
                movableTiles[x, y] = tile.transform;

                // Assign row and column indices to the tile
                tile.GetComponent<MovableTile>().Row = y;
                tile.GetComponent<MovableTile>().Column = x;
            }
        }

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
                if (cTile != null && cTile.CompareTag("MovableTile"))
                {
                    Debug.Log("currentmovables array, tile found: " + i + " , " + j);
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

                if (tile != null && tile.CompareTag("MovableTile"))
                {
                    // Update the movableTiles array.
                    movableTiles[col, row] = tile;
                    Debug.Log("updating array, tile found: " + col + " , " + row);
                }
                else
                {
                    movableTiles[col, row] = null;
                }
            }
        }

        return movableTiles;
    }


    public Transform GetTileAtPosition(int column, int row)
    {

        // Ensure row and column are within valid bounds.
        if (row >= 0 && row < backgroundGrid.gridSizeY && column >= 0 && column < backgroundGrid.gridSizeX)
        {
            Transform tile = movableTiles[column, row];

            // Check if the tile is not null and has the "MovableTile" tag.
            if (tile != null && tile.CompareTag("MovableTile"))
            {
                return tile;
            }
        }

        // Return null if the tile doesn't meet the criteria.
        return null;
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

