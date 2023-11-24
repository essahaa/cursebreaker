using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

public class MovableTileGrid : MonoBehaviour
{
    public GameObject movableTilePrefab;
    public GameObject evilTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject keyTilePrefab;
    public GameObject arrowPrefab;

    private GameObject arrow;

    public TextAsset csvFile; // Reference to your CSV file in Unity (assign it in the Inspector).

    public float movableTileSize = 1.0f; // Adjust the size of movable tiles.
    public Animator animator;
    
    public int gridSizeX; //number of columns, width of the grid
    public int gridSizeY; //number of rows, height of the grid

    public int selectedLevel; // The level you want to generate.
    private int rotation = 0;

    public Transform[,] movableTiles; // Change to a Transform[,] array.

    private bool backgroundGenerated = false;
    private bool[,] visited;

    private List<string> csvLines = new List<string>(); // Store CSV lines in a list.

    void Start()
    {
        ReadCSV(); // Read the CSV file.
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        selectedLevel = PlayerPrefs.GetInt("selectedLevel");
        if(selectedLevel > 0)
        {
            LoadLevel(selectedLevel);
        }else
        {
            PlayerPrefs.SetInt("selectedLevel", 1);
            selectedLevel = 1;
            LoadLevel(selectedLevel);
        }
    }

    public int CheckSelectedLevel()
    {
        return selectedLevel;
    }

    private void GenerateArrowPrefab()
    {
        Vector3 arrowposition = new Vector3(0f, 3f, 0);
        arrow = Instantiate(arrowPrefab, arrowposition, Quaternion.identity);
        
    }

    public void RotateArrow()
    {
        if(rotation > 360)
        {
            rotation = 0;
        } 
        else
        {
            rotation += 90;
        }
        
        arrow.transform.rotation = Quaternion.Euler(0, 0, rotation);
        Debug.Log("rotated " + rotation);
    }

