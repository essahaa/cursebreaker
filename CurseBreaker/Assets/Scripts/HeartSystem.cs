using System;
using UnityEngine;
using TMPro; // Make sure to include this namespace for TextMeshPro

public class HeartSystem : MonoBehaviour
{
    public int maxHearts = 5;
    private int currentHearts;
    private DateTime nextHeartTime;
    private TimeSpan heartRegenTime = TimeSpan.FromMinutes(0.3);

    public TextMeshProUGUI heartsText; // Public field to assign the TextMeshPro UI element
    public TextMeshProUGUI timerText; // Public field for the Timer TextMeshPro UI element

    private float lastHeartLossTime = 0f; // Field to store the last time a heart was lost
    private const float HeartLossCooldown = 2f; // Cooldown time in seconds


    void Start()
    {
        LoadHearts();
        UpdateHeartDisplay(); // Update display on start
    }

    void Update()
    {
        if (currentHearts < maxHearts)
        {
            UpdateTimerDisplay(); // Update the timer display every frame
            if (DateTime.Now > nextHeartTime)
            {
                GainHeart();
            }
        }
        else
        {
            ClearTimerDisplay(); // Clear the timer display when hearts are full
        }
    }

    public void LoseHeart()
    {
        // Check if enough time has passed since the last heart was lost
        if (Time.time - lastHeartLossTime < HeartLossCooldown)
        {
            return; // If not enough time has passed, just return
        }

        if (currentHearts > 0)
        {
            currentHearts--;
            SaveHearts();
            UpdateHeartDisplay(); // Update display when a heart is lost
        }

        if (currentHearts == maxHearts - 1)
        {
            nextHeartTime = DateTime.Now + heartRegenTime;
        }

        lastHeartLossTime = Time.time; // Update the lastHeartLossTime
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            TimeSpan timeRemaining = nextHeartTime - DateTime.Now;

            if (timeRemaining.Ticks < 0)
            {
                timerText.text = "00:00";
            }
            else
            {
                timerText.text = string.Format("{0:D2}:{1:D2}", timeRemaining.Minutes, timeRemaining.Seconds);
            }
        }
    }

    private void ClearTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = ""; // Or any message you prefer when hearts are full
        }
    }

    private void GainHeart()
    {
        currentHearts = Math.Min(currentHearts + 1, maxHearts);
        nextHeartTime = DateTime.Now + heartRegenTime;
        SaveHearts();
        UpdateHeartDisplay(); // Update display when a heart is gained
    }

    private void SaveHearts()
    {
        PlayerPrefs.SetInt("Hearts", currentHearts);
        PlayerPrefs.SetString("NextHeartTime", nextHeartTime.ToString());
    }

    private void LoadHearts()
    {
        currentHearts = PlayerPrefs.GetInt("Hearts", maxHearts);
        nextHeartTime = DateTime.Parse(PlayerPrefs.GetString("NextHeartTime", DateTime.Now.ToString()));
        UpdateHeartDisplay(); // Update display after loading the hearts
    }

    private void UpdateHeartDisplay()
    {
        if (heartsText != null)
        {
            heartsText.text = currentHearts.ToString(); // Update the TextMeshPro text
        }
    }

    public bool CanPlay()
    {
        if(currentHearts > 0){
            return true;
        }
        else
        {
            return false;
        }
        
    }

}
