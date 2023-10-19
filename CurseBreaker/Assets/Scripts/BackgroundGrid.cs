using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundGrid : MonoBehaviour
{
    public Transform[,] backgroundGrid;
    public GameObject backgroundTilePrefab;
    public int gridSizeX = 5; //number of columns, width of the grid
    public int gridSizeY = 5; //number of rows, height of the grid
    public float gridMargin = 5.0f; //margin in percentages of screen width
    public float backgroundTileSize; //Calculated based on screen size

    // Define the boundaries of the play area.
    public float minX; // Calculated based on screen size
    public float maxX;
    public float minY;
    public float maxY;

    public Vector2 startPosition;

    void Start()
    {
        CalculateTileSize();
        GenerateBackgroundGrid(gridSizeX, gridSizeY);
    }

    public void CalculateTileSize()
    {
        Camera mainCamera = Camera.main;

        if(mainCamera != null)
        {
            //calculate screen width in world units
            float screenWorldWidth = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - mainCamera.ScreenToWorldPoint(Vector3.zero).x;

            //calculate width of both margins
            float marginsPixelWidth = (gridMargin * 2 / 100) * Screen.width;
            float marginsWidth = mainCamera.ScreenToWorldPoint(new Vector3(marginsPixelWidth, 0, 0)).x - mainCamera.ScreenToWorldPoint(Vector3.zero).x;

            //grid width and tile width
            float gridWidth = screenWorldWidth - marginsWidth;
            backgroundTileSize = gridWidth / gridSizeX;

            //set boundaries
            float boundary = gridWidth / 2 - backgroundTileSize / 2;
            minX = -boundary;
            maxX = boundary;
            minY = -boundary;
            maxY = boundary;
        }
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
                tile.transform.localScale = new Vector3(backgroundTileSize, backgroundTileSize, 1); //scales the tile sprite

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
