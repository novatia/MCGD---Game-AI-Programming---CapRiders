using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour 
{
    private Camera cameraComponent = null;

    public Transform upperLeft = null;
    public Transform bottomRight = null;

    private float MinX = -10f;
    private float MaxX = 10f;
    private float MinY = -10f;
    private float MaxY = 10f;

    void Start()
    {
        cameraComponent = (Camera)GetComponent(typeof(Camera));

        if (upperLeft != null && bottomRight != null)
        {
            MinX = upperLeft.position.x;
            MaxX = bottomRight.position.x;

            MinY = bottomRight.position.y;
            MaxY = upperLeft.position.y;
        }
    }

    void LateUpdate()
    {
        if (cameraComponent == null)
        {
            return;
        }

        if (upperLeft != null && bottomRight != null)
        {
            Vector3 cameraPos = transform.position;

            float verticalSize = cameraComponent.orthographicSize * 2f;
            float horizontalSize = verticalSize * Screen.width / Screen.height;

            float minPossibleX = MinX + horizontalSize / 2f;
            float maxPossibleX = MaxX - horizontalSize / 2f;

            float minPossibleY = MinY + verticalSize / 2f;
            float maxPossibleY = MaxY - verticalSize / 2f;

            float x = Mathf.Clamp(cameraPos.x, minPossibleX, maxPossibleX);
            float y = Mathf.Clamp(cameraPos.y, minPossibleY, maxPossibleY);
			
            if (horizontalSize < MaxX - MinX)
            {
                // if camera can stay in bounds.

                cameraPos = new Vector3(x, cameraPos.y, cameraPos.z);
            }
			
			if (verticalSize < MaxY - MinY)
            {
                // if camera can stay in bounds.

                cameraPos = new Vector3(cameraPos.x, y, cameraPos.z);
            }
			
			transform.position = cameraPos;
        }
    }
}
