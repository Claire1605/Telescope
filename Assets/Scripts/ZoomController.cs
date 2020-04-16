using System.Collections;
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
    public float overlayPanSpeed = 1.0f;
    public float overlayPanDistance = 20.0f;
    public TelescopeCreak telescopeCreak;
    public TelescopeRingRotate telescopeRingRotate;

    private Vector3 panVelocity;
    private float zoomSize;
	private float initialZoomSize;
	private ZoomZone[] zoomZones;
    private Vector3 startPos;
    private int distanceFromStartPos = 500;
    
    private void Awake()
    {
        InputReference.GetPlayerID();
    }

    void Start ()
    {
        startPos = transform.position;
        
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

	void Update ()
    {
		FadeInMusic();

        UpdateView();

        UpdateOverlay();
    }

    public float GetZoomInput()
    {
        float zoomIn = InputReference.GetZoomIn() ? 1 : 0;
        float zoomOut = InputReference.GetZoomOut() ? -1 : 0;
        float zoomScroll = InputReference.GetZoomAxis() * scrollSpeed;

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            zoomIn *= 10;
            zoomOut *= 10;
        }
#endif

        float zoomInput = zoomScroll + zoomIn + zoomOut;

        return zoomInput;
    }

    void UpdateView()
    {
        //zoom
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

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            scaledPanSpeed *= 10;
        }
#endif

        Camera.main.transform.position += new Vector3(InputReference.GetHorizontalAxis(), InputReference.GetVerticalAxis()) * Time.deltaTime * scaledPanSpeed;

        if (gameState == GameState.INTRO)
        {
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, startPos.x - distanceFromStartPos, startPos.x + distanceFromStartPos), Mathf.Clamp(Camera.main.transform.position.y, startPos.y - distanceFromStartPos, startPos.y + distanceFromStartPos), Camera.main.transform.position.z);
            if (zoomSize > maxZoomSize)
            {
                zoomSize = Mathf.Lerp(zoomSize, maxZoomSize, Time.deltaTime);
            }
        }
    }

    void UpdateOverlay()
    {
        bool firstZoomBegan = InputReference.GetFirstZoomBegan();
        bool firstZoomEnded = InputReference.GetFirstZoomEnded();
        float zoomInput = GetZoomInput();

        if (firstZoomBegan && zoomInput == 1)
        {
            telescopeOverlayAnimator.ResetTrigger("zoomOutStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomEnded");
            telescopeOverlayAnimator.SetTrigger("zoomInStarted");
            InputReference.VibrateZoomIn();
            telescopeCreak.zoomInCreak();
        }
        else if (firstZoomBegan && zoomInput == -1)
        {
            telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomEnded");
            telescopeOverlayAnimator.SetTrigger("zoomOutStarted");
            InputReference.VibrateZoomOut();
            telescopeCreak.zoomOutCreak();
        }
        else if (firstZoomEnded)
        {
            telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomOutStarted");
            telescopeOverlayAnimator.SetTrigger("zoomEnded");
            telescopeCreak.returnCreak();
        }

        telescopeRingRotate.Rotate(zoomInput * Time.deltaTime);

        Vector2 panAmount = new Vector2(InputReference.GetHorizontalAxis(), InputReference.GetVerticalAxis());
        
        telescopeOverlayAnimator.transform.localPosition = Vector2.Lerp(telescopeOverlayAnimator.transform.localPosition, panAmount * overlayPanDistance, Time.deltaTime * overlayPanSpeed);
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
