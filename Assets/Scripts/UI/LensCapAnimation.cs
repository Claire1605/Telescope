using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LensCapState {CLOSED, OPENING, OPEN, CLOSING}
public class LensCapAnimation : MonoBehaviour
{
    public LensCapState lensCapState = LensCapState.CLOSED;
    public float speed = 1.0f;
    public float yLimit;

    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float i = 0;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        yLimit = Screen.height;
    }

    void Update()
    {
        if (lensCapState == LensCapState.OPENING)
        {
            i += Time.deltaTime * speed;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, i);

            if (i >= 1)
            {
                SwitchLensCapState(LensCapState.OPEN);
            }
        }
        else if (lensCapState == LensCapState.CLOSING)
        {
            i += Time.deltaTime * speed;
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, i);

            if (i >= 1)
            {
                SwitchLensCapState(LensCapState.CLOSED);
            }
        }
    }

    public void SwitchLensCapState(LensCapState state)
    {
        lensCapState = state;

        switch (lensCapState)
        {
            case LensCapState.CLOSED:
                {
                    rectTransform.anchoredPosition = new Vector3(0, 0, 0);
                    break;
                }
            case LensCapState.OPENING:
                {
                    i = 0;
                    startPosition = new Vector3(0, 0, 0);
                    endPosition = new Vector3(0, yLimit, 0);
                    break;
                }
            case LensCapState.OPEN:
                {
                    rectTransform.anchoredPosition = new Vector3(0, yLimit, 0);
                    break;
                }
            case LensCapState.CLOSING:
                {
                    i = 0;
                    startPosition = new Vector3(0, yLimit, 0);
                    endPosition = new Vector3(0, 0, 0);
                    break;
                }
        }
    }
}
