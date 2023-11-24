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
    public int moveCount = 0;

    void Start()
    {
        // Attach the method to the button click event
        //showStarsButton.onClick.AddListener(OnShowStarsButtonClick);

        // Get references to MovableTileGrid
        movableTileGrid = GameObject.FindGameObjectWithTag("MovableTileGrid").GetComponent<MovableTileGrid>();

        // Set current level
        currentLevel = movableTileGrid.selectedLevel;
        Debug.Log("Current level: " + currentLevel);
        
    }

    public void SetCounter()
    {
        int moveCounter = PlayerPrefs.GetInt("counter");
        Debug.Log("Moves in SetCounter = " + moveCounter);
        moveCount = moveCounter;
    }


    public void OnShowStarsButtonClick()
    {
        // Call ShowStars after a delay
        StartCoroutine(StartStarsWithDelay(1f));
    }



    private IEnumerator ShowStarsRoutine(int numberOfStars)
    {
        foreach (Star star in Stars)
        {
            star.YellowStar.transform.localScale = Vector3.zero;
        }

        // Ensure the number of stars does not exceed the length of the array
        int maxIndex = Mathf.Min(numberOfStars, Stars.Length);
        for (int i = 0; i < maxIndex; i++)
        {
            yield return StartCoroutine(EnlargeAndShrinkStar(Stars[i]));
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
        SetCounter();
        
        Debug.Log($"Calculating stars for {moveCount} moves.");

        // Switch case for different levels
        switch (currentLevel)
        {
            case 1:
                if (moveCount <= 3) return 3;
                else if (moveCount <= 5) return 2; 
                else return 1; 

            case 2:
                if (moveCount <= 3) return 3;
                else if (moveCount <= 5) return 2;
                else return 1;
            
            case 3:
                if (moveCount <= 3) return 3;
                else if (moveCount <= 5) return 2;
                else return 1;
            case 4:
                if (moveCount <= 5) return 3;
                else if (moveCount <= 7) return 2;
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
