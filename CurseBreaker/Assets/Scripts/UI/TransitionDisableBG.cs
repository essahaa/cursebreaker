using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TransitionDisableBG : MonoBehaviour
{
    private GameObject canvas;
    private GameObject bgCanvas;
    public Animator animator;

    public void DisableCanvas() 
    {
        canvas = GameObject.Find("Canvas");
        //bgCanvas = GameObject.Find("BackgroundCanvas");
        canvas.SetActive(false);
        //bgCanvas.SetActive(false);


        GameObject transition = GameObject.Find("Transitions");
        animator = transition.GetComponent<Animator>();
        animator.SetTrigger("play");

        Invoke("ResetButtonClickedFlag", 2f);
    }

    private void ResetButtonClickedFlag()
    {
        canvas.SetActive(true);
    }
}
