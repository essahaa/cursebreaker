using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;

public class TutorialLevel : MonoBehaviour
{
    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;
    public GameObject arrowPrefab;
    public GameObject lynaraPrefab;
    public GameObject dialogBubblePrefab;
    public Sprite overlay_0;
    public Sprite overlay_1;
    public Sprite overlay_2;

    public Canvas shadowCanvas;

    public GameObject arrow;
    private GameObject lynara;
    private GameObject dialogBubble;
    public Image overlay;

    private string dialogue1 = "Hi there! I'm here to guide you.";
    private string dialogue2 = "Your objective is to move the yellow tiles row by row so that the red tile isn't connected to the yellow ones.";
    private string dialogue3 = "In this game, you have to move the puzzles so that every other move is horizontal and every other vertical.";
    private string dialogue4 = "To move a row, just tap and hold it, then slide it to right.";
    private string dialogue5 = "Great job! Now, you need to know that there must be atleast 2 tiles in the row for you to be able to move it.";
    private string dialogue6 = "Let’s do another one! Tap and hold the column in the middle and slide it downwards.";
    private string dialogue7 = "Fantastic! You’ve completed the level.";
    private string dialogue8 = "Now that you've mastered the basics, it's time to tackle more challenging puzzles!";

    public TextAsset csvFile;
    public BackgroundGrid backgroundGrid;
    
    public Animator animator;

    private int selectedLevel = 0; // The level you want to generate.
    
    private int rowIndex;
    private int columnIndex;

    public bool firstMovementDone = false;
    public bool tutorialDone = false;
    
    private bool backgroundGenerated = false;

    public int gridSizeX = 10; //number of columns, width of the grid
    public int gridSizeY = 10; //number of rows, height of the grid

    private Transform[,] currentMovableTiles;
    private Transform[,] movableTiles;
    public Vector3[,] initialTilePositions;

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        backgroundGenerated = false;

        GameObject overlayObject = GameObject.Find("Overlay");
        overlay = overlayObject.GetComponent<Image>();

