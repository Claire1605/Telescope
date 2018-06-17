using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomZone : MonoBehaviour
{
	public float defaultZoomScale;
	public float minZoomScale;
	public float priority;
	public Texture2D zoomHeatmap;

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
}
