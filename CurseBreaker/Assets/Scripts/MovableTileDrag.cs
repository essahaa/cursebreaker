using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableTileDrag : MonoBehaviour
{
    public BackgroundGrid backgroundGrid;
    public MovableTileGrid movableTileGrid;
    public TutorialLevel tutorialLevel;

    private Vector3 initialMousePosition;
    private Vector3 initialTilePosition;

    private string currentMoveType = "horizontal"; // Initialize with a horizontal move

    private bool isDragging = false;
    private bool allElementsNull = true; //used for checking if currentmovables array has non-null tiles
    private bool tileInSamePosition = false;

    private Vector3[,] initialTilePositions;
    private Transform[,] movableTiles; 
    public Transform[,] currentMovableTiles;

    private GameObject selectedTile;
    private int rowIndex;
    private int columnIndex;

    private int selectedLevel;

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();

        movableTiles = movableTileGrid.movableTiles;
        selectedLevel = movableTileGrid.CheckSelectedLevel();
    }

    private void OnMouseDown()
    {
        // Raycast to detect which tile was clicked.
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero); 

        if (hit.collider != null && selectedLevel != 1)
        {
            //initialMousePosition = hit.transform.position;
            // Check if the clicked object is a movable tile.
            if (hit.collider.gameObject.CompareTag("MovableTile") || hit.collider.gameObject.CompareTag("EvilTile"))
            {
                selectedTile = hit.collider.gameObject;

                //get the row and column of the selected tile from MovableTile component
                rowIndex = selectedTile.GetComponent<MovableTile>().Row;
                columnIndex = selectedTile.GetComponent<MovableTile>().Column;

                // Find all the movable tiles in the specified row or column.
                if (currentMoveType == "horizontal")
                {
                    currentMovableTiles = movableTileGrid.FindAdjacentMovableTilesInRow(rowIndex);
                }
                else if (currentMoveType == "vertical")
                {
                    currentMovableTiles = movableTileGrid.FindAdjacentMovableTilesInColumn(columnIndex);
                }


                //checks if there are non null objects in current movables array
                for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
                {
                    for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
                    {
                        if (currentMovableTiles[i, j] != null)
                        {
                            allElementsNull = false;

                            MovableTile movableTile = currentMovableTiles[i, j].GetComponent<MovableTile>();

                            if (movableTile != null)
                            {
                                SpriteRenderer spriteRenderer = movableTile.gameObject.GetComponent<SpriteRenderer>();
                                if (movableTile.TileType == "Normal")
                                {
                                    spriteRenderer.sprite = movableTileGrid.glowingTile;  
                                }
                                else if (movableTile.TileType == "Evil")
                                {
                                    spriteRenderer.sprite = movableTileGrid.glowingTileEvil;
                                }
                            }
                        }
                    }
                }

                if (!allElementsNull)
                {
                    initialTilePositions = new Vector3[currentMovableTiles.GetLength(0), currentMovableTiles.GetLength(1)];

                    // Store the initial positions of all movable tiles in the row or column.
                    for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
                    {
                        for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                        {
                            if (currentMovableTiles[col, row] != null)
                            {
                                initialTilePositions[col, row] = currentMovableTiles[col, row].position;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("currentmovabletiles null");
                }

                isDragging = true;
            }
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging && !allElementsNull && selectedLevel != 1)
        {
            FindObjectOfType<AudioManager>().Play("liik");

            // Calculate the offset based on the initial mouse and tile positions.
            Vector3 mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = mouseCurrentPos - initialMousePosition;

            // Iterate through the tiles in the row or column.
            for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
            {
                for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                {
                    Transform tile = currentMovableTiles[col, row];

                    if (tile != null)
                    {
                        Vector3 targetPosition;

                        if (currentMoveType == "horizontal")
                        {
                            targetPosition = initialTilePositions[col, row] + new Vector3(offset.x, 0f, 0f);
                            targetPosition.x = Mathf.Clamp(targetPosition.x, backgroundGrid.minX, backgroundGrid.maxX);
                        }
                        else
                        {
                            targetPosition = initialTilePositions[col, row] + new Vector3(0f, offset.y, 0f);
                            targetPosition.y = Mathf.Clamp(targetPosition.y, backgroundGrid.minY, backgroundGrid.maxY);
                        }

                        foreach (Transform otherTile in currentMovableTiles)
                        {
                            if (otherTile != null && otherTile != tile)
                            {
                                Vector3 otherTilePosition = otherTile.position;

                                if (otherTilePosition == targetPosition)
                                {
                                    tileInSamePosition = true;
                                    Debug.Log("There is a tile at the target snapped position.");
                                }
                            }
                        }

                        if (!tileInSamePosition)
                        {
                            // Update the tile's position to the target position.
                            tile.position = targetPosition;

                            // Calculate the new column and row based on the target position.
                            int column = Mathf.RoundToInt((targetPosition.x - backgroundGrid.minX) / backgroundGrid.backgroundTileSize);
                            int newRow = Mathf.RoundToInt((targetPosition.y - backgroundGrid.minY) / backgroundGrid.backgroundTileSize);

                            // Update the movableTileGrid with the new position.
                            movableTileGrid.UpdateMovableTile(column, newRow, tile);
                        }
                    }
                }
            }
        }

    }

    private void OnMouseUp()
    {
        isDragging = false;
        bool isSnappedToNewPlace = false;

        if (!allElementsNull && selectedLevel != 1)
        {
            if (tileInSamePosition)
            {
                for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
                {
                    for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                    {
                        Transform tile = currentMovableTiles[col, row];

                        if (tile != null)
                        {
                            tile.position = initialTilePositions[col, row];
                            // Reset the movableTile to their original column and row.
                            movableTileGrid.UpdateMovableTile(col, row, tile);
                        }
                    }
                }

            }
            //empty the currently moved row or column from movabletiles array so that it can be replaced by new positions
            movableTiles = (currentMoveType == "horizontal") ? movableTileGrid.FindAllMovableTilesInRow(rowIndex) : movableTileGrid.FindAllMovableTilesInColumn(columnIndex);
            movableTileGrid.EmptyMovableTilesArrayRowOrColumn(movableTiles);

            // Snap each tile to the nearest grid position based on rows and columns.
            foreach (Transform tile in currentMovableTiles)
            {
                if (tile != null)
                {
                    MovableTile movableTileComponent = tile.GetComponent<MovableTile>();

                    // Calculate the target snapped position based on row and column.
                    float targetX = movableTileComponent.Column * backgroundGrid.backgroundTileSize + backgroundGrid.minX;
                    float targetY = movableTileComponent.Row * backgroundGrid.backgroundTileSize + backgroundGrid.minY;

                    // Ensure the snapped position stays within the background grid boundaries.
                    targetX = Mathf.Clamp(targetX, backgroundGrid.minX, backgroundGrid.maxX);
                    targetY = Mathf.Clamp(targetY, backgroundGrid.minY, backgroundGrid.maxY);

                    // Set the tile's position to the target position.
                    tile.position = new Vector3(targetX, targetY, 0f);

                    // Update the movableTileGrid with the new position.
                    movableTileGrid.UpdateMovableTile(movableTileComponent.Column, movableTileComponent.Row, tile);
                    Debug.Log("Snapped to row: " + movableTileComponent.Row + ", column: " + movableTileComponent.Column + " tile position " + tile.position);

                    if (!tileInSamePosition)
                    {
                        isSnappedToNewPlace = true;
                    }
                    else
                    {
                        isSnappedToNewPlace = false;
                    }                   
                }
            }
         
            if (isSnappedToNewPlace)
            {
                //Update movableTiles array with new snapped positions
                movableTiles = movableTileGrid.UpdateMovableTilesArray();

                // Toggle between "horizontal" and "vertical" move types.
                currentMoveType = (currentMoveType == "horizontal") ? "vertical" : "horizontal";
                Debug.Log("movetype change: " + currentMoveType);
                movableTileGrid.IsMovableTilesGroupConnected();
            }
            else if(!isSnappedToNewPlace)
            {
                currentMoveType = (currentMoveType == "horizontal") ? "horizontal" : "vertical";
                Debug.Log("movetype still same: " + currentMoveType);
            }

            //empty current movable tiles array
            for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
            {
                for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
                {
                    if (currentMovableTiles[i, j] != null)
                    {
                        MovableTile movableTile = currentMovableTiles[i, j].GetComponent<MovableTile>();
                        // Check if the MovableTile component is not null
                        if (movableTile != null)
                        {
                            SpriteRenderer spriteRenderer = movableTile.gameObject.GetComponent<SpriteRenderer>();
                            if (movableTile.TileType == "Normal")
                            {
                                spriteRenderer.sprite = movableTileGrid.tile;
                            }
                            else if (movableTile.TileType == "Evil")
                            {
                                spriteRenderer.sprite = movableTileGrid.evilTile;
                            }
                        }
                        currentMovableTiles[i, j] = null;
                    }
                }
            }
            allElementsNull = false;
            tileInSamePosition = false;
        }
        
    }

}