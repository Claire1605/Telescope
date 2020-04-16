﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {INTRO, MAIN}
public class ZoomController : MonoBehaviour
{
    public GameState gameState = GameState.INTRO;
    public float zoomSpeed;
    public float scrollSpeed;
    public float panSpeed;
    public float maxZoomSize;
    public Transform fadePosition;
    public float minFadeSize;
    public float maxFadeSize;
    public float fadeDistance = 1.0f;
	public AnimationCurve musicFadeCurve;
    public Animator telescopeOverlayAnimator;
    public TelescopeCreak telescopeCreak;

    private Vector3 panVelocity;
    private float zoomSize;
	private float initialZoomSize;
	private ZoomZone[] zoomZones;
    private Vector3 startPos;
    private int distanceFromStartPos = 500;

    public UnityEngine.UI.Button zoomInButton;
    public UnityEngine.UI.Button zoomOutButton;
    public UnityEngine.UI.Button zoomNoneButton;

    UnityEngine.UI.ColorBlock activeButtonColours;
    UnityEngine.UI.ColorBlock inactiveButtonColours;

    private bool touchOverrideZoomOut = false;
    private bool touchOverrideZoomIn = false;

    public void SetTouchOverrideZoomOut()
    {
        if (InputReference.usingTouch)
        {
            touchOverrideZoomIn = false;
            touchOverrideZoomOut = true;

            SetButtonColours(-1);
        }
    }

    public void SetTouchOverrideZoomIn()
    {
        if (InputReference.usingTouch)
        {
            touchOverrideZoomIn = true;
            touchOverrideZoomOut = false;

            SetButtonColours(1);
        }
    }

    public void SetTouchOverrideZoomNone()
    {
        if (InputReference.usingTouch)
        {
            touchOverrideZoomIn = false;
            touchOverrideZoomOut = false;

            SetButtonColours(0);
        }
    }

    public void SetButtonColours(float zoomInput)
    {
        zoomOutButton.colors = zoomInput < 0.0f ? activeButtonColours : inactiveButtonColours;
        zoomInButton.colors = zoomInput > 0.0f ? activeButtonColours : inactiveButtonColours;
        zoomNoneButton.colors = zoomInput == 0.0f ? activeButtonColours : inactiveButtonColours;
    }

    private void Awake()
    {
        InputReference.GetPlayerID();
    }

    void Start ()
    {
        startPos = transform.position;

        activeButtonColours = zoomInButton.colors;
        activeButtonColours.normalColor = activeButtonColours.pressedColor;

        inactiveButtonColours = zoomInButton.colors;

        SetButtonColours(0);

        if (Camera.main.orthographic)
        {
            zoomSize = Camera.main.orthographicSize;
        }
        else
        {
            zoomSize = Camera.main.fieldOfView;
        }

		initialZoomSize = zoomSize;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

		zoomZones = FindObjectsOfType<ZoomZone>();
	}

    public float GetZoomInput()
    {
        float zoomIn = InputReference.GetZoomIn() ? 1 : 0;
        float zoomOut = InputReference.GetZoomOut() ? -1 : 0;
        float zoomScroll = InputReference.GetZoomAxis() * scrollSpeed;

        // If you are using an android device override touches
        if (InputReference.usingTouch)
        {
            if (touchOverrideZoomIn && Input.touchCount == 1)
            {
                zoomIn = 1;
                zoomOut = 0;
            }

            if (touchOverrideZoomOut && Input.touchCount == 1)
            {
                zoomIn = 0;
                zoomOut = -1;
            }

            if (touchOverrideZoomIn == false && touchOverrideZoomOut == false)
            {
                zoomIn = 0;
                zoomOut = 0;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            zoomIn *= 10;
            zoomOut *= 10;
        }
#endif

        float zoomInput = zoomScroll + zoomIn + zoomOut;
        
        // Update button colours on non touch devices
        if (InputReference.usingTouch == false)
        {
            SetButtonColours(zoomInput);
        }

        return zoomInput;
    }

	void Update ()
    {
        //FadeInOverlay();
		FadeInMusic();

        //zoomy
        float scaledZoomSpeed = zoomSpeed * zoomSize;

        float zoomAmount = GetZoomInput() * scaledZoomSpeed * Time.deltaTime;

        zoomSize += zoomAmount;

		CheckZoomZones();

        if (Camera.main.orthographic)
        {
			Camera.main.orthographicSize = zoomSize;
			Camera.main.transform.localScale = Vector3.one * zoomSize;
        }
        else
        {
            Camera.main.fieldOfView = zoomSize;
        }

        //pan
        float scaledPanSpeed = panSpeed * zoomSize;
        Camera.main.transform.position += new Vector3(InputReference.GetHorizontalAxis(), InputReference.GetVerticalAxis()) * Time.deltaTime * scaledPanSpeed;

        if (gameState == GameState.INTRO)
        {
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, startPos.x - distanceFromStartPos, startPos.x + distanceFromStartPos), Mathf.Clamp(Camera.main.transform.position.y, startPos.y - distanceFromStartPos, startPos.y + distanceFromStartPos), Camera.main.transform.position.z);
            if (zoomSize > maxZoomSize)
            {
                zoomSize = Mathf.Lerp(zoomSize, maxZoomSize, Time.deltaTime);
            }
        }

        Debug.Log("GetFirstTouchBegan: " + InputReference.GetFirstZoomBegan());
        Debug.Log("GetZoomInput: " + GetZoomInput());

