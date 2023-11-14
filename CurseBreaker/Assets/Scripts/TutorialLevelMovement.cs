using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialLevelMovement : MonoBehaviour
{
    public BackgroundGrid backgroundGrid;
    public TutorialLevel tutorialLevel;

    private Transform[] currentMovableTiles;
    public Vector3[,] initialTilePositions;

    private bool isMoved = false;
    private bool secondMoveCanBeDone = false;
    private bool levelEnd = false;
    private string currentMoveType = "horizontal";

    int clickCount = 0; // Counter for clicks
    int speechBubbleThreshold = 3; // Number of clicks to show speech bubbles before starting tile movement

    private GameObject selectedTile;

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        tutorialLevel = GameObject.FindGameObjectWithTag("TutorialLevel").GetComponent<TutorialLevel>();

    }

    private void Update()
    {
        // Check for mouse click or touch input
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            clickCount++; // Increment the click counter
            Debug.Log("clicks " + clickCount);

            if (clickCount <= speechBubbleThreshold)
            {
                // Show next speech bubble
                tutorialLevel.ShowNextSpeechBubble(clickCount);
            }
            else if (tutorialLevel.tutorialDone)
            {
                tutorialLevel.ShowNextSpeechBubble(7);

                if(levelEnd)
                {
                    tutorialLevel.EndLevel();
                }
                levelEnd = true;

            }
            else
            {
                if (clickCount > speechBubbleThreshold && secondMoveCanBeDone)
                {
                    tutorialLevel.ShowNextSpeechBubble(5);
                    secondMoveCanBeDone = false;

                }
                // Raycast to detect which tile was clicked or tapped.
                Vector3 inputPos = Input.mousePosition;
                if (Input.touchCount > 0)
                {
                    inputPos = Input.GetTouch(0).position;
                }

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(inputPos), Vector2.zero);

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
                tutorialLevel.ShowNextSpeechBubble(4);
                tutorialLevel.ChangeMovementDone();
                secondMoveCanBeDone = true;
            }
            else if(currentMoveType == "vertical" && !tutorialLevel.tutorialDone)
            {
                tutorialLevel.TutorialCompleted();
            }
            
            

            
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
