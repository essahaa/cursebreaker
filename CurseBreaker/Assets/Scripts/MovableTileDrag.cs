using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTileDrag : MonoBehaviour
{
    // Reference to the BackgroundGrid script.
    public BackgroundGrid backgroundGrid;
    public MovableTileGrid movableTileGrid;
    //public RowColumnManager rowColumnManager;

    private Vector3 initialMousePosition;
    private Vector3 initialTilePosition;

    public string currentMoveType = "horizontal"; // Initialize with a horizontal move

    // Set the drag threshold for detecting a swipe (you can adjust this value).
    public float swipeThreshold = 50f;
    public float stepSize = 1.0f;
    public float tileSpacing = 0.4f;

    private bool isDragging = false;
    private bool isRowMoving = false;
    private bool isColumnMoving = false;
    private bool hasAdjacent = false;

    Transform[,] currentMovableTiles;
    private Vector3[,] initialTilePositions;
    private Transform[,] movableTiles; // Declare it at the class level.

    private GameObject selectedTile;
    private int selectedRow;
    private int rowIndex;
    private int columnIndex;
    private float initialX;
    private float initialY;

    private void Start()
    {
        // Find the GameObject with the name "BackgroundGridManager" and get its BackgroundGrid component.
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();
        //rowColumnManager = GameObject.Find("RowColumnManager").GetComponent<RowColumnManager>();

        movableTiles = movableTileGrid.movableTiles;

    }

    private void OnMouseDown()
    {

        // Raycast to detect which tile was clicked.
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Vector3 mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //movableTiles = movableTileGrid.GetMovableTiles();
        //initialMousePosition = mouseCurrentPos;

        if (hit.collider != null)
        {

            Debug.Log("Hit object name: " + hit.collider.gameObject.name);
            // Check if the clicked object is a movable tile.
            if (hit.collider.gameObject.CompareTag("MovableTile"))
            {
                selectedTile = hit.collider.gameObject;

                // Calculate the row and column of the selected tile based on its grid position.
                rowIndex = selectedTile.GetComponent<MovableTile>().Row;

                columnIndex = selectedTile.GetComponent<MovableTile>().Column;

                hasAdjacent = (currentMoveType == "horizontal") ? movableTileGrid.HasAdjacentMovableTilesInRow(columnIndex, rowIndex) : movableTileGrid.HasAdjacentMovableTilesInColumn(columnIndex, rowIndex);
                Debug.Log("On naapuri" + hasAdjacent);

                // Store initial X and Y positions for reference.
                initialX = selectedTile.transform.position.x;
                initialY = selectedTile.transform.position.y;

                Debug.Log("rowindex " + selectedTile.GetComponent<MovableTile>().Row + " colindex " + selectedTile.GetComponent<MovableTile>().Column + " currentmovetype: " + currentMoveType);

                // Find all the movable tiles in the specified row or column.
                if (currentMoveType == "horizontal")
                {
                    currentMovableTiles = movableTileGrid.FindMovableTilesInRow(rowIndex);



                    if (currentMovableTiles != null)
                    {
                        // Assuming 'array' is your 2D array of type Transform[,]
                        for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
                        {
                            for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
                            {
                                Transform element = currentMovableTiles[i, j];

                                if (element != null)
                                {
                                    Debug.Log($"currentmovables ({i}, {j}): {element.name}");
                                }

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("currentmovabletiles null");
                    }


                }
                else if (currentMoveType == "vertical")
                {
                    currentMovableTiles = movableTileGrid.FindMovableTilesInColumn(columnIndex);


                }


                if (currentMovableTiles != null)
                {
                    // Initialize the 2D array based on the size of the movableTiles array.
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

                    // Set the movableTiles array to the currentMovableTiles.
                    //movableTiles = currentMovableTiles;
                }
                else
                {
                    Debug.Log("currentmovabletiles null");
                }





                // Mark dragging as initiated.
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
        if (isDragging)
        {
            // Calculate the offset based on the initial mouse and tile positions.
            Vector3 mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = mouseCurrentPos - initialMousePosition;


            if (currentMoveType == "horizontal")
            {
                isRowMoving = true;

                // Horizontal movement (row)
                MoveRow(rowIndex, offset.x, offset.y);


                // Calculate the target position for each tile in the row.
                for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
                {
                    for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                    {
                        Transform tile = currentMovableTiles[col, row];

                        // Check if the tile is not null (it may be empty in some positions).
                        if (tile != null)
                        {
                            // Calculate the target position for the tile based on row movement.
                            Vector3 targetPosition = initialTilePositions[col, row] + new Vector3(offset.x, 0f, 0f);

                            // Move the tile to the target position.
                            tile.position = targetPosition;
                        }
                    }
                }



            }
            else
            {
                isColumnMoving = true;

                // Vertical movement (column)
                MoveColumn(columnIndex, offset.x, offset.y);


                // Calculate the target position for each tile in the column.
                for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
                {
                    for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                    {
                        Transform tile = currentMovableTiles[col, row];

                        // Check if the tile is not null (it may be empty in some positions).
                        if (tile != null)
                        {
                            // Calculate the target position for the tile based on column movement.
                            Vector3 targetPosition = initialTilePositions[col, row] + new Vector3(0f, offset.y, 0f);

                            // Move the tile to the target position.
                            tile.position = targetPosition;
                        }
                    }
                }


            }
        }
    }


    private void OnMouseUp()
    {
        isDragging = false;
        isRowMoving = false;
        isColumnMoving = false;

        // Toggle between "horizontal" and "vertical" move types.
        currentMoveType = (currentMoveType == "horizontal") ? "vertical" : "horizontal";


        // TODO: Implement snapping logic.
        if (currentMovableTiles.Length > 0)
        {
            movableTiles = currentMovableTiles;
            movableTileGrid.EmptyMovableTilesArrayRowOrColumn(movableTiles);
            // Snap each tile to the nearest grid position.
            foreach (Transform tile in currentMovableTiles)
            {
                if (tile != null) // Check if the tile is not null.
                {
                    Vector3 currentPosition = tile.position;

                    // Calculate the nearest grid position by rounding the current position.
                    Vector3 snappedPosition = new Vector3(
                        Mathf.Round(currentPosition.x / backgroundGrid.backgroundTileSize) * backgroundGrid.backgroundTileSize,
                        Mathf.Round(currentPosition.y / backgroundGrid.backgroundTileSize) * backgroundGrid.backgroundTileSize,
                        0f
                    );

                    // Ensure the snapped position stays within the background grid boundaries.
                    snappedPosition.x = Mathf.Clamp(snappedPosition.x, backgroundGrid.minX, backgroundGrid.maxX);
                    snappedPosition.y = Mathf.Clamp(snappedPosition.y, backgroundGrid.minY, backgroundGrid.maxY);

                    // Calculate the row and column indices
                    int column = Mathf.RoundToInt((snappedPosition.x - backgroundGrid.minX) / backgroundGrid.backgroundTileSize);
                    int row = Mathf.RoundToInt((snappedPosition.y - backgroundGrid.minY) / backgroundGrid.backgroundTileSize);

                    // Set the tile's position to the snapped position.
                    tile.position = snappedPosition;

                    // Ensure that row and column are within valid bounds
                    if (column >= 0 && column < backgroundGrid.gridSizeX && row >= 0 && row < backgroundGrid.gridSizeY)
                    {
                        // Get the MovableTile component of the tile.
                        MovableTile movableTileComponent = tile.GetComponent<MovableTile>();
                        movableTileGrid.UpdateMovableTile(column, row, movableTileComponent.transform);

                        // Update the Row and Column properties of the MovableTile component.
                        //movableTileComponent.Row = row;
                        //movableTileComponent.Column = column;

                        // Now 'row' and 'column' contain the indices of the snapped tile.
                        Debug.Log("Snapped to row: " + row + ", column: " + column + " tile position " + tile.position);
                        Debug.Log("movabletilecomponent.Row " + movableTileComponent.Row + " movabletilecomponent.Column " + movableTileComponent.Column);
                    }





                }
            }
            // Call the method to update row and column data after tile movement.
            //rowColumnManager.UpdateRowColumnData(movableTiles);

            movableTiles = movableTileGrid.UpdateMovableTilesArray();
            //movableTiles = movableTileGrid.GetMovableTiles();


            // Assuming 'array' is your 2D array of type Transform[,]
            for (int i = 0; i < movableTiles.GetLength(0); i++)
            {
                for (int j = 0; j < movableTiles.GetLength(1); j++)
                {
                    Transform element = movableTiles[i, j];

                    if (element != null)
                    {
                        Debug.Log($"On mouse up ({i}, {j}): {element.name}");
                    }
                    else
                    {
                        Debug.Log($"On mouse up ({i}, {j}): null");
                    }
                }
            }
        }

        //empty current movable tiles array
        for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
        {
            for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
            {
                currentMovableTiles[i, j] = null;
            }
        }

    }

    private void MoveRow(int rowIndex, float xOffset, float yOffset)
    {
        if (isRowMoving && xOffset != 0f && Mathf.Abs(xOffset) > Mathf.Abs(yOffset))
        {
            float maxMoveDistance = backgroundGrid.backgroundTileSize; // Set your desired maximum move distance here.

            if (Mathf.Abs(xOffset) <= maxMoveDistance)
            {
                if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(0) && movableTiles.GetLength(0) > 0 && movableTiles.GetLength(1) > 0)
                {
                    // Calculate the target X position for each tile in the row incrementally.
                    for (int column = 0; column < movableTiles.GetLength(1); column++)
                    {
                        Transform tile = movableTiles[column, rowIndex];

                        // Check if the tile is not null (it may be empty in some positions).
                        if (tile != null)
                        {
                            // Get the TileCollisionManager component of the tile.
                            TileCollisionManager tileCollisionManager = tile.GetComponent<TileCollisionManager>();

                            // Check if the tile is at the boundary.
                            if (!tileCollisionManager.IsAtBoundary())
                            {
                                Vector3 targetPosition = initialTilePositions[column, rowIndex] + new Vector3(xOffset, 0f, 0f);

                                // Ensure the tile stays within the background grid boundaries.
                                targetPosition.x = Mathf.Clamp(targetPosition.x, backgroundGrid.minX, backgroundGrid.maxX);

                                // Move the tile to the target position.
                                tile.position = targetPosition;
                            }
                        }
                    }

                    // Mark the row as moving.
                    isRowMoving = true;

                }
            }
        }
    }

    private void MoveColumn(int columnIndex, float xOffset, float yOffset)
    {
        if (!isRowMoving || (isColumnMoving && yOffset != 0f && Mathf.Abs(yOffset) > Mathf.Abs(xOffset)))
        {
            float maxMoveDistance = backgroundGrid.backgroundTileSize; // Set your desired maximum move distance here.

            if (Mathf.Abs(yOffset) <= maxMoveDistance)
            {
                if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(1) && movableTiles.GetLength(0) > 0 && movableTiles.GetLength(1) > 0)
                {
                    for (int row = 0; row < movableTiles.GetLength(0); row++)
                    {
                        Transform tile = movableTiles[columnIndex, row];

                        if (tile != null) // Check if the tile is not null (it may be empty in some positions).
                        {
                            // Get the TileCollisionManager component of the tile.
                            TileCollisionManager tileCollisionManager = tile.GetComponent<TileCollisionManager>();

                            // Check if the tile is at the boundary.
                            if (!tileCollisionManager.IsAtBoundary())
                            {
                                Vector3 targetPosition = initialTilePositions[columnIndex, row] + new Vector3(0f, yOffset, 0f);

                                // Ensure the tile stays within the background grid boundaries.
                                targetPosition.y = Mathf.Clamp(targetPosition.y, backgroundGrid.minY, backgroundGrid.maxY);

                                // Move the tile to the target position.
                                tile.position = targetPosition;

                            }
                        }
                    }
                }

                // Mark the column as moving.
                isColumnMoving = true;

            }
        }
    }
}
