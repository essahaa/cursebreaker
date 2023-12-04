using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsUtility
{
    public static void SetLevelStars(int levelNumber, int stars)
    {
        PlayerPrefs.SetInt("Level_" + levelNumber + "_Stars", stars);
        PlayerPrefs.Save();
    }

    public static int GetLevelStars(int levelNumber)
    {
        return PlayerPrefs.GetInt("Level_" + levelNumber + "_Stars", 0);
    }
}

