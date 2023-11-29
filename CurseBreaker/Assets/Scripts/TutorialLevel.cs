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
    public GameObject handPrefab;
    public GameObject lynaraPrefab;
    public GameObject dialogBubblePrefab;

    public GameObject hand;
    private GameObject lynara;
    private GameObject dialogBubble;
    public Image overlay;

    private string dialogue1 = "Hi there! I'm here to guide you.";
    private string dialogue2 = "Your objective is to move the yellow tiles row by row so that the red tile isn't connected to the yellow ones.";
    private string dialogue3 = "In this game, you have to move the puzzles so that every other move is horizontal and every other vertical.";
    private string dialogue4 = "Here is the arrow telling if your move needs to be horizontal or vertical.";
    private string dialogue5 = "To move a row, just tap and hold it, then drag it to right.";
    private string dialogue6 = "Oh no! Now the level failed! The row was dragged too far from the tile cluster and the yellow tiles dropped!";
    private string dialogue7 = "Let's try again! To move a row, just tap and hold it, then drag it to right.";
    private string dialogue8 = "Great job! Now, you need to know that there must be atleast 2 tiles in the row for you to be able to move it.";
    private string dialogue9 = "Let’s do another one! Tap and hold the column in the middle and slide it downwards.";
    private string dialogue10 = "Fantastic! You’ve completed the level.";
    private string dialogue11 = "Now that you've mastered the basics, it's time to tackle more challenging puzzles!";

    public BackgroundGrid backgroundGrid;
    GameObject animation_hand;
    GameObject yellow_glow;

    public Animator animator;
    public Animator tutorial_animator;

    public bool firstMovementDone = false;
    public bool tutorialDone = false;

    public Vector3[,] initialTilePositions;

    public float tapCooldown = 0.5f; // Time in seconds to ignore taps after the first tap
    private float lastTapTime;
    int clickCount = 0; // Counter for clicks

    private void Start()
    {
        backgroundGrid = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundGrid>();
        backgroundGrid.GenerateBackgroundGrid(10, 10);

        animation_hand = GameObject.Find("Hand");
        animation_hand.SetActive(false);
        yellow_glow = GameObject.Find("YellowGlow");
        yellow_glow.SetActive(false);
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
        Vector3 handposition = new Vector3(0.8f, 2, 0);
        hand = Instantiate(handPrefab, handposition, Quaternion.identity);
        hand.transform.rotation = Quaternion.Euler(0, 0, 12);
        hand.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        hand.GetComponent<SpriteRenderer>().sortingOrder = 2;
        hand.SetActive(false);

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

        
        GameObject tilemoving = GameObject.Find("TileMovingAnimation");
        if (tilemoving != null)
        {
            tutorial_animator = tilemoving.GetComponent<Animator>();
        }

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
                hand.SetActive(true);
                tmp.text = dialogue4;
                break;
            case 5:
                hand.SetActive(false);
                tmp.text = dialogue5;
                break;
            case 6:
                animation_hand.SetActive(true);
                tutorial_animator.SetBool("tutorial_fail", true);
                tmp.text = dialogue6;
                break;
            case 7:
                tutorial_animator.SetBool("tutorial_fail", false);
                tutorial_animator.SetTrigger("tutorial_horizontal");
                tmp.text = dialogue7;
                break;
            case 8:
                tmp.text = dialogue8;
                break;
            case 9:
                yellow_glow.SetActive(true);
                tutorial_animator.SetTrigger("tutorial_vertical");
                tmp.text = dialogue9;
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
            hand.SetActive(false);
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
        hand.SetActive(false);
        hand.transform.position = new Vector3(1.2f, 0, 0);
        hand.transform.rotation = Quaternion.Euler(0, 0, 270);

    }
}

