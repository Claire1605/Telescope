using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ZoomAnimation : MonoBehaviour {

    public Transform transformStart;
    public Transform transformEnd;
    public Color colourStart;
    public Color colourEnd;

    private Material mat;
    private float i;
    private ZoomController zoomController;

    public void Start()
    {
        mat = GetComponent<Renderer>().material;
        zoomController = FindObjectOfType<ZoomController>();
    }

    public void Update()
    {
        //transform.localPosition = Vector3.Lerp(transformStart.localPosition, transformEnd.localPosition, i);
       // transform.localScale = Vector3.Lerp(transformStart.localScale, transformEnd.localScale, i);
        mat.color = Color.Lerp(colourStart, colourEnd, i);
    }

}
