using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Landscapes { ISLAND, CYBER, ORGANIC }
public static class SaveManager
{ 
    public static void Save_PassedIntro()
    {
        PlayerPrefs.SetInt("PassedIntro", 1);
    }

    public static void Save_LastLandscapeVisited(Landscapes landscape)
    {
        PlayerPrefs.SetInt("LastLandscapeVisited", (int)landscape);
    }

    public static bool Load_PassedIntro()
    {
        bool hasPassedIntro = false;
        if (PlayerPrefs.GetInt("PassedIntro") == 1)
        {
            hasPassedIntro = true;
        }
        return hasPassedIntro;
    }

    public static Landscapes Load_LastLandscapeVisited()
    {
        Landscapes landscape = Landscapes.ISLAND;

        if (PlayerPrefs.HasKey("LastLandscapeVisited"))
        {
            landscape = (Landscapes)PlayerPrefs.GetInt("LastLandscapeVisited");
        }
       
        return landscape;
    }

    public static void Save_NewGameRefresh()
    {
        PlayerPrefs.DeleteAll();
    }
}
