using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class WellDoneScreenManager : MonoBehaviour
{
    [SerializeField] Star[] Stars;
    [SerializeField] float EnlargeScale = 1.5f;
    [SerializeField] float ShrinkScale = 1f;
    [SerializeField] float EnlargeDuration = 0.25f;
    [SerializeField] float ShrinkDuration = 0.25f;

    // Reference to the button that triggers the stars
    [SerializeField] Button showStarsButton;

    public MovableTileGrid movableTileGrid;
    //private MovableTileDrag movableTileDrag;
    public int currentLevel; // Current level number
    public int moveCounter = 1;

    void Start()
    {
        // Attach the method to the button click event
        //showStarsButton.onClick.AddListener(OnShowStarsButtonClick);

        // Get references to MovableTileGrid and MovableTileDrag
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();
        //movableTileDrag = GameObject.FindGameObjectWithTag("MovableTileDrag").GetComponent<MovableTileDrag>();

        // Set current level
        currentLevel = movableTileGrid.selectedLevel;

        
    }

    public void LoadCounter()
    {
        int moveCounter = PlayerPrefs.GetInt("counter");
        Debug.Log(moveCounter);
    }

    public void OnShowStarsButtonClick()
    {
        // Call ShowStars after a delay
        StartCoroutine(StartStarsWithDelay(1f));
    }



private IEnumerator ShowStarsRoutine(int numberOfStars)
{
    Debug.Log($"Showing {numberOfStars} stars");

    foreach (Star star in Stars)
    {
        if (star != null && star.YellowStar != null)
        {
            star.YellowStar.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogError("Star or YellowStar is null");
        }
    }

        int maxIndex = Math.Min(numberOfStars, Stars.Length);
        for (int i = 0; i < maxIndex; i++)
        {
        if (Stars[i] != null)
        {
            yield return StartCoroutine(EnlargeAndShrinkStar(Stars[i]));
        }
        else
        {
            Debug.LogError($"Star {i} is null");
        }
    }
}
    public IEnumerator StartStarsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        
        ShowStars(CalculateStarsBasedOnMoves()); // Pass the move counter here
    }

    public void ShowStars(int numberOfStars)
    {
        StartCoroutine(ShowStarsRoutine(numberOfStars));
    }

    public int CalculateStarsBasedOnMoves()
    {
        // Ensure this method initializes moveCounter properly
        //moveCounter = movableTileGrid.getMoveCount(moveCounter);
        //Debug.Log($"MoveCounter in StartsWithDelay  {moveCounter}");
        //Debug.Log($"Calculating stars for {moveCounter} moves.");
        // Switch case for different levels
        switch (currentLevel)
        {
            case 1:
                if (moveCounter <= 3) return 3;
                else if (moveCounter <= 5) return 2; 
                else return 1; 

            case 2:
                if (moveCounter <= 3) return 3;
                else if (moveCounter <= 5) return 2;
                else return 1;
            
            case 3:
                if (moveCounter <= 3) return 3;
                else if (moveCounter <= 5) return 2;
                else return 1;
            default:
                Debug.Log("Default case hit in CalculateStarsBasedOnMoves");
                return 1; // Default to 1 star if level not recognized
        }
    }


    private IEnumerator EnlargeAndShrinkStar(Star star)
    {
        yield return StartCoroutine(ChangeStarScale(star, EnlargeScale, EnlargeDuration));
        yield return StartCoroutine(ChangeStarScale(star, ShrinkScale, ShrinkDuration));
    }

    private IEnumerator ChangeStarScale(Star star, float targetScale, float duration)
    {
        Vector3 initialScale = star.YellowStar.transform.localScale;
        Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            star.YellowStar.transform.localScale = Vector3.Lerp(initialScale, finalScale, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        star.YellowStar.transform.localScale = finalScale;
    }
}
