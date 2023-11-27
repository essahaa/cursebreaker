using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarDisplayUpdater : MonoBehaviour
{
    public GameObject[] yellowStars; // Array of YellowStar GameObjects

    void Awake()
{
    // Assuming each GreyStar has one YellowStar as a child
    yellowStars = new GameObject[transform.childCount];
    for (int i = 0; i < transform.childCount; i++)
    {
        yellowStars[i] = transform.GetChild(i).gameObject; // Assign each YellowStar
    }
}

    public void UpdateStars(int earnedStars)
    {
        for (int i = 0; i < yellowStars.Length; i++)
        {
            yellowStars[i].SetActive(i < earnedStars);
        }
    }
}