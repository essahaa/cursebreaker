using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevel : MonoBehaviour
{
    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;

    public TextAsset csvFile;
    public BackgroundGrid backgroundGrid;
    
    public Animator animator;

    private int selectedLevel = 1; // The level you want to generate.
    
    private int rowIndex;
    private int columnIndex;

    private string currentMoveType = "horizontal";

    public bool firstMovementDone = false;
    public bool tutorialDone = false;
    
    private bool backgroundGenerated = false;

    public int gridSizeX; //number of columns, width of the grid
    public int gridSizeY; //number of rows, height of the grid

    private Transform[,] currentMovableTiles;
    private Transform[,] movableTiles;
    public Vector3[,] initialTilePositions;

    private List<string> csvLines = new List<string>();

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();

        ReadLevelDataFromCSV();
        
        
    }

    

    public void ReadLevelDataFromCSV()
    {
        Debug.Log("reading csv");
        string[] lines = csvFile.text.Split('\n');
        csvLines.AddRange(lines);
        bool arraySizeSet = false; // Add a flag to track if array size is set.

        if (csvLines.Count > 0)
        {
            foreach (string line in csvLines)
            {
                string[] values = line.Split(';'); // Split each line into values.

                // Check if the current line corresponds to the target level.
                if (values.Length >= 1 && int.TryParse(values[0], out int level) && level == 0)
                {
                    Debug.Log("level " + values[0] + " selectedlevel " + 0);
                    // Parse data from the CSV line.
                    int column = int.Parse(values[1]);
                    int row = int.Parse(values[2]);
                    string tileType = values[3];
                    gridSizeX = int.Parse(values[4]);
                    gridSizeY = int.Parse(values[5]);

                    // Set the array size only once.
                    if (!arraySizeSet)
                    {
                        movableTiles = new Transform[gridSizeX, gridSizeY];
                        arraySizeSet = true; // Update the flag.
                    }

                    GenerateTileFromCSV(column, row, tileType, gridSizeX, gridSizeY);
                }
            }

        }
        

    }

    GameObject GetTilePrefab(string tileType)
    {
        // Choose the appropriate prefab based on the tileType.
        switch (tileType)
        {
            case "Normal":
                return movableTilePrefab;
            case "Evil":
                return evilTilePrefab;
            // Add more cases for other tile types as needed.
            default:
                return movableTilePrefab; // Default to a fallback prefab.
        }
    }

    public void GenerateTileFromCSV(int column, int row, string tileType, int gridSizeX, int gridSizeY)
    {
        if (!backgroundGenerated)
        {
            backgroundGrid.GenerateBackgroundGrid(gridSizeX, gridSizeY);
            backgroundGenerated = true;
        }

        GameObject tilePrefab = GetTilePrefab(tileType);

        if (column < gridSizeX && row < gridSizeY)
        {
            // Check if the position exists in the backgroundGrid array
            if (backgroundGrid.backgroundGrid[column, row] != null)
            {
                // Get the position from the backgroundGrid array
                Vector3 position = backgroundGrid.backgroundGrid[column, row].position;

                // Create the tile at the retrieved position
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
                movableTiles[column, row] = tile.transform;

                MovableTile tileData = tile.GetComponent<MovableTile>();
                tileData.Level = selectedLevel;
                tileData.Row = row;
                tileData.Column = column;
                tileData.TileType = tileType;
                tileData.GridSizeX = gridSizeX;
                tileData.GridSizeY = gridSizeY;
            }
        }
    }
    public void UpdateMovableTile(int column, int row, Transform newTile)
    {
        //empty cell 
        movableTiles[column, row] = null;
        // Update the cell with the new tile.
        movableTiles[column, row] = newTile;

        Transform tile = movableTiles[column, row];

        if (tile != null)
        {
            // Get the MovableTile component of the tile.
            MovableTile movableTileComponent = tile.GetComponent<MovableTile>();
            movableTileComponent.Row = row;
            movableTileComponent.Column = column;
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

    public void DestroyExistingMovableTiles()
    {
        // Clear the references in the movableTiles array.
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                movableTiles[x, y] = null;
            }
        }

        // Find and destroy game objects with the "MovableTile" and "EvilTile" tags.
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("MovableTile");
        objectsToDestroy = objectsToDestroy.Concat(GameObject.FindGameObjectsWithTag("EvilTile")).ToArray();
        objectsToDestroy = objectsToDestroy.Concat(GameObject.FindGameObjectsWithTag("BackgroundTile")).ToArray();

        // Loop through and destroy each GameObject
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }

        backgroundGenerated = false;
        // Generate new movable tiles (and evil tiles if needed).
        ReadLevelDataFromCSV();
    }

    public Transform[,] FindAdjacentMovableTilesInRow(int rowIndex)
    {
        Debug.Log("rowindex in findmovable " + rowIndex);

        int numRows = movableTiles.GetLength(1);
        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        Transform[,] tilesInRow = new Transform[numCols, numRows];

        bool foundAdjacentTile = false; // Flag to track if an adjacent tile has been found.

        if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(1))
        {
            for (int col = 0; col < numCols; col++)
            {
                if (movableTiles[col, rowIndex] != null)
                {
                    // Check if the tile has an adjacent tile to the left or right.
                    bool hasLeftAdjacent = (col > 0 && movableTiles[col - 1, rowIndex] != null);
                    bool hasRightAdjacent = (col < numCols - 1 && movableTiles[col + 1, rowIndex] != null);

                    if (hasLeftAdjacent || hasRightAdjacent)
                    {
                        // Include this tile in the movable row.
                        tilesInRow[col, rowIndex] = movableTiles[col, rowIndex];
                        foundAdjacentTile = true; // Set the flag to true.

                    }
                    else if (foundAdjacentTile)
                    {
                        // Exclude this tile as it's alone after an adjacent tile.
                        tilesInRow[col, rowIndex] = null;
                    }
                }
            }
        }

        return tilesInRow;
    }

    // Function to find and return movable tiles in a specified column.
    public Transform[,] FindAdjacentMovableTilesInColumn(int columnIndex)
    {
        Debug.Log("colindex in findmovable " + columnIndex);

        int numRows = movableTiles.GetLength(1); // Assuming movableTiles is in [col, row] format.
        int numCols = movableTiles.GetLength(0);
        Transform[,] tilesInColumn = new Transform[numCols, numRows];

        bool foundAdjacentTile = false; // Flag to track if an adjacent tile has been found.

        if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(0))
        {
            for (int row = 0; row < numRows; row++)
            {
                if (movableTiles[columnIndex, row] != null)
                {
                    // Check if the tile has an adjacent tile above or below.
                    bool hasAboveAdjacent = (row > 0 && movableTiles[columnIndex, row - 1] != null);
                    bool hasBelowAdjacent = (row < numRows - 1 && movableTiles[columnIndex, row + 1] != null);

                    if (hasAboveAdjacent || hasBelowAdjacent)
                    {
                        // Include this tile in the movable column.
                        tilesInColumn[columnIndex, row] = movableTiles[columnIndex, row];
                        foundAdjacentTile = true; // Set the flag to true.
                    }
                    else if (foundAdjacentTile)
                    {
                        // Exclude this tile as it's alone after an adjacent tile.
                        tilesInColumn[columnIndex, row] = null;
                    }
                }
            }
        }

        return tilesInColumn;
    }

    public Transform[,] TutorialLevelFindCurrentMovables()
    {
        if (!firstMovementDone)
        {
            currentMoveType = "horizontal";
            rowIndex = 5;

            currentMovableTiles = FindAdjacentMovableTilesInRow(rowIndex);
        }
        else if (firstMovementDone)
        {
            currentMoveType = "vertical";
            columnIndex = 5;

            currentMovableTiles = FindAdjacentMovableTilesInColumn(columnIndex);
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

                }
            }
        }
        return initialTilePositions;
    }

    public void TutorialCompleted()
    {
        if(firstMovementDone)
        {
            Destroy(movableTiles[6, 5].gameObject);
            Debug.Log("tutorial completed");
            tutorialDone = true;

            GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
            if (levelCompletedBox != null)
            {
                animator = levelCompletedBox.GetComponent<Animator>();
            }
            animator.SetTrigger("LevelEnd");
        }
        
    }

    public void ChangeMovementDone()
    {
        firstMovementDone = true;
        currentMoveType = "vertical";
        UpdateMovableTilesArray();
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

