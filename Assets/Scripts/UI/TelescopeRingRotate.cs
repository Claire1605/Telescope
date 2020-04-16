using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelescopeRingRotate : MonoBehaviour
{
    public float speed = 1.0f;
    public RectTransform rectTransform;

    public void Rotate(float rotateAmount)
    {
        rectTransform.Rotate(Vector3.forward, rotateAmount * speed);
    }
}
