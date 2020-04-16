﻿using System.Collections;
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
        float zoom = 0;

        zoom = player.GetAxis(ZoomAxis);

        return zoom;
    }

    public static float GetHorizontalAxis()
    {
        float horizontal = 0;
        
        horizontal = player.GetAxis(MoveHorizontal);

        horizontal = Mathf.Clamp(horizontal, -1.0f, 1.0f);

        return horizontal;
    }

    public static float GetVerticalAxis()
    {
        float vertical = 0;
        
        vertical = player.GetAxis(MoveVertical);

        vertical = Mathf.Clamp(vertical, -1.0f, 1.0f);

        return vertical;
    }

    public static bool GetFirstZoomBegan()
    {
        bool zoomBegan = false;

        if (player.GetButtonDown(ZoomInButton))
        {
            zoomBegan = true;
        }
        else if (player.GetButtonDown(ZoomOutButton))
        {
            zoomBegan = true;
        }

        return zoomBegan;
    }

    public static bool GetFirstZoomEnded()
    {
        bool zoomEnded = false;

        if (player.GetButtonUp(ZoomInButton))
        {
            zoomEnded = true;
        }
        else if (player.GetButtonUp(ZoomOutButton))
        {
            zoomEnded = true;
        }

        return zoomEnded;
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
