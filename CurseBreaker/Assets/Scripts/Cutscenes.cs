using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cutscenes : MonoBehaviour
{
    private int tapCounter;
    private bool countTaps = false;
    private string cutSceneToPlay;

    public GameObject beginningCutscenePrefab;
    public Sprite dialogBoxLeft;

    private GameObject beginningCanvas;
    private TextMeshProUGUI frameText;
    private Image dialogImage;
    private TextMeshProUGUI dialogText;

    public string beginningText1 = "Evil sorcerer Keilith has cursed the town of Lumina and its villagers.";
    public string beginningText2 = "Only the young healer Lynara can save the village.";
    public string beginningText3 = "Can you help her?";
    public string beginningText4 = "I have been cursed. Please cure me Lynara.";
    public string beginningText5 = "Hello stranger! Help me break Ms. Bunnies curse.";

    private void Update()
    {
        if (countTaps && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                tapCounter++;
                if (cutSceneToPlay == "beginning")
                {
                    ContinueBeginningCutscene();
                }
            }
        }
    }

    public void PlayBeginningCutscene()
    {
        beginningCanvas = Instantiate(beginningCutscenePrefab, new Vector3(0, 0, 0), Quaternion.identity);

        Transform frame = beginningCanvas.transform.Find("Frame");
        frameText = frame.GetComponentInChildren<TextMeshProUGUI>();
        Transform dialogBox = beginningCanvas.transform.Find("DialogBox");
        dialogImage = dialogBox.GetComponent<Image>();
        dialogText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();

        cutSceneToPlay = "beginning";
        countTaps = true;
        tapCounter = 1;
    }

    public void ContinueBeginningCutscene()
    {
        Animator animator = beginningCanvas.GetComponent<Animator>();

        switch (tapCounter)
        {
            case 1:
                frameText.text = beginningText1;
                break;
            case 2:
                frameText.text = beginningText2;
                animator.SetTrigger("RevealLynara");
                break;
            case 3:
                frameText.text = beginningText3;
                break;
            case 4:
                animator.SetTrigger("GoToCottage");
                dialogText.text = beginningText4;
                break;
            case 5:
                dialogImage.sprite = dialogBoxLeft;
                dialogText.text = beginningText5;
                animator.SetTrigger("GoToCottage");
                break;
            default:
                SceneLoader loader = GameObject.FindGameObjectWithTag("SceneLoader").GetComponent<SceneLoader>();
                loader.LoadScene("Gameboard");
                break;
        }
    }
}
