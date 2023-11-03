using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TutorialLevel : MonoBehaviour
{
    public string currentMoveType = "horizontal";
    private int rowIndex;
    private int columnIndex;

    public bool firstMovementDone = false;
    public bool lastMovementDone = false;

    private Transform[,] currentMovableTiles;
    public Vector3[,] initialTilePositions;

    private void Start()
    {
        firstMovementDone = false;
    }

    public Transform[,] TutorialLevelFindCurrentMovables()
    {
        MovableTileGrid movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();

        if (!firstMovementDone)
        {
            currentMoveType = "horizontal";
            rowIndex = 5;

            currentMovableTiles = movableTileGrid.FindAdjacentMovableTilesInRow(rowIndex);
        }
        else if (firstMovementDone)
        {
            currentMoveType = "vertical";
            columnIndex = 5;

            currentMovableTiles = movableTileGrid.FindAdjacentMovableTilesInColumn(columnIndex);
        }

        return currentMovableTiles;
    }

    public Vector3[,] TutorialLevelGetInitialPositions(Transform[,] currentMovableTiles)
    {
        initialTilePositions = new Vector3[currentMovableTiles.GetLength(0), currentMovableTiles.GetLength(1)];

        for (int row = 0; row < currentMovableTiles.GetLength(0); row++)
        {
            for (int col = 0; col < currentMovableTiles.GetLength(1); col++)
            {
                Transform tile = currentMovableTiles[col, row];

                if (tile != null)
                {
                    initialTilePositions[col, row] = currentMovableTiles[col, row].position;
                    Debug.Log("initialtileposition " + currentMovableTiles[col, row].position);

                }
            }
        }
        return initialTilePositions;
    }

    public void TutorialCompleted()
    {
        MovableTileGrid movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();
        Destroy(movableTileGrid.movableTiles[6, 5]);
        Debug.Log("tutorial completed");
    }

    public void ChangeMovementDone()
    {
        firstMovementDone = true;
        currentMoveType = "vertical";
        MovableTileGrid movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();
        movableTileGrid.UpdateMovableTilesArray();
    }

    public void EmptyCurrentMovableArray()
    {
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

