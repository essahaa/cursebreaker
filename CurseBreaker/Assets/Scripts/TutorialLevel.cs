using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevel : MonoBehaviour
{
    //eka siirto on yks pyk‰l‰ oikealle, toka siirto yks pyk‰l‰ alas
    //n‰ill‰ on tietyt rowit ja columnit
    //onmouseclick pit‰s ottaa vaan sen tietyn rowin tilet jos kyseess‰ on tutorial

    //pit‰‰ tarkistaa snappaako siirretty rowi oikeaan kohtaan, jos snappaa niin sitten siirryt‰‰n toiseen siirtoon

    //pit‰‰ ehk‰ asettaa jotkut rajat ett‰ rowia ei saa edes siirretty‰ pitemm‰lle kuin yhden pyk‰l‰n verran
    private string currentMoveType = "horizontal";
    private int rowIndex;
    private int columnIndex;

    public bool firstMovementDone = false;
    public bool lastMovementDone = false;

    private Transform[,] currentMovableTiles;
    private Vector3[,] initialTilePositions;

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

    public Vector3[,] TutorialLevelGetInitialPositions()
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

                }
            }
        }

        return initialTilePositions;
    }

    public bool ChangeMovementDone()
    {
        firstMovementDone = firstMovementDone = false ? true : false;
        return firstMovementDone;
    }
    
}

