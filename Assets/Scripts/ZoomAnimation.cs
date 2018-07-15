using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ZoomAnimation : MonoBehaviour
{
    public ZoomZone parentZoomZone;

    [Header("Position")]
    public AnimationCurve positionCurveX = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve positionCurveY = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector2 positionZoomOut;
    public Vector2 positionZoomIn;
    private Vector3 initialPosition;

    [Header("Rotation")]
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float rotationZoomOut = 0;
    public float rotationZoomIn = 0;

    [Header("Scale")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector2 scaleZoomOut = Vector2.one;
    public Vector2 scaleZoomIn = Vector2.one;

    [Header("Colours")]
    public AnimationCurve colourCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Color colourZoomOut;
    public Color colourZoomIn;

    private Material mat;
    private float t;
    private ZoomController zoomController;

    public void Start()
    {
        mat = GetComponent<Renderer>().material;
        zoomController = FindObjectOfType<ZoomController>();
        initialPosition = transform.localPosition;
    }

    public void Update()
    {
        if (!TimeManager.timeStop)
        {
            t = Mathf.Clamp01(parentZoomZone.ZoomZoneRatio());

            float xPosition = Mathf.Lerp(positionZoomIn.x, positionZoomOut.x, positionCurveX.Evaluate(t));
            float yPosition = Mathf.Lerp(positionZoomIn.y, positionZoomOut.y, positionCurveY.Evaluate(t));
            transform.localPosition = initialPosition + new Vector3(xPosition, yPosition, 0.0f);
            transform.localScale = Vector3.Lerp(scaleZoomIn, scaleZoomOut, t);
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(rotationZoomIn, rotationZoomOut, rotationCurve.Evaluate(t)));
            mat.color = Color.Lerp(colourZoomIn, colourZoomOut, colourCurve.Evaluate(t));
        }
    }
}
