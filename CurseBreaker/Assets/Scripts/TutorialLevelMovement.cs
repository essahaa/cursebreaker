using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelMovement : MonoBehaviour
{
    public BackgroundGrid backgroundGrid;
    public TutorialLevel tutorialLevel;

    private Transform[,] currentMovableTiles;
    public Vector3[,] initialTilePositions;

    private bool isDragging = false;
    private string currentMoveType = "horizontal";

    private GameObject selectedTile;

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        tutorialLevel = GameObject.FindGameObjectWithTag("TutorialLevel").GetComponent<TutorialLevel>();

    }
    private void OnMouseDown()
    {
        // Raycast to detect which tile was clicked.
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("hit detected");
            if (hit.collider.gameObject.CompareTag("MovableTile") || hit.collider.gameObject.CompareTag("EvilTile"))
            {
                selectedTile = hit.collider.gameObject;

                currentMovableTiles = tutorialLevel.TutorialLevelFindCurrentMovables();
                initialTilePositions = tutorialLevel.TutorialLevelGetInitialPositions(currentMovableTiles);

                isDragging = true;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Debug.Log("dragging");
            for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
            {
                for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
                {
                    Transform tile = currentMovableTiles[col, row];

                    if (tile != null)
                    {
                        Vector3 targetPosition;
                        if (currentMoveType == "horizontal" && !tutorialLevel.firstMovementDone)
                        {
                            targetPosition = backgroundGrid.backgroundGrid[col + 1, row].position;

                            tile.position = targetPosition;
                            Debug.Log("horizontal col, row " + (col + 1) + " , " + row);
                            tutorialLevel.UpdateMovableTile(col + 1, row, tile);
                        }
                        else
                        {
                            targetPosition = backgroundGrid.backgroundGrid[col, row - 1].position;

                            tile.position = targetPosition;
                            Debug.Log("vertical col, row " + col + " , " + (row - 1));
                            tutorialLevel.UpdateMovableTile(col, row - 1, tile);
                        }
                    }
                }
            }
        }
    }

    private void OnMouseUp()
    {
        // Snap each tile to the nearest grid position based on rows and columns.
        foreach (Transform tile in currentMovableTiles)
        {
            if (tile != null)
            {
                MovableTile movableTileComponent = tile.GetComponent<MovableTile>();
                int col = movableTileComponent.Column;
                int row = movableTileComponent.Row;

                tile.position = backgroundGrid.backgroundGrid[col, row].position;
                tutorialLevel.UpdateMovableTile(col, row, tile);

                if (currentMoveType == "horizontal" && !tutorialLevel.firstMovementDone)
                {
                    //first movement done
                }
                else
                {
                    tutorialLevel.TutorialCompleted();
                }
            }
        }
        tutorialLevel.ChangeMovementDone();
        currentMoveType = "vertical";
        tutorialLevel.EmptyCurrentMovableArray();

        //empty current movable tiles array
        for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
        {
            for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
            {
                currentMovableTiles[i, j] = null;
            }
        }
    }
}
