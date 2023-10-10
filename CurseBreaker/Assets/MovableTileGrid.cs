using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTileGrid : MonoBehaviour
{
    public GameObject movableTilePrefab;
    public BackgroundGrid backgroundGrid;
    //public RowColumnManager rowColumnManager;

    public float movableTileSize = 1.0f; // Adjust the size of movable tiles.

    public int startColumn = 0; // Set the starting column index.
    public int startRow = 0;    // Set the starting row index.

    public Transform[,] movableTiles; // Change to a Transform[,] array.

    void Start()
    {
        // Find the GameObject with the name "BackgroundGridManager" and get its BackgroundGrid component.
        backgroundGrid = GameObject.Find("BackgroundGridManager").GetComponent<BackgroundGrid>();
        //rowColumnManager = GameObject.Find("RowColumnManager").GetComponent<RowColumnManager>();
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

        // After generating the grid...
        //rowColumnManager.InitializeRowColumnDataFromTiles(movableTiles);
    }

    public Transform[,] GetMovableTiles()
    {
        //UpdateMovableTilesArray(); // Call a method to update the array if needed.
        if (movableTiles != null)
        {
            /*
            // Assuming 'array' is your 2D array of type Transform[,]
            for (int i = 0; i < movableTiles.GetLength(0); i++)
            {
                for (int j = 0; j < movableTiles.GetLength(1); j++)
                {
                    Transform element = movableTiles[i, j];

                    if (element != null)
                    {
                        Debug.Log($"movables after update [col, row] ({i}, {j}): {element.name}");
                    }
                    else
                    {
                        Debug.Log($"tile at ({i}, {j}): null");
                    }
                }
            }*/
        }
        return movableTiles;
    }

    public void UpdateMovableTile(int column, int row, Transform newTile)
    {
        // Empty the cell first by setting it to null.
        movableTiles[column, row] = null;
        // Update the cell with the new tile.
        movableTiles[column, row] = newTile;

        Transform tile = movableTiles[column, row];

        if(tile != null)
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
                Transform tile = GetTileAtPosition(col, row);

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

    // Function to find and return movable tiles in a specified row.
    public Transform[,] FindMovableTilesInRow(int rowIndex)
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
    public Transform[,] FindMovableTilesInColumn(int columnIndex)
    {
        Debug.Log("colindex in findmovable " + columnIndex);

        int numRows = movableTiles.GetLength(1); // Assuming movableTiles is in [col, row] format.
        int numCols = movableTiles.GetLength(0);
        Transform[,] tilesInColumn = new Transform[numCols, numRows];

        if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(0))
        {
            for (int row = 0; row < numRows; row++)
            {
                
                    tilesInColumn[columnIndex, row] = movableTiles[columnIndex, row] != null ?movableTiles[columnIndex, row].transform : null;
                
            }
        }

        return tilesInColumn;
    }

    public bool HasAdjacentMovableTilesInRow(int columnIndex, int rowIndex)
    {
        int adjacentMovableTilesCount = 0;

        // Define the raycast direction as right.
        Vector2 raycastDirection = Vector2.right;

        // Perform a raycast from the current tile's position in the specified direction.
        RaycastHit2D hit = Physics2D.Raycast(movableTiles[columnIndex, rowIndex].position, raycastDirection);

        // Check for adjacent movable tiles to the right.
        while (hit.collider != null && hit.collider.CompareTag("MovableTile"))
        {
            adjacentMovableTilesCount++;
            // Cast another ray to the right from the last hit position.
            hit = Physics2D.Raycast(hit.transform.position, raycastDirection);
        }

        // Reset the raycast direction as left.
        raycastDirection = Vector2.left;

        // Perform a raycast from the current tile's position in the specified direction.
        hit = Physics2D.Raycast(movableTiles[columnIndex, rowIndex].position, raycastDirection);

        // Check for adjacent movable tiles to the left.
        while (hit.collider != null && hit.collider.CompareTag("MovableTile"))
        {
            adjacentMovableTilesCount++;
            // Cast another ray to the left from the last hit position.
            hit = Physics2D.Raycast(hit.transform.position, raycastDirection);
        }

        return adjacentMovableTilesCount >= 2;
    }

    public bool HasAdjacentMovableTilesInColumn(int columnIndex, int rowIndex)
    {
        int adjacentMovableTilesCount = 0;

        // Define the raycast direction as up.
        Vector2 raycastDirection = Vector2.up;

        // Perform a raycast from the current tile's position in the specified direction.
        RaycastHit2D hit = Physics2D.Raycast(movableTiles[columnIndex, rowIndex].position, raycastDirection);

        // Check for adjacent movable tiles above.
        while (hit.collider != null && hit.collider.CompareTag("MovableTile"))
        {
            adjacentMovableTilesCount++;
            // Cast another ray upwards from the last hit position.
            hit = Physics2D.Raycast(hit.transform.position, raycastDirection);
        }

        // Reset the raycast direction as down.
        raycastDirection = Vector2.down;

        // Perform a raycast from the current tile's position in the specified direction.
        hit = Physics2D.Raycast(movableTiles[columnIndex, rowIndex].position, raycastDirection);

        // Check for adjacent movable tiles below.
        while (hit.collider != null && hit.collider.CompareTag("MovableTile"))
        {
            adjacentMovableTilesCount++;
            // Cast another ray downwards from the last hit position.
            hit = Physics2D.Raycast(hit.transform.position, raycastDirection);
        }

        return adjacentMovableTilesCount >= 2;
    }

    


}

