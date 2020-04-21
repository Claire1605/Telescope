using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public LensCapAnimation lensCapAnimation;
    public bool paused = false;
    public bool canOpenMenu = false;
    public GameObject menu;
    public GameObject cursor;
    public Button continueButton;
    public Animator telescopeOverlayAnimator;
    public GameObject continueObject;
    public Animator menuRotate;

    private bool grab;
    private bool canTurn = true;

    void Start()
    {
        if (SaveManager.Load_PassedIntro())
        {
            lensCapAnimation.SwitchLensCapState(LensCapState.CLOSED);
            OpenMenu(true);
        }
        else
        {
            lensCapAnimation.SwitchLensCapState(LensCapState.OPENING);
        }
    }

    void Update()
    {
        if (lensCapAnimation.lensCapState == LensCapState.OPEN)
        {
            canOpenMenu = true;
        }

        if (InputReference.GetOpenMenu() && canOpenMenu)
        {
            lensCapAnimation.SwitchLensCapState(LensCapState.CLOSING);
            StartCoroutine(waitForLensToClose());
        }

        if (paused)
        {
            if (InputReference.GetMenuHorizontalAxis() < 0 && canTurn)
            {
                canTurn = false;
                menuRotate.SetTrigger("turnClockwise");
            }
            else if (InputReference.GetMenuHorizontalAxis() > 0 && canTurn)
            {
                canTurn = false;
                menuRotate.SetTrigger("turnAnticlockwise");
            }
            else if (InputReference.GetMenuHorizontalAxis() == 0)
            {
                canTurn = true;
            }

            if (InputReference.GetZoomIn())
            {
                if (menuRotate.GetCurrentAnimatorStateInfo(0).IsName("MenuRotationContinue"))
                {
                    CloseMenu();
                }
                else if (menuRotate.GetCurrentAnimatorStateInfo(0).IsName("MenuRotationObservatory"))
                {
                    CloseMenu();
                } 
                else if (menuRotate.GetCurrentAnimatorStateInfo(0).IsName("MenuRotationQuit"))
                {
                    Application.Quit();
                }
            }
        }
    }

    public void LateUpdate()
    {
        if (grab)
        {
            int photoWidth = 400;
            int photoHeight = 400;
            RenderTexture rt = new RenderTexture(photoWidth, photoHeight, 24);
            Camera.main.targetTexture = rt;
            RenderTexture.active = rt;
            Camera.main.Render();
            Texture2D screenShot = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, photoWidth, photoHeight), 0, 0);
            Camera.main.targetTexture = null;
            screenShot.Apply();
            continueObject.GetComponent<Image>().sprite = Sprite.Create(screenShot, new Rect(0, 0, photoWidth, photoHeight), new Vector2(0, 0));

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            grab = false;
        }
    }

    IEnumerator waitForLensToClose()
    {
        canOpenMenu = false;

        grab = true;

        telescopeOverlayAnimator.ResetTrigger("zoomOutStarted");
        telescopeOverlayAnimator.ResetTrigger("zoomEnded");
        telescopeOverlayAnimator.SetTrigger("zoomInStarted");

        while (lensCapAnimation.lensCapState != LensCapState.CLOSED)
        {
            yield return null;
        }
        OpenMenu(false);
    }

    

    void OpenMenu(bool startOfGame)
    {
        paused = true;
        menu.SetActive(true);
        cursor.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //if (InputReference.GetActiveController() == Rewired.ControllerType.Joystick)
        //{
        //    continueButton.Select();
        //    continueButton.OnSelect(null);

        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else
        //{
        //    if (startOfGame)
        //    {
        //        continueButton.Select();
        //        continueButton.OnSelect(null);
        //    }

        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        cursor.SetActive(false);
        paused = false;
        lensCapAnimation.SwitchLensCapState(LensCapState.OPENING);

        //if (InputReference.GetActiveController() == Rewired.ControllerType.Joystick)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}

        telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
        telescopeOverlayAnimator.ResetTrigger("zoomEnded");
        telescopeOverlayAnimator.SetTrigger("zoomOutStarted");
    }
}
