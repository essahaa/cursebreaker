using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGrid : MonoBehaviour
{
    public Transform[,] backgroundGrid;
    public GameObject backgroundTilePrefab;
    public int gridSizeX = 5; //number of columns, width of the grid
    public int gridSizeY = 5; //number of rows, height of the grid
    public float backgroundTileSize = 1.2f;

    // Define the boundaries of the play area.
    public float minX = -2.4f; // Adjust these values as needed.
    public float maxX = 2.4f;
    public float minY = -2.4f;
    public float maxY = 2.4f;

    public Vector2 startPosition;

    void Start()
    {
        GenerateBackgroundGrid(gridSizeX, gridSizeY);
    }

    public void GenerateBackgroundGrid(int gridSizeX, int gridSizeY)
    {
        backgroundGrid = new Transform[gridSizeX, gridSizeY];

        float tileWidth = backgroundTileSize; // Use the adjusted background tile size.
        float tileHeight = backgroundTileSize;

        float totalWidth = gridSizeX * tileWidth;
        float totalHeight = gridSizeY * tileHeight;
        
        Vector2 startPosition = new Vector2(
            -(totalWidth - tileWidth) / 2,
            -(totalHeight - tileHeight) / 2
        );

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + x * tileWidth,
                    startPosition.y + y * tileHeight
                );

                GameObject tile = Instantiate(backgroundTilePrefab, position, Quaternion.identity);

                // Store the tile in the grid array
                backgroundGrid[x, y] = tile.transform;

                //backgroundGrid[x, y] accesses the tile at column x and row y
            }
        }

    }

    // Function to get the position of a background grid tile based on its column index.
    public Vector2 GetColumnPosition(int col)
    {
        float tileWidth = backgroundTileSize;
        float x = -(tileWidth * (gridSizeX - 1)) / 2 + col * tileWidth;
        return new Vector2(x, 0f); // Assuming the grid is at y=0.
    }

    // Function to get the position of a background grid tile based on its row index.
    public Vector2 GetRowPosition(int row)
    {
        float tileHeight = backgroundTileSize;
        float y = -(tileHeight * (gridSizeY - 1)) / 2 + row * tileHeight;
        return new Vector2(0f, y); // Assuming the grid is at x=0.
    }

    
    // Function to get the position of a background grid tile based on its row and column index.
    public Vector2 GetTilePosition(int row, int col)
    {
        float tileWidth = backgroundTileSize;
        float tileHeight = backgroundTileSize;

        float x = -(tileWidth * (gridSizeX - 1)) / 2 + col * tileWidth;
        float y = -(tileHeight * (gridSizeY - 1)) / 2 + row * tileHeight;

        return new Vector2(x, y);
    }
    
}
