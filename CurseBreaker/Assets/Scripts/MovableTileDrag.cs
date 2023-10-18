using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableTileDrag : MonoBehaviour
{
    public BackgroundGrid backgroundGrid;
    public MovableTileGrid movableTileGrid;

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

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();

        movableTiles = movableTileGrid.movableTiles;
    }

    private void OnMouseDown()
    {
        // Raycast to detect which tile was clicked.
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            // Check if the clicked object is a movable tile.
            if (hit.collider.gameObject.CompareTag("MovableTile"))
            {
                selectedTile = hit.collider.gameObject;

                //get the row and column of the selected tile from MovableTile component
                rowIndex = selectedTile.GetComponent<MovableTile>().Row;
                columnIndex = selectedTile.GetComponent<MovableTile>().Column;

                Debug.Log("rowindex " + selectedTile.GetComponent<MovableTile>().Row + " colindex " + selectedTile.GetComponent<MovableTile>().Column + " currentmovetype: " + currentMoveType);

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
                            break;  // No need to continue checking once a non-null element is found.
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
                                Debug.Log("position " + currentMovableTiles[col, row].position);
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
        if (isDragging && !allElementsNull)
        {
            FindObjectOfType<AudioManager>().Play("liik");

            // Calculate the offset based on the initial mouse and tile positions.
            Vector3 mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = mouseCurrentPos - initialMousePosition;

            if (currentMoveType == "horizontal")
            {
                // Horizontal movement (row)

                // Calculate the target position for each tile in the row.
                for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
                {
                    for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                    {
                        Transform tile = currentMovableTiles[col, row];

                        if (tile != null)
                        {
                            Vector3 targetPosition = initialTilePositions[col, row] + new Vector3(offset.x, 0f, 0f);

                            // Ensure the tile stays within the game board's boundaries.
                            targetPosition.x = Mathf.Clamp(targetPosition.x, backgroundGrid.minX, backgroundGrid.maxX); 


                            // Iterate through existing tiles to check for collisions.
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
                                    else
                                    {
                                        // Move the tile to the target position.
                                        tile.position = targetPosition;
                                    }
                                }
                            }
                        }

                        
                    }
                }
                
            }
            else
            {
                // Vertical movement (column)
                
                // Calculate the target position for each tile in the column.
                for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
                {
                    for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                    {
                        Transform tile = currentMovableTiles[col, row];

                        if (tile != null)
                        {
                            Vector3 targetPosition = initialTilePositions[col, row] + new Vector3(0f, offset.y, 0f);

                            // Ensure the tile stays within the game board's boundaries.
                            targetPosition.y = Mathf.Clamp(targetPosition.y, backgroundGrid.minY, backgroundGrid.maxY);

                            // Iterate through existing tiles to check for collisions.
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
                                    else
                                    {
                                        // Move the tile to the target position.
                                        tile.position = targetPosition;
                                    }
                                }
                            }
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

        if (!allElementsNull)
        {
            //empty the currently moved row or column from movabletiles array so that it can be replaced by new positions
            movableTiles = (currentMoveType == "horizontal") ? movableTileGrid.FindAllMovableTilesInRow(rowIndex) : movableTileGrid.FindAllMovableTilesInColumn(columnIndex);

            movableTileGrid.EmptyMovableTilesArrayRowOrColumn(movableTiles);

            // Snap each tile to the nearest grid position.
            foreach (Transform tile in currentMovableTiles)
            {
                if (tile != null) 
                {
                    Vector3 currentPosition = tile.position;

                    // Define the target snapped position for the current tile.
                    Vector3 targetSnappedPosition = new Vector3(
                        Mathf.Round(currentPosition.x / backgroundGrid.backgroundTileSize) * backgroundGrid.backgroundTileSize,
                        Mathf.Round(currentPosition.y / backgroundGrid.backgroundTileSize) * backgroundGrid.backgroundTileSize,
                        0f
                    );

                    // Ensure the snapped position stays within the background grid boundaries.
                    targetSnappedPosition.x = Mathf.Clamp(targetSnappedPosition.x, backgroundGrid.minX, backgroundGrid.maxX);
                    targetSnappedPosition.y = Mathf.Clamp(targetSnappedPosition.y, backgroundGrid.minY, backgroundGrid.maxY);

                    int column, row;
                    MovableTile movableTileComponent = tile.GetComponent<MovableTile>();

                    if (!tileInSamePosition)
                    {
                        column = Mathf.RoundToInt((targetSnappedPosition.x - backgroundGrid.minX) / backgroundGrid.backgroundTileSize);
                        row = Mathf.RoundToInt((targetSnappedPosition.y - backgroundGrid.minY) / backgroundGrid.backgroundTileSize);

                        tile.position = targetSnappedPosition;

                        // Ensure that row and column are within valid bounds
                        if (column >= 0 && column < backgroundGrid.gridSizeX && row >= 0 && row < backgroundGrid.gridSizeY)
                        {
                            movableTileGrid.UpdateMovableTile(column, row, movableTileComponent.transform);

                            Debug.Log("Snapped to row: " + row + ", column: " + column + " tile position " + tile.position);
                            Debug.Log("movabletilecomponent.Row " + movableTileComponent.Row + " movabletilecomponent.Column " + movableTileComponent.Column);
                        }

                        isSnappedToNewPlace = true;
                    }
                    else
                    {
                        tile.position = initialTilePositions[movableTileComponent.Column, movableTileComponent.Row];
                        movableTileGrid.UpdateMovableTile(movableTileComponent.Column, movableTileComponent.Row, movableTileComponent.transform);
                        
                        Debug.Log("tile position in snap " + tile.position + "movabletilecomponent.Row " + movableTileComponent.Row + " movabletilecomponent.Column " + movableTileComponent.Column);

                        isSnappedToNewPlace = false;

                    }                  
                }
            }

            if(isSnappedToNewPlace)
            {
                //Update movableTiles array with new snapped positions
                movableTiles = movableTileGrid.UpdateMovableTilesArray();

                // Toggle between "horizontal" and "vertical" move types.
                currentMoveType = (currentMoveType == "horizontal") ? "vertical" : "horizontal";
                Debug.Log("movetype change: " + currentMoveType);
            }
            else
            {
                currentMoveType = (currentMoveType == "horizontal") ? "horizontal" : "vertical";
                Debug.Log("movetype still same: " + currentMoveType);
            }

            //empty current movable tiles array
            for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
            {
                for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
                {
                    currentMovableTiles[i, j] = null;
                }
            }

            allElementsNull = false;
            tileInSamePosition = false;
            
        }
    }
}