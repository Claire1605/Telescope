using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class ZoomAnimation : MonoBehaviour
{
    public ZoomZone parentZoomZone;
    [Header("Ratio: Start and End")]
    public float ratio = 1;
    public float startRatio = 1;
    public float endRatio = 1;

    [Header("Position")]
    public bool animatePosition = false;
    public Vector2 positionZoomedOut;
    public Vector2 positionZoomedIn;
    public AnimationCurve positionCurveX = AnimationCurve.EaseInOut(1, 0, 0, 1);
    public AnimationCurve positionCurveY = AnimationCurve.EaseInOut(1, 0, 0, 1);
    private Vector3 initialPosition;

    [Header("Rotation")]
    public bool animateRotation = false;
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(1, 0, 0, 1);
    public float rotationZoomedOut = 0;
    public float rotationZoomedIn = 0;

    [Header("Scale")]
    public bool animateScale = false;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(1, 0, 0, 1);
    public Vector2 scaleZoomedOut = Vector2.one;
    public Vector2 scaleZoomedIn = Vector2.one;

    [Header("Colours")]
    public bool animateColour = false;
    public AnimationCurve colourCurve = AnimationCurve.EaseInOut(1, 0, 0, 1);
    public Color colourZoomedOut;
    public Color colourZoomedIn;

    private Material mat;
    private float t = 0;
    private ZoomController zoomController;

    public void Start()
    {
        mat = GetComponent<Renderer>().material;
        zoomController = FindObjectOfType<ZoomController>();
        initialPosition = transform.localPosition;
    }

    public void Update()
    {
        ratio = parentZoomZone.ZoomZoneRatio();

        if (!TimeManager.timeStop)
        {
            if (parentZoomZone) //null check
            {
                t = Mathf.Clamp01((ratio - endRatio) / (startRatio - endRatio));
                t = Mathf.Pow(1 - t, 2);
                if (animatePosition)
                {
                    float xPosition = Mathf.Lerp(positionZoomedIn.x, positionZoomedOut.x, positionCurveX.Evaluate(t));
                    float yPosition = Mathf.Lerp(positionZoomedIn.y, positionZoomedOut.y, positionCurveY.Evaluate(t));
                    transform.localPosition = initialPosition + new Vector3(xPosition, yPosition, 0.0f);
                }
                if (animateScale)
                {
                    transform.localScale = Vector3.Lerp(scaleZoomedIn, scaleZoomedOut, scaleCurve.Evaluate(t));
                }
                if (animateRotation)
                {
                    transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(rotationZoomedIn, rotationZoomedOut, rotationCurve.Evaluate(t)));
                }
                if (animateColour)
                {
                    mat.color = Color.Lerp(colourZoomedIn, colourZoomedOut, colourCurve.Evaluate(t));
                }
            }
            else
            {
                Debug.Log(gameObject.name + " " + transform.parent.name);
            }
        }
    }
}
