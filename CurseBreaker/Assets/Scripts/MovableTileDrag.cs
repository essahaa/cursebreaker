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

    private string currentMoveType = "horizontal"; // Initialize with a horizontal move

    private bool allElementsNull = true; //used for checking if currentmovables array has non-null tiles
    private bool tileInSamePosition = false;
    private bool targetPositionInLock = false;

    private Vector3[,] initialTilePositions;
    private Transform[,] movableTiles;
    public Transform[,] currentMovableTiles;

    private int rowIndex;
    private int columnIndex;

    private Vector3 initialTouchPosition;
    private Vector3 currentTouchPosition;
    private Vector3 offset;

    public float tapCooldown = 0.5f; // Time in seconds to ignore taps after the first tap
    private float lastTapTime;


    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();

        movableTiles = movableTileGrid.movableTiles;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Store the initial touch position
                    if (Time.time - lastTapTime > tapCooldown)
                    {
                        initialTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        currentMovableTiles = GetCurrentMovableTiles(touch.position);
                        lastTapTime = Time.time;
                    }
                    break;

                case TouchPhase.Moved:
                    // Update the current touch position and calculate the offset
                    currentTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    offset = currentTouchPosition - initialTouchPosition;

                    // Now call MoveTile or MoveTiles method with the calculated offset
                    MoveTiles(offset);
                    break;

                case TouchPhase.Ended:
                    SnapTilesToGrid();
                    break;
            }
        }
    }

    private Transform[,] GetCurrentMovableTiles(Vector2 screenPosition)
    {
        // Convert the screen position to a world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0; // Set Z to 0 if your game is 2D

        // Perform a raycast to see if we hit a tile
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject tile = hit.collider.gameObject;
            Debug.Log(tile);
            // Check if the object hit is a tile
            if (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile") || tile.CompareTag("KeyTile"))
            {
                Debug.Log(tile + " in if");
                rowIndex = tile.GetComponent<MovableTile>().Row;
                columnIndex = tile.GetComponent<MovableTile>().Column;

                Debug.Log(tile + " row, col " + rowIndex + columnIndex);

                // Find all the movable tiles in the specified row or column.
                if (currentMoveType == "horizontal")
                {
                    currentMovableTiles = movableTileGrid.FindAdjacentMovableTilesInRow(rowIndex);                
                }
                else if (currentMoveType == "vertical")
                {
                    currentMovableTiles = movableTileGrid.FindAdjacentMovableTilesInColumn(columnIndex);                   
                }

                CheckNonNullAndChangeSprites();
                SetInitialTilePositions();

                return currentMovableTiles;
            }
        }
        allElementsNull = true;
        return null; // Return null if no row or column of tiles was found
    }

    private void CheckNonNullAndChangeSprites()
    {
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
                        Animator animator = movableTile.gameObject.GetComponent<Animator>();
                        animator.SetBool("isGlowing", true);
                    }
                }
            }
        }
    }

    private void SetInitialTilePositions()
    {
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
    }

    private void MoveTiles(Vector3 offset)
    {
        if (!allElementsNull)
        {
           

            int [] firstMovableTile = getFirstAndLastMovableTile();

            // Iterate through the tiles in the row or column.
            for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
            {
                //Debug.Log("iterating row " + row);
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

                        CheckForLockCollition(targetPosition);

                        if (!tileInSamePosition && !targetPositionInLock)
                        {
                            // Update the tile's position to the target position.
                            tile.position = targetPosition;

                            // Calculate the new column and row based on the target position.
                            int column = Mathf.RoundToInt((targetPosition.x - backgroundGrid.minX) / backgroundGrid.backgroundTileSize);
                            int newRow = Mathf.RoundToInt((targetPosition.y - backgroundGrid.minY) / backgroundGrid.backgroundTileSize);

                            // Update the movableTileGrid with the new position.
                            movableTileGrid.UpdateMovableTile(column, newRow, tile);
                        }else if(targetPositionInLock)
                        {
                            targetPositionInLock = false;
                        }
                    }
                }
            }
        }
    }

    private int[] getFirstAndLastMovableTile()
    {
        //Array allTiles = new Array();
        for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
        {
            for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
            {
                Transform tile = currentMovableTiles[col, row];

                if (tile != null)
                {
                    int[] newArray = { row, col };
                    return newArray;
                }
            }
        }
        return null;
    }

    private void CheckForLockCollition(Vector3 targetPosition)
    {
        List<BoxCollider2D> lockTileColliders = GetLockColliders();

        foreach (BoxCollider2D collider in lockTileColliders)
        {
            if(collider.bounds.Contains(targetPosition)) {
                //return true;
                targetPositionInLock = true; break;
            }
        }
        //return false;
    }

    private List<BoxCollider2D> GetLockColliders()
    {
        List<BoxCollider2D> lockTileColliders = new List<BoxCollider2D>();

        foreach (Transform tileInGrid in movableTiles)
        {
            //Debug.Log("tileInGrid: " + tileInGrid);
            if (tileInGrid != null && tileInGrid.childCount > 0)
            {
                //Debug.Log("child found");
                Transform tileChild = tileInGrid.GetChild(0);
                if (tileChild.CompareTag("LockTile"))
                {
                    //Debug.Log("lock found");
                    lockTileColliders.Add(tileInGrid.gameObject.GetComponent<BoxCollider2D>());
                }
            }
        }

        return lockTileColliders;
    }

    private void SnapTilesToGrid()
    {
        bool isSnappedToNewPlace = false;

        if (!allElementsNull)
        {
            FindObjectOfType<AudioManager>().Play("liik");
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

            for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
            {
                for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                {
                    Transform tile = currentMovableTiles[col, row];

                    if (tile != null)
                    {                      
                        MovableTile movableTileComponent = tile.GetComponent<MovableTile>();

                        // Skip the locked tiles
                        if (movableTileComponent.IsLocked)
                        {
                            Debug.Log($"Locked Tile Final Position at [{movableTileComponent.Column}, {movableTileComponent.Row}]: {tile.position}");
                            continue;
                        }

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

                        if (tile.position == initialTilePositions[col, row])
                        {
                            isSnappedToNewPlace = false;
                        }
                        else
                        {
                            isSnappedToNewPlace = true;
                        }
                        Debug.Log("issnappedtonewplace " + isSnappedToNewPlace);
                    }
                }
            }
            
            movableTileGrid.DestroyKeyAndLockTilesIfNeighbor();

            if (isSnappedToNewPlace)
            {
                //Update movableTiles array with new snapped positions
                movableTiles = movableTileGrid.UpdateMovableTilesArray();

                // Toggle between "horizontal" and "vertical" move types.
                currentMoveType = (currentMoveType == "horizontal") ? "vertical" : "horizontal";
                Debug.Log("movetype change: " + currentMoveType);
                movableTileGrid.IsMovableTilesGroupConnected();
            }
            else if (!isSnappedToNewPlace)
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
                            Animator animator = movableTile.gameObject.GetComponent<Animator>();
                            animator.SetBool("isGlowing", false);
                        }
                        currentMovableTiles[i, j] = null;
                    }
                }
            }
            allElementsNull = true;
            tileInSamePosition = false;
        }

    }
}