using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TelescopeOverlayBorder : MonoBehaviour
{
    public RectTransform topBorder;
    public RectTransform bottomBorder;
    public RectTransform leftBorder;
    public RectTransform rightBorder;

    private RectTransform thisRect;

    private float currentScreenWidth;
    private float currentScreenHeight;
    
    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        topBorder.gameObject.SetActive(true);
        bottomBorder.gameObject.SetActive(true);
        leftBorder.gameObject.SetActive(true);
        rightBorder.gameObject.SetActive(true);
        FillBorders();
    }

    private void Update()
    {
        if (currentScreenWidth != Screen.width || currentScreenHeight != Screen.height)
        {
            FillBorders();
        }
    }

    void FillBorders()
    {
        Vector3[] v = new Vector3[4];
        thisRect.GetWorldCorners(v);

        float x = v[2].x;
        float y = v[2].y;

        float border = x < y ? x / 2 : y / 2;

        topBorder.anchoredPosition = new Vector3(0, border, 0);
        bottomBorder.anchoredPosition = new Vector3(0, -border, 0);
        leftBorder.anchoredPosition = new Vector3(-border, 0, 0);
        rightBorder.anchoredPosition = new Vector3(border, 0, 0);

        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;
    }
}