    public void LoadSceneAndLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("selectedLevel", levelNumber);
        SceneManager.LoadScene("Gameboard");
    }

    public void LoadLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("selectedLevel", levelNumber);
        selectedLevel = levelNumber;
        //TÄHÄN TILALLE LEVELMANAGERIN LEVELDATAN KAUTTA TIEDOT 
        ReadLevelDataFromCSV();
    }

    public void NextLevel()
    {
        int newSelectedLevel = selectedLevel + 1;
        PlayerPrefs.SetInt("selectedLevel", newSelectedLevel);
        selectedLevel = newSelectedLevel;
        DestroyExistingMovableTiles();
    }

    private void ReadCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        csvLines.AddRange(lines);
    }

    public void ReadLevelDataFromCSV()
    {
        bool arraySizeSet = false; // Add a flag to track if array size is set.
        bool noMoreLevels = true; // Flag to check if there are no more levels.

        if (csvLines.Count > 0)
        {
            foreach (string line in csvLines)
            {
                string[] values = line.Split(';'); // Split each line into values.

                // Check if the current line corresponds to the target level.
                if (values.Length >= 1 && int.TryParse(values[0], out int level) && level == selectedLevel)
                {
                    noMoreLevels = false; // We found a matching level.
                    Debug.Log("level " + values[0] + " selectedlevel " + selectedLevel);
                    // Parse data from the CSV line.
                    int column = int.Parse(values[1]);
                    int row = int.Parse(values[2]);
                    string tileType = values[3];
                    gridSizeX = int.Parse(values[4]);
                    gridSizeY = int.Parse(values[5]);
                    string isLockedStr = values[6].ToLower(); // Convert to lowercase to handle case-insensitivity
                    bool isLocked = isLockedStr == "true";
                    bool isKey = values[7].ToLower() == "true";



                    // Set the array size only once.
                    if (!arraySizeSet)
                    {
                        movableTiles = new Transform[gridSizeX, gridSizeY];
                        arraySizeSet = true; // Update the flag.
                    }

                    GenerateTileFromCSV(column, row, tileType, gridSizeX, gridSizeY, isLocked, isKey);
                }
            }
        }
        if (noMoreLevels)
        {
            Debug.Log("No more levels in the CSV file.");
            SceneManager.LoadScene("MainMenu");
        }

    }

    private GameObject GetTilePrefab(string tileType, bool isLocked, bool isKey)
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

    private string GetAnimationController(string tileType)
    {
        switch (tileType)
        {
            case "Normal":
                return "TileController";
            case "Evil":
                return "EvilTileController";
            default:
                return null;
        }
    }

    void GenerateTileFromCSV(int column, int row, string tileType, int gridSizeX, int gridSizeY, bool isLocked, bool isKey)
    {
        BackgroundGrid backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();

        if (!backgroundGenerated)
        {
            backgroundGrid.GenerateBackgroundGrid(gridSizeX, gridSizeY);
            backgroundGenerated = true;
            GenerateArrowPrefab();
        }
        
        GameObject tilePrefab = GetTilePrefab(tileType, isLocked, isKey);

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
                // add individual Animator controllers
                tile.AddComponent<Animator>(); 
                Animator animator = tile.GetComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(GetAnimationController(tileType));

                MovableTile tileData = tile.GetComponent<MovableTile>();
                tileData.Level = selectedLevel;
                tileData.Row = row;
                tileData.Column = column;
                tileData.TileType = tileType;
                tileData.GridSizeX = gridSizeX;
                tileData.GridSizeY = gridSizeY;
                tileData.IsLocked = isLocked;
                tileData.IsKey = isKey;
                // Check if the lock tile is being created
                if(isLocked)
                {
                    CreateLockTileOnMovableTile(column, row, isLocked);
                }
                if(isKey)
                {
                    CreateKeyTileOnMovableTile(column, row, isKey);
                }
   
            }
        }
    }

    void CreateLockTileOnMovableTile(int column, int row, bool isLocked)
    {
        BackgroundGrid backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        //Debug.Log("CreateLockTileOnMovableTile - Column: " + column + ", Row: " + row + ", isLocked: " + isLocked);

        if (column >= 0 && column < gridSizeX && row >= 0 && row < gridSizeY)
        {
            if (movableTiles != null && movableTiles[column, row] != null)
            {
                if (isLocked)
                {
                    GameObject lockTile = Instantiate(lockTilePrefab, movableTiles[column, row].position, Quaternion.identity);
                    lockTile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
                    lockTile.transform.SetParent(movableTiles[column, row]);
                }
            }
        }
    }
    void CreateKeyTileOnMovableTile(int column, int row, bool isKey)
    {
        BackgroundGrid backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        Debug.Log("CreateKeyTileOnMovableTile - Column: " + column + ", Row: " + row + ", isKey: " + isKey);

        if (column >= 0 && column < gridSizeX && row >= 0 && row < gridSizeY)
        {
            if (movableTiles != null && movableTiles[column, row] != null)
            {
                if (isKey)
                {
                    GameObject keyTile = Instantiate(keyTilePrefab, movableTiles[column, row].position, Quaternion.identity);
                    keyTile.transform.localScale = new Vector3(backgroundGrid.backgroundTileSize, backgroundGrid.backgroundTileSize, 1);
                    keyTile.transform.SetParent(movableTiles[column, row]);

                    MovableTile component = keyTile.GetComponent<MovableTile>();
                    component.Column = column;
                    component.Row = row;
                }
            }
        }
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
        Destroy(arrow);

        // Loop through and destroy each GameObject
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }

        backgroundGenerated = false;
        // Generate new movable tiles (and evil tiles if needed).
        ReadLevelDataFromCSV();
        FindObjectOfType<AudioManager>().StopPlaying("youfail");
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

                if (tile != null && (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile")) && CheckNeighbours(col, row) != true)
                {
                    //no neighbors found, destroy tile
                    FindObjectOfType<AudioManager>().Play("riddedred");
                    Debug.Log("destroy tile " + col + " , " + row);

                    movableTiles[col, row] = null;

                    GameObject tileToDestroy = tile.gameObject; // Get the GameObject.
                    Destroy(tileToDestroy);

                    if(tile.CompareTag("MovableTile"))
                    {
                        Debug.Log("level failed koska yks tippu");
                        GameObject levelFailedBox = GameObject.Find("LevelFailedBox");
                        if (levelFailedBox != null)
                        {
                            animator = levelFailedBox.GetComponent<Animator>();
                        }
                        animator.SetTrigger("LevelEnd");
                        FindObjectOfType<AudioManager>().Play("youfail");
                    }
                    else
                    {
                        if(CountEvilTiles() == 0)
                        {
                            Debug.Log("level completed, evil tiles count: " + CountEvilTiles());
                            GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
                            if (levelCompletedBox != null)
                            {
                                animator = levelCompletedBox.GetComponent<Animator>();
                            }
                            animator.SetTrigger("LevelEnd");

                        }
                        
                    }
                }
               
            }
        }

        return movableTiles;
    }

    private int CountEvilTiles()
    {
        int count = 0;

        for (int row = 0; row < gridSizeY; row++)
        {
            for (int col = 0; col < gridSizeX; col++)
            {
                Transform movableTile = movableTiles[col, row];

                if (movableTile != null)
                {
                    MovableTile movableTileComponent = movableTile.GetComponent<MovableTile>();

                    if (movableTileComponent != null && movableTileComponent.TileType == "Evil")
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }


    private bool CheckNeighbours(int col, int row)
    {
        bool hasMovableTile = false;

        // Check the right neighbor
        if (col < gridSizeX - 1)
        {
            Transform rightNeighbor = movableTiles[col + 1, row];
            if (rightNeighbor != null && (rightNeighbor.CompareTag("MovableTile") || rightNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        // Check the left neighbor
        if (col > 0)
        {
            Transform leftNeighbor = movableTiles[col - 1, row];
            if (leftNeighbor != null && (leftNeighbor.CompareTag("MovableTile") || leftNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        // Check the upper neighbor
        if (row < gridSizeY - 1)
        {
            Transform upperNeighbor = movableTiles[col, row + 1];
            if (upperNeighbor != null && (upperNeighbor.CompareTag("MovableTile") || upperNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        // Check the lower neighbor
        if (row > 0)
        {
            Transform lowerNeighbor = movableTiles[col, row - 1];
            if (lowerNeighbor != null && (lowerNeighbor.CompareTag("MovableTile") || lowerNeighbor.CompareTag("EvilTile")))
            {
                hasMovableTile = true;
            }
        }

        return hasMovableTile;
    }

    public Transform[,] FindAdjacentMovableTilesInRow(int rowIndex)
    {
        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        int numRows = movableTiles.GetLength(1);
        Transform[,] tilesInRow = new Transform[numCols, numRows]; // Only declared once at the start.

        // Initialize all tiles as null or some default state as required.
        for (int col = 0; col < numCols; col++)
        {
            tilesInRow[col, rowIndex] = null;
        }

        bool inSequence = false;
        List<Transform> sequenceTiles = new List<Transform>();

        for (int col = 0; col < numCols; col++)
        {
            Transform tile = movableTiles[col, rowIndex];

            // If we encounter a LockTile or null, reset the sequence.
            if (tile == null || (tile.GetComponent<MovableTile>().IsLocked))
            {
                if (inSequence && sequenceTiles.Count >= 2) // Assuming a valid row needs at least 2 tiles.
                {
                    foreach (Transform validTile in sequenceTiles)
                    {
                        MovableTile tileData = validTile.GetComponent<MovableTile>();
                        tilesInRow[tileData.Column, rowIndex] = validTile;
                        // Lock the parent tile if it has a child LockTile.
                        if (HasChildLockTile(validTile))
                        {
                            tileData.IsLocked = true;
                        }
                    }
                }
                // Reset the sequence regardless of its validity because of the LockTile or gap.
                sequenceTiles.Clear();
                inSequence = false;
                continue;
            }

            // Add to current sequence if we are in one.
            if (inSequence)
            {
                sequenceTiles.Add(tile);
            }
            else
            {
                // Start a new sequence if we're not already in one.
                sequenceTiles.Clear();
                sequenceTiles.Add(tile);
                inSequence = true;
            }
        }

        // Handle the case where the last tile in the row is not a LockTile and we have a valid sequence.
        if (inSequence && sequenceTiles.Count >= 2)
        {
            foreach (Transform validTile in sequenceTiles)
            {
                MovableTile tileData = validTile.GetComponent<MovableTile>();
                tilesInRow[tileData.Column, rowIndex] = validTile;
                // Lock the parent tile if it has a child LockTile.
                if (HasChildLockTile(validTile))
                {
                    tileData.IsLocked = true;
                }
            }
        }

        return tilesInRow;
    }

    public Transform[,] FindAdjacentMovableTilesInColumn(int columnIndex)
    {
        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        int numRows = movableTiles.GetLength(1);
        Transform[,] tilesInColumn = new Transform[numCols, numRows]; // Only declared once at the start.

        // Initialize all tiles in the column as null or some default state as required.
        for (int row = 0; row < numRows; row++)
        {
            tilesInColumn[columnIndex, row] = null;
        }

        bool inSequence = false;
        List<Transform> sequenceTiles = new List<Transform>();

        for (int row = 0; row < numRows; row++)
        {
            Transform tile = movableTiles[columnIndex, row];

            // If we encounter a LockTile or null, reset the sequence.
            if (tile == null || (tile.GetComponent<MovableTile>().IsLocked))
            {
                if (inSequence && sequenceTiles.Count >= 2) // Assuming a valid column needs at least 2 tiles.
                {
                    foreach (Transform validTile in sequenceTiles)
                    {
                        MovableTile tileData = validTile.GetComponent<MovableTile>();
                        tilesInColumn[columnIndex, tileData.Row] = validTile;
                        // Lock the parent tile if it has a child LockTile.
                        if (HasChildLockTile(validTile))
                        {
                            tileData.IsLocked = true;
                        }
                    }
                }
                // Reset the sequence regardless of its validity because of the LockTile or gap.
                sequenceTiles.Clear();
                inSequence = false;
                continue;
            }

            // Add to current sequence if we are in one.
            if (inSequence)
            {
                sequenceTiles.Add(tile);
            }
            else
            {
                // Start a new sequence if we're not already in one.
                sequenceTiles.Clear();
                sequenceTiles.Add(tile);
                inSequence = true;
            }
        }

        // Handle the case where the last tile in the column is not a LockTile and we have a valid sequence.
        if (inSequence && sequenceTiles.Count >= 2)
        {
            foreach (Transform validTile in sequenceTiles)
            {
                MovableTile tileData = validTile.GetComponent<MovableTile>();
                tilesInColumn[columnIndex, tileData.Row] = validTile;
                // Lock the parent tile if it has a child LockTile.
                if (HasChildLockTile(validTile))
                {
                    tileData.IsLocked = true;
                }
            }
        }

        return tilesInColumn;
    }

        private bool HasChildLockTile(Transform parentTile)
    {
        // Iterate through child objects of the parentTile.
        foreach (Transform child in parentTile)
        {
            // Check if the child is a LockTile.
            if (child.CompareTag("LockTile"))
            {
                return true;
            }
        }

        // Return false if no child LockTile was found.
        return false;
    }

    // Function to find and return movable tiles in a specified row.
    public Transform[,] FindAllMovableTilesInRow(int rowIndex)
    {
        Debug.Log("rowindex in findmovable " + rowIndex);

        int numRows = movableTiles.GetLength(1);
        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        Transform[,] tilesInRow = new Transform[numCols, numRows];

        if (rowIndex >= 0 && rowIndex < movableTiles.GetLength(1))
        {
            for (int col = 0; col < numCols; col++)
            {
                // Check if the tile exists and is not locked
                if (movableTiles[col, rowIndex] != null && !movableTiles[col, rowIndex].GetComponent<MovableTile>().IsLocked)
                {
                    tilesInRow[col, rowIndex] = movableTiles[col, rowIndex].transform;
                }
                else
                {
                    tilesInRow[col, rowIndex] = null; // Set to null if the tile is locked or doesn't exist
                }
            }
        }

        return tilesInRow;
    }

    // Function to find and return movable tiles in a specified column.
    public Transform[,] FindAllMovableTilesInColumn(int columnIndex)
    {
        Debug.Log("colindex in findmovable " + columnIndex);

        int numCols = movableTiles.GetLength(0); // Assuming movableTiles is in [col, row] format.
        int numRows = movableTiles.GetLength(1);
        Transform[,] tilesInColumn = new Transform[numCols, numRows];

        if (columnIndex >= 0 && columnIndex < movableTiles.GetLength(0))
        {
            for (int row = 0; row < numRows; row++)
            {
                // Check if the tile exists and is not locked
                if (movableTiles[columnIndex, row] != null && !movableTiles[columnIndex, row].GetComponent<MovableTile>().IsLocked)
                {
                    tilesInColumn[columnIndex, row] = movableTiles[columnIndex, row].transform;
                }
                else
                {
                    tilesInColumn[columnIndex, row] = null; // Set to null if the tile is locked or doesn't exist
                }
            }
        }

        return tilesInColumn;
    }

    private bool IsRelevantTile(Transform tile)
    {
        return tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile") || tile.CompareTag("LockTile");
    }

    public bool IsMovableTilesGroupConnected()
    {
        visited = new bool[gridSizeX, gridSizeY];
        List<Transform> largestGroupTiles = new List<Transform>();
        bool movableTileDestroyed = false;
        bool evilTileDestroyed = false;

        // Iterate over all tiles to find the largest connected group
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Transform tile = movableTiles[x, y];
                if (tile != null && !visited[x, y] && IsRelevantTile(tile))
                {
                    List<Transform> currentGroupTiles = DepthFirstSearch(tile, visited);
                    if (currentGroupTiles.Count > largestGroupTiles.Count)
                    {
                        largestGroupTiles = currentGroupTiles;
                    }
                }
            }
        }

        // Destroy tiles not in the largest group and check their types
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Transform tile = movableTiles[x, y];
                if (tile != null && !largestGroupTiles.Contains(tile))
                {
                    if (tile.CompareTag("MovableTile"))
                    {
                        movableTileDestroyed = true;
                    }
                    else if (tile.CompareTag("EvilTile"))
                    {
                        evilTileDestroyed = true;
                    } 
                    Destroy(tile.gameObject);
                    movableTiles[x, y] = null;
                }
            }
        }

        if (movableTileDestroyed)
        {
            // Logic to handle level failure
            HandleLevelFailure();
            return false;
        }
        else if (evilTileDestroyed)
        {
            // Continue gameplay if only EvilTiles are destroyed
            Debug.Log("The disconnected group contains only EvilTiles.");
            if (CountEvilTiles() == 0)
            {
                Debug.Log("level completed, evil tiles count: " + CountEvilTiles());
                GameObject levelCompletedBox = GameObject.Find("LevelCompletedBox");
                if (levelCompletedBox != null)
                {
                    animator = levelCompletedBox.GetComponent<Animator>();
                }
                animator.SetTrigger("LevelEnd");

            }
            return true;
        }
        else
        {
            return true;
        }
    }

    private void HandleLevelFailure()
    {
        // Implement the logic to handle level failure
        // e.g., showing a game over screen, resetting the level, etc.
        Debug.Log("Game Over: MovableTiles group is not connected.");
        GameObject levelFailedBox = GameObject.Find("LevelFailedBox");
        if (levelFailedBox != null)
        {
            animator = levelFailedBox.GetComponent<Animator>();
        }
        animator.SetTrigger("LevelEnd");
    }

    private List<Transform> DepthFirstSearch(Transform tile, bool[,] visited)
    {
        List<Transform> groupTiles = new List<Transform>();
        InternalDFS(tile, visited, groupTiles);
        return groupTiles;
    }

    private void InternalDFS(Transform tile, bool[,] visited, List<Transform> groupTiles)
    {
        int x = tile.GetComponent<MovableTile>().Column;
        int y = tile.GetComponent<MovableTile>().Row;

        visited[x, y] = true;
        groupTiles.Add(tile);

        // Check the left neighbor
        if (x > 0 && !visited[x - 1, y])
        {
            Transform leftNeighbor = movableTiles[x - 1, y];
            if (leftNeighbor != null && IsTileOrChildConnectable(leftNeighbor))
            {
                InternalDFS(leftNeighbor, visited, groupTiles);
            }
        }

        // Check the right neighbor
        if (x < gridSizeX - 1 && !visited[x + 1, y])
        {
            Transform rightNeighbor = movableTiles[x + 1, y];
            if (rightNeighbor != null && IsTileOrChildConnectable(rightNeighbor))
            {
                InternalDFS(rightNeighbor, visited, groupTiles);
            }
        }

        // Check the lower neighbor
        if (y > 0 && !visited[x, y - 1])
        {
            Transform lowerNeighbor = movableTiles[x, y - 1];
            if (lowerNeighbor != null && IsTileOrChildConnectable(lowerNeighbor))
            {
                InternalDFS(lowerNeighbor, visited, groupTiles);
            }
        }

        // Check the upper neighbor
        if (y < gridSizeY - 1 && !visited[x, y + 1])
        {
            Transform upperNeighbor = movableTiles[x, y + 1];
            if (upperNeighbor != null && IsTileOrChildConnectable(upperNeighbor))
            {
                InternalDFS(upperNeighbor, visited, groupTiles);
            }
        }
    }

    private bool IsTileOrChildConnectable(Transform tile)
    {
        // Check if the tile itself is connectable
        if (tile.CompareTag("MovableTile") || tile.CompareTag("EvilTile"))
        {
            return true;
        }

        // Check if any of the children is a locked but connectable tile
        foreach (Transform child in tile)
        {
            if (child.CompareTag("LockTile") || child.CompareTag("KeyTile"))
            {
                return true; // Considering locked tiles as connectors
            }
        }

        return false;
    }

    public void DestroyKeyAndLockTilesIfNeighbor()
{
    for (int x = 0; x < gridSizeX; x++)
    {
        for (int y = 0; y < gridSizeY; y++)
        {
            Transform tile = movableTiles[x, y];

            if (tile != null && HasKeyTileChild(tile))
            {
                // Check neighbors in all four directions.
                if (IsLockTileAt(x + 1, y))
                {
                    DestroyChildTiles(tile);
                    DestroyChildTiles(movableTiles[x + 1, y]);
                }
                if (IsLockTileAt(x - 1, y))
                {
                    DestroyChildTiles(tile);
                    DestroyChildTiles(movableTiles[x - 1, y]);
                }
                if (IsLockTileAt(x, y + 1))
                {
                    DestroyChildTiles(tile);
                    DestroyChildTiles(movableTiles[x, y + 1]);
                }
                if (IsLockTileAt(x, y - 1))
                {
                    DestroyChildTiles(tile);
                    DestroyChildTiles(movableTiles[x, y - 1]);
                }
            }
        }
    }
}

    private bool HasKeyTileChild(Transform parentTile)
    {
        // Iterate through child objects of the parentTile.
        foreach (Transform child in parentTile)
        {
            // Check if the child is a KeyTile.
            if (child.CompareTag("KeyTile"))
            {
                return true;
            }
        }

        return false; // No KeyTile child found.
    }

    private bool IsLockTileAt(int x, int y)
    {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            Transform tile = movableTiles[x, y];

            if (tile != null)
            {
                // Check if any of the child objects have the "LockTile" tag.
                foreach (Transform child in tile)
                {
                    if (child.CompareTag("LockTile"))
                    {
                        return true;
                    }
                }
            }
        }

        return false; // Coordinates are out of bounds or no LockTile found.
    }


    private void DestroyChildTiles(Transform parentTile)
    {
        bool lockTileDestroyed = false; // Flag to check if a lock tile was destroyed

        // Iterate through child objects of the parentTile.
        foreach (Transform child in parentTile)
        {
            // Check if the child is a KeyTile or LockTile.
            if (child.CompareTag("KeyTile") || child.CompareTag("LockTile"))
            {
                // Destroy the child object.
                Destroy(child.gameObject);

                if (child.CompareTag("LockTile"))
                {
                    lockTileDestroyed = true;
                }
            }
        }

        // If a lock tile was destroyed, update the parent tile's isLocked status
        if (lockTileDestroyed)
        {
            MovableTile parentTileComponent = parentTile.GetComponent<MovableTile>();
            if (parentTileComponent != null)
            {
                parentTileComponent.IsLocked = false;
            }
        }
    }

}

