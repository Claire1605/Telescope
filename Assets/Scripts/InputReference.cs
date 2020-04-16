using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public static class InputReference
{
    private static int playerId = 0;
    public static Player player; // The Rewired Player

    //REWIRED INPUTS
    public static string ZoomInButton = "ZoomInButton";
    public static string ZoomOutButton = "ZoomOutButton";
    public static string ZoomAxis = "ZoomAxis";
    public static string MoveHorizontal = "MoveHorizontal";
    public static string MoveVertical = "MoveVertical";

    public static void GetPlayerID()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    public static bool GetZoomIn()
    {
        bool pressed = false;
        
        if (player.GetButton(ZoomInButton))
        {
            pressed = true;
        }

        return pressed;
    }

    public static bool GetZoomOut()
    {
        bool pressed = false;
        
        if (player.GetButton(ZoomOutButton))
        {
            pressed = true;
        }

        return pressed;
    }

    public static float GetZoomAxis()
    {
        float i = 0;

        i = player.GetAxis(ZoomAxis);

        return i;
    }

    public static float GetHorizontalAxis()
    {
        float i = 0;
        
        i = player.GetAxis(MoveHorizontal);

        return i;
    }

    public static float GetVerticalAxis()
    {
        float i = 0;
        
        i = player.GetAxis(MoveVertical);

        return i;
    }

    public static bool GetFirstZoomBegan()
    {
        bool touchBegan = false;

        if (player.GetButtonDown(ZoomInButton))
        {
            touchBegan = true;
        }
        else if (player.GetButtonDown(ZoomOutButton))
        {
            touchBegan = true;
        }

        return touchBegan;
    }

    public static bool GetFirstZoomEnded()
    {
        bool touchEnded = false;

        if (player.GetButtonUp(ZoomInButton))
        {
            touchEnded = true;
        }
        else if (player.GetButtonUp(ZoomOutButton))
        {
            touchEnded = true;
        }

        return touchEnded;
    }

    public static void VibrateZoomIn()
    {
        player.SetVibration(1,0.15f,0.6f);
    }
    public static void VibrateZoomOut()
    {
        player.SetVibration(0, 0.15f, 0.6f);
    }
}
