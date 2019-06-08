using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target = null;

	public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	public float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.

    public float smoothTime = 0.5f;

    private float xVelocity = 0f;
    private float yVelocity = 0f;

	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
        return Mathf.Abs(transform.position.x - target.position.x) > xMargin;
	}

	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(transform.position.y - target.position.y) > yMargin;
	}

	void LateUpdate()
	{
        if (target == null)
        {
            return;
        }

		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;

		// If the player has moved beyond the x margin...
        if (CheckXMargin())
        {
            // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
            targetX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref xVelocity, smoothTime);
        }

		// If the player has moved beyond the y margin...
        if (CheckYMargin())
        {
            // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
            targetY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref yVelocity, smoothTime);
        }

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}

    // BUSINESS LOGIC

    public void SetTarget(GameObject i_Target)
    {
        if (i_Target != null)
        {
            target = i_Target.transform;
        }
    }
}
