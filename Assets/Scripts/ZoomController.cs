using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    public float zoomSpeed;
    public float scrollSpeed;
    public float panSpeed;
    private Vector3 panVelocity;
    private float zoomSize;
	private ZoomZone[] zoomZones;

	void Start ()
    {
        if (Camera.main.orthographic)
        {
            zoomSize = Camera.main.orthographicSize;
        }
        else
        {
            zoomSize = Camera.main.fieldOfView;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

		zoomZones = FindObjectsOfType<ZoomZone>();
	}

    public float GetZoomInput()
    {
        float zoomIn = (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z)) ? 0 : -1;
        float zoomOut = (Input.GetMouseButton(1) || Input.GetKey(KeyCode.X)) ? 0 : 1;
        float zoomScroll = Input.GetAxis("Zoom") * scrollSpeed;

        float zoomInput = zoomScroll + zoomIn + zoomOut;
        return zoomInput;
    }

	void Update ()
    {
        //zoomy
        float scaledZoomSpeed = zoomSpeed * zoomSize;   
        float zoomAmount = GetZoomInput() * scaledZoomSpeed * Time.deltaTime;
        zoomSize += zoomAmount;

		CheckZoomZones();

        if (Camera.main.orthographic)
        {
			Camera.main.orthographicSize = zoomSize;
			Camera.main.transform.localScale = Vector3.one * zoomSize;
        }
        else
        {
            Camera.main.fieldOfView = zoomSize;
        }

        //pan
        float scaledPanSpeed = panSpeed * zoomSize;
        Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * scaledPanSpeed;
	}

	public void CheckZoomZones()
	{
		ZoomZone highestPriorityZone = null;

		foreach (ZoomZone zoomZone in zoomZones)
		{
			if (zoomZone)
			{
				// Check if the zoom zone is linked and should jump
				float zoomJump = zoomZone.GetLinkedZoomJump(Camera.main.GetComponent<Collider>().bounds);

                if (zoomJump != 0.0f)
                {
                    ZoomZone linkedZoomZone = zoomZone.GetLinkedZoomZone();

                    Vector3 newCameraPosition = linkedZoomZone.transform.position + zoomZone.GetZoomZoneOffset(Camera.main.transform.position) * zoomJump;
                    Camera.main.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, Camera.main.transform.position.z);
                    zoomSize = zoomSize * zoomJump;

                    zoomZone.UnRecordZoomZone(linkedZoomZone);
                    linkedZoomZone.RecordZoomZone(zoomZone);
                }

				if (zoomZone.InsideZoomZone(Camera.main.transform.position) && (highestPriorityZone == null || zoomZone.priority > highestPriorityZone.priority))
				{
					highestPriorityZone = zoomZone;
				}
			}
		}

		if (highestPriorityZone)
		{
			float minZoomSize = highestPriorityZone.GetMinZoomAtPoint(Camera.main.transform.position);

			if (minZoomSize > zoomSize)
			{
				zoomSize = Mathf.Lerp(zoomSize, minZoomSize, Time.deltaTime);
			}
		}
	}

    public void AcceleratedMovement(float panAcceleration, float panDecceleration, float panMaxSpeed)
    {
		//pan
		/* if ((Input.GetAxis("Horizontal") > 0 && panSpeed.x < 0) || (Input.GetAxis("Horizontal") < 0 && panSpeed.x > 0))
		{
			panSpeed.x = 0;
		}

		if ((Input.GetAxis("Vertical") > 0 && panSpeed.y < 0) || (Input.GetAxis("Vertical") < 0 && panSpeed.y > 0))
		{
			panSpeed.y = 0;
		}*/

        panVelocity += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * panAcceleration * Time.deltaTime;
        panVelocity = Vector3.ClampMagnitude(panVelocity, panMaxSpeed);

        if (Input.GetAxis("Horizontal") == 0)
        {
            if (panVelocity.x > 0)
            {
                panVelocity.x -= panDecceleration * Time.deltaTime;

                if (panVelocity.x < 0)
                {
                    panVelocity.x = 0;
                }
            }

            if (panVelocity.x < 0)
            {
                panVelocity.x += panDecceleration * Time.deltaTime;

                if (panVelocity.x > 0)
                {
                    panVelocity.x = 0;
                }
            }
        }

        if (Input.GetAxis("Vertical") == 0)
        {
            if (panVelocity.y > 0)
            {
                panVelocity.y -= panDecceleration * Time.deltaTime;

                if (panVelocity.y < 0)
                {
                    panVelocity.y = 0;
                }
            }

            if (panVelocity.y < 0)
            {
                panVelocity.y += panDecceleration * Time.deltaTime;

                if (panVelocity.y > 0)
                {
                    panVelocity.y = 0;
                }
            }
        }

        Vector3 scaledPanSpeed = panVelocity * zoomSize;
        Camera.main.transform.position += scaledPanSpeed * Time.deltaTime;
    }
}