        ReadLevelDataFromCSV();
        GeneratePrefabs();
        LogTutorialStartEvent();
    }

    private void LogTutorialStartEvent()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_started");
   
    }

    public void ReadLevelDataFromCSV()
    {
        using (var reader = new StreamReader(new MemoryStream(csvFile.bytes)))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

                // Ensure there's enough data in the line to avoid IndexOutOfRangeException
                if (values.Length < 6) continue;

                if (int.TryParse(values[0], out int level) && level == selectedLevel)
                {
                    // Use TryParse for safety.
                    if (int.TryParse(values[1], out int column) &&
                        int.TryParse(values[2], out int row) &&
                        int.TryParse(values[4], out gridSizeX) &&
                        int.TryParse(values[5], out gridSizeY))
                    {
                        string tileType = values[3];

                        // Initialize the array on first successful parse.
                        if (movableTiles == null)
                        {
                            movableTiles = new Transform[gridSizeX, gridSizeY];
                        }

                        GenerateTileFromCSV(column, row, tileType, gridSizeX, gridSizeY);
                        
                    }
                    else
                    {
                        Debug.Log("Error when parsing values");
                    }
                }
            }
        }
    }

    void GeneratePrefabs()
    {      
            Vector3 arrowposition = new Vector3(0.1f, 0.7f, 0);
            arrow = Instantiate(arrowPrefab, arrowposition, Quaternion.identity);
            arrow.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
            arrow.SetActive(false);

            Vector3 lynaraPosition = new Vector3(-0.6f, -2.4f, 0);
            lynara = Instantiate(lynaraPrefab, lynaraPosition, Quaternion.identity);
            lynara.transform.localScale = new Vector3(0.9f, 0.9f, 1);

            Vector3 dialogPosition = new Vector3(0.4f, -4f, 0);
            dialogBubble = Instantiate(dialogBubblePrefab, dialogPosition, Quaternion.identity);
            dialogBubble.transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    public void GenerateTileFromCSV(int column, int row, string tileType, int gridSizeX, int gridSizeY)
    {
        if (!backgroundGenerated)
        {
            backgroundGrid.GenerateBackgroundGrid(gridSizeX, gridSizeY);
            backgroundGenerated = true;
        }

            if (column < gridSizeX && row < gridSizeY)
        {
            // Cache the reference to the background grid cell
            Transform backgroundGridCell = backgroundGrid.backgroundGrid[column, row];

            if (backgroundGridCell != null)
            {
                Vector3 position = backgroundGridCell.position;
                GameObject tilePrefab = GetTile(tileType);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                //tile.transform.position = position;
                tile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);

                MovableTile movableTileComponent = tile.GetComponent<MovableTile>();
                if (movableTileComponent != null)
                {
                    movableTileComponent.Level = selectedLevel;
                    movableTileComponent.Row = row;
                    movableTileComponent.Column = column;
                    movableTileComponent.TileType = tileType;
                    movableTileComponent.GridSizeX = gridSizeX;
                    movableTileComponent.GridSizeY = gridSizeY;

                    movableTiles[column, row] = movableTileComponent.transform;
                }
            }
        }
    }

    public GameObject GetTile(string tileType)
    {
        switch (tileType)
        {
            case "Normal":
                return movableTilePrefab;
            case "Evil":
                return evilTilePrefab;
            default:
                return null;
        }
    }

    public void UpdateMovableTile(int column, int row, Transform newTile)
    {
        //empty cell 
        movableTiles[column, row] = null;
        // Update the cell with the new tile.
        movableTiles[column, row] = newTile;
        MovableTile tile = movableTiles[column, row].GetComponent<MovableTile>();

        if (tile != null)
        {
            tile.Row = row;
            tile.Column = column;
        }
    }

    public Transform[,] UpdateMovableTilesArray()
    {
        for (int row = 0; row < gridSizeY; row++)
        {
            for (int col = 0; col < gridSizeX; col++)
            {
                // Get the tile at the current position.
                Transform tile = movableTiles[col, row];

                if (tile != null && (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile")))
                {
                    // Update the movableTiles array.
                    movableTiles[col, row] = tile.transform;
                }
                else
                {
                    movableTiles[col, row] = null;
                }
            }
        }

        return movableTiles;
    }

    public Transform[,] FindAdjacentMovableTilesInRow(int rowIndex)
    {
        int numCols = movableTiles.GetLength(0); // Number of columns.
        int numRows = movableTiles.GetLength(1); // Number of rows.
        Transform[,] tilesInRow = new Transform[numCols, numRows];

        if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(1))
        {
            for (int col = 0; col < numCols; col++)
            {
                // Only consider this tile if it's not null and has at least one adjacent tile.
                if (movableTiles[col, rowIndex] != null &&
                    ((col > 0 && movableTiles[col - 1, rowIndex] != null) ||
                     (col < numCols - 1 && movableTiles[col + 1, rowIndex] != null)))
                {
                    tilesInRow[col, rowIndex] = movableTiles[col, rowIndex];
                }
            }
        }

        return tilesInRow;
    }

    public Transform[,] FindAdjacentMovableTilesInColumn(int columnIndex)
    {
        int numCols = movableTiles.GetLength(0); // Number of columns.
        int numRows = movableTiles.GetLength(1); // Number of rows.
        Transform[,] tilesInColumn = new Transform[numCols, numRows]; 

        if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(0))
        {
            for (int row = 0; row < numRows; row++)
            {
                if (movableTiles[columnIndex, row] != null &&
                    ((row > 0 && movableTiles[columnIndex, row - 1] != null) ||
                     (row < numRows - 1 && movableTiles[columnIndex, row + 1] != null)))
                {
                    tilesInColumn[columnIndex, row] = movableTiles[columnIndex, row];
                }
            }
        }

        return tilesInColumn;
    }

    public Transform[,] TutorialLevelFindCurrentMovables(int columnIndex, int rowIndex)
    {
        if (!firstMovementDone)
        {
            currentMovableTiles = FindAdjacentMovableTilesInRow(rowIndex);
        }
        else if (firstMovementDone)
        {
            currentMovableTiles = FindAdjacentMovableTilesInColumn(columnIndex);
        }

        return currentMovableTiles;
    }

    public Vector3[,] TutorialLevelGetInitialPositions(Transform[,] currentMovableTiles)
    {
        // Initialize the array for initial positions
        initialTilePositions = new Vector3[gridSizeY, gridSizeX];

        // Store the initial positions of movable tiles in the row or column.
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

        return initialTilePositions;
    }

    public void EmptyMovableTilesArrayRowOrColumn(Transform[,] currentMovableTiles)
    {
        //empty row or column that has been moved
        for (int i = 0; i < currentMovableTiles.GetLength(0); i++)
        {
            for (int j = 0; j < currentMovableTiles.GetLength(1); j++)
            {
                Transform cTile = currentMovableTiles[i, j];
                if (cTile != null && (cTile.CompareTag("MovableTile") || cTile.CompareTag("EvilTile")))
                {
                    movableTiles[i, j] = null;
                }
            }
        }
    }

    public void ShowNextSpeechBubble(int number)
    {
        TextMeshPro tmp = dialogBubble.GetComponentInChildren<TextMeshPro>();
        number = number + 1;

        switch (number)
        {
            case 1:
                tmp.text = dialogue1;
                break;
            case 2:
                tmp.text = dialogue2;
                break;
            case 3:
                tmp.text = dialogue3;
                break;
            case 4:
                tmp.text = dialogue4;
                break;
            case 5:
                tmp.text = dialogue5;
                break;
            case 6:
                tmp.text = dialogue6;
                break;
            case 7:
                tmp.text = dialogue7;
                break;
            case 8:
                tmp.text = dialogue8;
                break;
            default:
                tmp.text = dialogue1;
                break;
        }
    }

    public void TutorialCompleted()
    {
        if(firstMovementDone)
        {
            arrow.SetActive(false);
            MovableTile tileToDestroy = movableTiles[6, 5].GetComponent<MovableTile>();
            Destroy(tileToDestroy.gameObject);
            Debug.Log("tutorial completed");
            tutorialDone = true;

            ShowNextSpeechBubble(6);
        }       
    }

    public void EndLevel()
    {
        GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
        if (levelCompletedBox != null)
        {
            animator = levelCompletedBox.GetComponent<Animator>();
        }
        animator.SetTrigger("LevelEnd");

        LogTutorialCompletionEvent();
    }

    private void LogTutorialCompletionEvent()
    {
        FirebaseAnalytics.LogEvent("tutorial_completed");

    }

    public void ChangeMovementDone()
    {
        firstMovementDone = true;
        arrow.SetActive(false);
        arrow.transform.position = new Vector3(1.2f, 0, 0);
        arrow.transform.rotation = Quaternion.Euler(0, 0, 270);
        
        UpdateMovableTilesArray();
    }
}