        if (InputReference.GetFirstZoomBegan() && GetZoomInput() == 1) // zooming in, first touch
        {
            telescopeOverlayAnimator.ResetTrigger("zoomOutStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomEnded");
            telescopeOverlayAnimator.SetTrigger("zoomInStarted");
            InputReference.VibrateZoomIn();
            telescopeCreak.zoomInCreak();
        }
        else if (InputReference.GetFirstZoomBegan() && GetZoomInput() == -1) // zooming out, first touch
        {
            telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomEnded");
            telescopeOverlayAnimator.SetTrigger("zoomOutStarted");
            InputReference.VibrateZoomOut();
            telescopeCreak.zoomOutCreak();
        }
        else if (InputReference.GetFirstZoomEnded() && (InputReference.usingTouch == false || (touchOverrideZoomIn || touchOverrideZoomOut))) // end touch
        {
            telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomOutStarted");
            telescopeOverlayAnimator.SetTrigger("zoomEnded");
            telescopeCreak.returnCreak();
        }
    }

	void FadeInMusic()
	{
		if (gameState == GameState.INTRO)
		{
			GetComponent<AudioSource>().volume = Mathf.Lerp(0.0f, 1f, musicFadeCurve.Evaluate(1.0f - Mathf.Clamp01(zoomSize / (initialZoomSize / 2))));
		}
		else
		{
			GetComponent<AudioSource>().volume = Mathf.Lerp(GetComponent<AudioSource>().volume, 1f, Time.deltaTime);
		}
	}

    //void FadeInOverlay()
    //{
    //    if (gameState == GameState.INTRO)
    //    {
    //        Vector3 distance = transform.position - fadePosition.position;
    //        distance.z = 0.0f;

    //        if (distance.magnitude < fadeDistance)
    //        {
    //            GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().intensity = Mathf.Lerp(GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().intensity, Mathf.Lerp(0.0f, 1.0f, 1.0f - Mathf.Clamp01((zoomSize - minFadeSize) / (maxFadeSize / minFadeSize))), Time.deltaTime * 2f);
    //        }
    //        else
    //        {
    //            GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().intensity = Mathf.Lerp(GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().intensity, 0.0f, Time.deltaTime * 10);
    //        }
    //    }
    //    else
    //    {
    //        GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().intensity = Mathf.Lerp(GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().intensity, 1.0f, Time.deltaTime * 1.5f);
    //    }
    //}

	public void CheckZoomZones()
	{
		ZoomZone highestPriorityZone = null;

		foreach (ZoomZone zoomZone in zoomZones)
		{
			if (zoomZone)
			{
				// Check if the zoom zone is linked and should jump
				float zoomJump = zoomZone.GetLinkedZoomJump(Camera.main.GetComponent<Collider>().bounds);

                if (zoomJump != 0.0f)
                {
                    if (gameState == GameState.INTRO)
                    {
                        gameState = GameState.MAIN;
                    }

                    ZoomZone linkedZoomZone = zoomZone.GetLinkedZoomZone();

                    Vector3 newCameraPosition = linkedZoomZone.transform.position + zoomZone.GetZoomZoneOffset(Camera.main.transform.position) * zoomJump;
                    Camera.main.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, Camera.main.transform.position.z);
                    zoomSize = zoomSize * zoomJump;

                    zoomZone.UnRecordZoomZone(linkedZoomZone);
                    linkedZoomZone.RecordZoomZone(zoomZone);
                }

				if (zoomZone.InsideZoomZone(Camera.main.transform.position) && (highestPriorityZone == null || zoomZone.priority > highestPriorityZone.priority))
				{
					highestPriorityZone = zoomZone;
				}
			}
		}
		
		if (highestPriorityZone)
		{
			float minZoomSize = highestPriorityZone.GetMinZoomAtPoint(Camera.main.transform.position);

			if (minZoomSize > zoomSize)
			{
				zoomSize = Mathf.Lerp(zoomSize, minZoomSize, Time.deltaTime);
            }
        }
	}

  //  public void AcceleratedMovement(float panAcceleration, float panDecceleration, float panMaxSpeed)
  //  {
		////pan
		// if ((Input.GetAxis("Horizontal") > 0 && panSpeed.x < 0) || (Input.GetAxis("Horizontal") < 0 && panSpeed.x > 0))
		//{
		//	panSpeed.x = 0;
		//}

		//if ((Input.GetAxis("Vertical") > 0 && panSpeed.y < 0) || (Input.GetAxis("Vertical") < 0 && panSpeed.y > 0))
		//{
		//	panSpeed.y = 0;
		//}

  //      panVelocity += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * panAcceleration * Time.deltaTime;
  //      panVelocity = Vector3.ClampMagnitude(panVelocity, panMaxSpeed);

  //      if (Input.GetAxis("Horizontal") == 0)
  //      {
  //          if (panVelocity.x > 0)
  //          {
  //              panVelocity.x -= panDecceleration * Time.deltaTime;

  //              if (panVelocity.x < 0)
  //              {
  //                  panVelocity.x = 0;
  //              }
  //          }

  //          if (panVelocity.x < 0)
  //          {
  //              panVelocity.x += panDecceleration * Time.deltaTime;

  //              if (panVelocity.x > 0)
  //              {
  //                  panVelocity.x = 0;
  //              }
  //          }
  //      }

  //      if (Input.GetAxis("Vertical") == 0)
  //      {
  //          if (panVelocity.y > 0)
  //          {
  //              panVelocity.y -= panDecceleration * Time.deltaTime;

  //              if (panVelocity.y < 0)
  //              {
  //                  panVelocity.y = 0;
  //              }
  //          }

  //          if (panVelocity.y < 0)
  //          {
  //              panVelocity.y += panDecceleration * Time.deltaTime;

  //              if (panVelocity.y > 0)
  //              {
  //                  panVelocity.y = 0;
  //              }
  //          }
  //      }

  //      Vector3 scaledPanSpeed = panVelocity * zoomSize;
  //      Camera.main.transform.position += scaledPanSpeed * Time.deltaTime;
  //  }
}
