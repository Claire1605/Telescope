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
	}
	
	void Update ()
    {
        //zoomy
        float scaledZoomSpeed = zoomSpeed * zoomSize;

        float zoomIn = (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z)) ? 0 : -1;
        float zoomOut = (Input.GetMouseButton(1) || Input.GetKey(KeyCode.X)) ? 0 : 1;
        float zoomScroll = Input.GetAxis("Zoom") * scrollSpeed;

        float zoomInput = zoomScroll + zoomIn + zoomOut;
        float zoomAmount = zoomInput * scaledZoomSpeed * Time.deltaTime;
        zoomSize += zoomAmount;

		CheckZoomZones();

        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = zoomSize;
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
		RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward);

		ZoomZone highestPriorityZone = null;

		foreach (RaycastHit hit in hits)
		{
			ZoomZone hitZoomZone = hit.collider.GetComponent<ZoomZone>();

			if (hitZoomZone)
			{
				// Check if the zoom zone is linked and should jump
				zoomSize = zoomSize * hitZoomZone.GetLinkedZoomJump(new Bounds(transform.position, new Vector3(zoomSize, zoomSize / Camera.main.aspect)));

				if (highestPriorityZone == null || hitZoomZone.priority > highestPriorityZone.priority)
				{
					highestPriorityZone = hitZoomZone;
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
