using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelMovement : MonoBehaviour
{
    public BackgroundGrid backgroundGrid;
    public TutorialLevel tutorialLevel;

    private Transform[] currentMovableTiles;
    public Vector3[,] initialTilePositions;

    private bool isMoved = false;
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

                isMoved = true;
            }
        }
    }

    private void OnMouseUp()
    {
        if (isMoved)
        {
            Debug.Log("mouse up");

            foreach (Transform tile in currentMovableTiles)
            {
                if (tile != null)
                {
                    MovableTile movableTileComponent = tile.GetComponent<MovableTile>();
                    if (movableTileComponent == null)
                    {
                        Debug.LogError("MovableTile component not found on tile.");
                        continue;
                    }

                    int row = movableTileComponent.Row;
                    int col = movableTileComponent.Column;

                    Vector3 targetPosition;
                    if (currentMoveType == "horizontal")
                    {
                        targetPosition = backgroundGrid.backgroundGrid[col + 1, row].position;
                        Debug.Log("horizontal col, row " + (col + 1) + " , " + row);
                        tutorialLevel.UpdateMovableTile(col + 1, row, tile);
                    }
                    else // vertical movement
                    {
                        targetPosition = backgroundGrid.backgroundGrid[col, row - 1].position;
                        Debug.Log("vertical col, row " + col + " , " + (row - 1));
                        tutorialLevel.UpdateMovableTile(col, row - 1, tile);
                    }

                    tile.position = targetPosition;
                }
            }

            // Logic after moving tiles
            if (currentMoveType == "horizontal" && !tutorialLevel.firstMovementDone)
            {
                // First movement done
                // Additional logic for first movement
            }
            else
            {
                tutorialLevel.TutorialCompleted();
            }

            tutorialLevel.ChangeMovementDone();
            currentMoveType = "vertical";

            //empty current movable tiles array
            for (int i = 0; i < currentMovableTiles.Length; i++)
            {
                currentMovableTiles[i] = null;
            }
        }

        isMoved = false;
    }

}
