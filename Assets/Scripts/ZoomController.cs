using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum GameState {INTRO, MAIN, MENU}
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
    public Menu menu;
    public Vector3 afterIntroPosition;
    public AudioMixer mixer;
    public float musicFadeSpeed = 0.2f;

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
        if (SaveManager.Load_PassedIntro() && !DebugStatic.AreWeTestingIntro())
        {
            transform.position = afterIntroPosition;
            gameState = GameState.MAIN;
        }

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

		zoomZones = FindObjectsOfType<ZoomZone>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update ()
    {
		FadeInMusic();

        if (!menu.paused)
        {
            UpdateView();
        }

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

            if (!menu.paused)
            {
                telescopeCreak.zoomInCreak();
            }
        }
        else if (firstZoomBegan && zoomInput == -1)
        {
            telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomEnded");
            telescopeOverlayAnimator.SetTrigger("zoomOutStarted");
            InputReference.VibrateZoomOut();
            if (!menu.paused)
            {
                telescopeCreak.zoomOutCreak();
            }
        }
        else if (firstZoomEnded)
        {
            telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
            telescopeOverlayAnimator.ResetTrigger("zoomOutStarted");
            telescopeOverlayAnimator.SetTrigger("zoomEnded");
            if (!menu.paused)
            {
                telescopeCreak.returnCreak();
            }
        }

        telescopeRingRotate.Rotate(zoomInput * Time.deltaTime);

        Vector2 panAmount = new Vector2(InputReference.GetHorizontalAxis(), InputReference.GetVerticalAxis());
        
        telescopeOverlayAnimator.transform.localPosition = Vector2.Lerp(telescopeOverlayAnimator.transform.localPosition, panAmount * overlayPanDistance, Time.deltaTime * overlayPanSpeed);
    }

	void FadeInMusic()
    {
        float musicVolume = GetMusicMixerVolume();

        if (gameState == GameState.INTRO)
		{
            float maxVolume = musicFadeCurve.Evaluate(1.0f - Mathf.Clamp01(zoomSize / (initialZoomSize / 2)));

            SetMixerVolume(Mathf.Min(musicVolume + Time.deltaTime * musicFadeSpeed, maxVolume));
		}
        else if (gameState == GameState.MENU)
        {
            SetMixerVolume(musicVolume - Time.deltaTime * musicFadeSpeed);
        }
		else
        {
            SetMixerVolume(musicVolume + Time.deltaTime * musicFadeSpeed);
        }
	}

    private string MIXER_VOLUME = "MusicVolume";

    float GetMusicMixerVolume()
    {
        float mixerVolume;
        mixer.GetFloat(MIXER_VOLUME, out mixerVolume);
        mixerVolume = Mathf.Pow(10, mixerVolume / 20);
        return mixerVolume;
    }

    void SetMixerVolume(float targetVolume)
    {
        targetVolume = Mathf.Clamp(targetVolume, 0.0001f, 1.0f);
        mixer.SetFloat(MIXER_VOLUME, Mathf.Log10(targetVolume) * 20.0f);
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
                        SaveManager.Save_PassedIntro();
                    }

                    ZoomZone linkedZoomZone = zoomZone.GetLinkedZoomZone();

                    Vector3 newCameraPosition = linkedZoomZone.transform.position + zoomZone.GetZoomZoneOffset(Camera.main.transform.position) * zoomJump;

                    Debug.Log("Zoom Offset: " + zoomZone.GetZoomZoneOffset(Camera.main.transform.position), zoomZone);
                    Debug.Log("Zoom Jump: " + zoomJump, zoomZone);

                    Camera.main.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, Camera.main.transform.position.z);
                    zoomSize = zoomSize * zoomJump;

                    zoomZone.UnRecordZoomZone(linkedZoomZone);
                    linkedZoomZone.RecordZoomZone(zoomZone);

                    //Ambient Audio
                    if (AmbientAudio.currentTrack != linkedZoomZone.ambientSound)
                    {
                        if (AmbientAudio.isFading)
                        {
                            StopCoroutine(AmbientAudio.GetFadeCoroutine());
                        }

                        AmbientAudio.SetFadeCoroutine(StartCoroutine(AmbientAudio.FadeAudio(linkedZoomZone.ambientSound)));
                    }

                    //Music
                    if (Music.currentTrack != linkedZoomZone.musicTrack)
                    {
                        if (Music.isFading)
                        {
                            StopCoroutine(Music.GetFadeCoroutine());
                        }

                        Music.SetFadeCoroutine(StartCoroutine(Music.FadeAudio(linkedZoomZone.musicTrack)));
                    }
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

    public void IntroComplete()
    {
        SaveManager.Save_PassedIntro();
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
