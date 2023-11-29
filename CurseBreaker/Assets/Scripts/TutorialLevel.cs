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
    private string dialogue4 = "To move a row, just tap and hold it, then drag it to right.";
    private string dialogue5 = "Great job! Now, you need to know that there must be atleast 2 tiles in the row for you to be able to move it.";
    private string dialogue6 = "Let’s do another one! Tap and hold the column in the middle and slide it downwards.";
    private string dialogue7 = "Fantastic! You’ve completed the level.";
    private string dialogue8 = "Now that you've mastered the basics, it's time to tackle more challenging puzzles!";

    public TextAsset csvFile;
    public BackgroundGrid backgroundGrid;
    
    public Animator animator;

    public bool firstMovementDone = false;
    public bool tutorialDone = false;

    public int gridSizeX = 10; //number of columns, width of the grid
    public int gridSizeY = 10; //number of rows, height of the grid

    private Transform[,] currentMovableTiles;
    private Transform[,] movableTiles;
    public Vector3[,] initialTilePositions;

    public float tapCooldown = 0.5f; // Time in seconds to ignore taps after the first tap
    private float lastTapTime;
    int clickCount = 0; // Counter for clicks

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        backgroundGrid.GenerateBackgroundGrid(7, 7);

        GameObject overlayObject = GameObject.Find("Overlay");
        overlay = overlayObject.GetComponent<Image>();

        GeneratePrefabs();
        LogTutorialStartEvent();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Store the initial touch position
                    if (Time.time - lastTapTime > tapCooldown)
                    {
                        lastTapTime = Time.time;
                        clickCount++; // Increment the click counter
                        Debug.Log("clicks " + clickCount);
                        ShowNextSpeechBubble(clickCount);
                    }
                    break;
   
            }
        }
    }

    private void LogTutorialStartEvent()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_started");   
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

    }
}

