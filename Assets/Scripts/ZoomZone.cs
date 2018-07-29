using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Edge { TOP, RIGHT, BOTTOM, LEFT }

public class ZoomZone : MonoBehaviour
{
	public float defaultZoomScale;
	public float minZoomScale;
    public float priority;
	public Texture2D zoomHeatmap;
	public ZoomZone linkedZoomZone;
    public bool linkedToEvent;
    private bool invokedEventZoomIn;
    public UnityEvent linkedZoomEventZoomIn;
    private bool invokedEventZoomOut;
    public UnityEvent linkedZoomEventZoomOut;
    public int linkedZoomDirection;
    public bool recordZone = true;
    public List<ZoomZone> recordedZoomZones;

    private void Start()
    {
        recordedZoomZones = new List<ZoomZone>();
    }

    public ZoomZone GetLinkedZoomZone()
    {
        ZoomZone lastRecordedZoomZone = linkedZoomZone;

        if (linkedZoomZone)
        {
            // If any zoom zones have been recorded use the last one and clear it from the record
            if (recordedZoomZones.Count > 0)
            {
                lastRecordedZoomZone = recordedZoomZones[recordedZoomZones.Count - 1];
            }
        }

        return lastRecordedZoomZone;
    }

    public void RecordZoomZone(ZoomZone zoomZone)
    {
        // If a zoom zone has already been recorded or the zoom zone to be recorded is not the linked one
        if (zoomZone.recordZone && (recordedZoomZones.Count > 0 || zoomZone != linkedZoomZone))
        {
            if (recordedZoomZones.Count == 0 || recordedZoomZones[recordedZoomZones.Count - 1] != zoomZone)
            {
                recordedZoomZones.Add(zoomZone);
            }
        }
    }

    public void UnRecordZoomZone(ZoomZone zoomZone)
    {
        if (recordedZoomZones.Count > 0 && recordedZoomZones[recordedZoomZones.Count - 1] == zoomZone)
        {
            recordedZoomZones.RemoveAt(recordedZoomZones.Count - 1);
        }
    }
    
    public float GetLinkedZoomJump(Bounds viewBounds)
	{
		float zoomJump = 0.0f;

        ZoomZone lastRecordedZoomZone = GetLinkedZoomZone();

		if (lastRecordedZoomZone || linkedToEvent)
		{
			Bounds zoneBounds = GetComponent<BoxCollider>().bounds;

            if (InsideZoomZone(viewBounds.center))
            {
                if (linkedZoomDirection == 1)
                {
                    if (zoneBounds.Contains(viewBounds.max) == true && zoneBounds.Contains(viewBounds.min) == true)
                    {
                        if (linkedToEvent && invokedEventZoomIn == false)
                        {
                            linkedZoomEventZoomIn.Invoke();
                            invokedEventZoomIn = true;
                            invokedEventZoomOut = false;
                        }

                        if (lastRecordedZoomZone)
                        {
                            zoomJump = lastRecordedZoomZone.transform.localScale.magnitude / transform.localScale.magnitude;
                        }
                    }
                }
                else if (linkedZoomDirection == -1)
                {
                    if (zoneBounds.Contains(viewBounds.center + Vector3.up * viewBounds.extents.y) == false ||
                        zoneBounds.Contains(viewBounds.center + Vector3.right * viewBounds.extents.x) == false ||
                        zoneBounds.Contains(viewBounds.center + Vector3.down * viewBounds.extents.y) == false ||
                        zoneBounds.Contains(viewBounds.center + Vector3.left * viewBounds.extents.x) == false)
                    {
                        if (linkedToEvent && invokedEventZoomOut == false)
                        {
                            linkedZoomEventZoomOut.Invoke();
                            invokedEventZoomOut = true;
                            invokedEventZoomIn = false;
                        }

                        if (lastRecordedZoomZone)
                        {
                            zoomJump = lastRecordedZoomZone.transform.localScale.magnitude / transform.localScale.magnitude;
                        }
                    }
                }
                else //linked event only
                {
                    if (zoneBounds.Contains(viewBounds.max) == true && zoneBounds.Contains(viewBounds.min) == true) //zooming in
                    {
                        if (linkedToEvent && invokedEventZoomIn == false)
                        {
                            linkedZoomEventZoomIn.Invoke();
                            invokedEventZoomIn = true;
                            invokedEventZoomOut = false;
                        }
                    }
                    if (zoneBounds.Contains(viewBounds.center + Vector3.up * viewBounds.extents.y) == false ||
                    zoneBounds.Contains(viewBounds.center + Vector3.right * viewBounds.extents.x) == false ||
                    zoneBounds.Contains(viewBounds.center + Vector3.down * viewBounds.extents.y) == false ||
                    zoneBounds.Contains(viewBounds.center + Vector3.left * viewBounds.extents.x) == false) //zooming out
                    {
                        if (linkedToEvent && invokedEventZoomOut == false)
                        {
                            linkedZoomEventZoomOut.Invoke();
                            invokedEventZoomOut = true;
                            invokedEventZoomIn = false;
                        }
                    }
                }
            }  
		}

		return zoomJump;
	}

    public Vector3 GetZoomZoneOffset(Vector3 point)
    {
        return point - transform.position;
    }

    // Find the minimum zoom at a given point in the world
    public float GetMinZoomAtPoint(Vector3 point)
	{
		float minZoomAtPoint = 0.0f;
		
		Bounds zoneBounds = GetComponent<BoxCollider>().bounds;

		// Align point with zone in the z axis
		point.z = zoneBounds.center.z;

		if (zoneBounds.Contains(point))
		{
			Vector3 zoneOffset = point - zoneBounds.min;
			Vector3 zoneCoordinates = new Vector3 (zoneOffset.x / zoneBounds.size.x, zoneOffset.y / zoneBounds.size.y);

			// Use the heatmap to determine the minimum zoom
			if (zoomHeatmap)
			{
				minZoomAtPoint = Mathf.Lerp(minZoomScale, defaultZoomScale, 1.0f - zoomHeatmap.GetPixelBilinear(zoneCoordinates.x, zoneCoordinates.y).a);
			}
			// Otherwise use the distance from the center of the zone
			else
			{
				minZoomAtPoint = Mathf.Lerp(minZoomScale, defaultZoomScale, Vector3.Distance(point, zoneBounds.center) / zoneBounds.extents.magnitude);
			}
		}

		return minZoomAtPoint;
	}
    
    public bool InsideZoomZone(Vector3 point)
    {
        Bounds zoneBounds = GetComponent<BoxCollider>().bounds;

        // Align point with zone in the z axis
        point.z = zoneBounds.center.z;
        return zoneBounds.Contains(point);
    }

    public float ZoomZoneRatio()
    {
        float ratio = 0;
        float cameraSize = 0;
        float zoneSize = 0;

        cameraSize = Vector3.Distance(Camera.main.GetComponent<BoxCollider>().bounds.max, Camera.main.GetComponent<BoxCollider>().bounds.center);
        zoneSize = Vector3.Distance(GetComponent<BoxCollider>().bounds.max, GetComponent<BoxCollider>().bounds.center);

        ratio = cameraSize / zoneSize;
        return ratio;
    }
}
