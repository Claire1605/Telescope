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
    public Button continueButton;
    public Animator telescopeOverlayAnimator;

    void Start()
    {
        lensCapAnimation.SwitchLensCapState(LensCapState.CLOSED);
        OpenMenu(true);
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
    }

    IEnumerator waitForLensToClose()
    {
        canOpenMenu = false;

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

        if (InputReference.GetActiveController() == Rewired.ControllerType.Joystick)
        {
            continueButton.Select();
            continueButton.OnSelect(null);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (startOfGame)
            {
                continueButton.Select();
                continueButton.OnSelect(null);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        paused = false;
        lensCapAnimation.SwitchLensCapState(LensCapState.OPENING);

        if (InputReference.GetActiveController() == Rewired.ControllerType.Joystick)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        telescopeOverlayAnimator.ResetTrigger("zoomInStarted");
        telescopeOverlayAnimator.ResetTrigger("zoomEnded");
        telescopeOverlayAnimator.SetTrigger("zoomOutStarted");
    }
}
