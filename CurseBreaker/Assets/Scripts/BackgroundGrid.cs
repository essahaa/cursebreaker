using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundGrid : MonoBehaviour
{
    public Transform[,] backgroundGrid;
    public GameObject backgroundTilePrefab;
    
    public float gridMargin = 5.0f; //margin in percentages of screen width
    public float backgroundTileSize; //Calculated based on screen size
    private float gridWidth;

    // Define the boundaries of the play area.
    public float minX; // Calculated based on screen size
    public float maxX;
    public float minY;
    public float maxY;

    public Vector2 startPosition;

    public void CalculateTileSize(int gridSizeX, int gridSizeY)
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
            gridWidth = screenWorldWidth - marginsWidth;
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
        CalculateTileSize(gridSizeX, gridSizeY);

        backgroundGrid = new Transform[gridSizeX, gridSizeY];

        float tileWidth = backgroundTileSize;
        float tileHeight = backgroundTileSize;

        // Calculate the position of the middle tile based on gridSizeX and gridSizeY
        float middleX = (gridSizeX - 1) * 0.5f * tileWidth;
        float middleY = (gridSizeY - 1) * 0.5f * tileHeight;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the position for the current tile relative to the middle point of the grid
                Vector2 position = new Vector2(
                    x * tileWidth - middleX,
                    y * tileHeight - middleY
                );

                GameObject tile = Instantiate(backgroundTilePrefab, position, Quaternion.identity);
                tile.transform.localScale = new Vector3(backgroundTileSize, backgroundTileSize, 1); // Scales the tile sprite

                // Store the tile in the grid array
                backgroundGrid[x, y] = tile.transform;

                if((x == 0) || (y == 0) || (x == gridSizeX - 1) || (y == gridSizeY - 1))
                {
                    SetSpriteTransparency(backgroundGrid[x, y].transform);
                }
            }
        }
    }

    void SetSpriteTransparency(Transform tile)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Get the current color from the material
            Color currentColor = spriteRenderer.material.color;

            // Set the new alpha value
            currentColor.a = 0.2f;

            // Update the material color with the new alpha value
            spriteRenderer.material.color = currentColor;
        }
    }

}
