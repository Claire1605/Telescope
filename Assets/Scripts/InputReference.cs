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

    public static Vector2 initialTouchPosition = Vector2.zero;

    public static void GetPlayerID()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    public static bool GetZoomIn()
    {
        bool pressed = false;

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).tapCount == 1)
            {
                pressed = true;
            }
        }
        else
        {
            if (player.GetButton(ZoomInButton))
            {
                pressed = true;
            }
        }

        return pressed;
    }

    public static bool GetZoomOut()
    {
        bool pressed = false;

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).tapCount == 2)
            {
                pressed = true;
            }
        }
        else
        {
            if (player.GetButton(ZoomOutButton))
            {
                pressed = true;
            }
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

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
            }
            else
            {
                i = Mathf.Clamp((touch.position - initialTouchPosition).x / (Screen.width / 4), -1.0f, 1.0f);
            }
        }
        else
        {
            i = player.GetAxis(MoveHorizontal);
        }

        return i;
    }

    public static float GetVerticalAxis()
    {
        float i = 0;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
            }
            else
            {
                i = Mathf.Clamp((touch.position - initialTouchPosition).y / (Screen.width / 4), -1.0f, 1.0f);
            }
        }
        else
        {
            i = player.GetAxis(MoveVertical);
        }

        return i;
    }
}
