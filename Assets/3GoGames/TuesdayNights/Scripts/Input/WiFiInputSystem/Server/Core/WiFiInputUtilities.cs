using UnityEngine;

using System;

using WiFiInput.Common;

namespace WiFiInput.Server
{
    public static class WiFiInputUtilities
    {
        //this function will get the minimum point and the width and height and return it in a rect
        //NOTE: this is only a function for special case rectTransform (assumes anchormin = anchormax (point anchor))
        public static Rect GetScreenRect(RectTransform rectTransform)
        {
            RectTransform clientControlCanvas;
            clientControlCanvas = GameObject.Find("ClientControlCanvas").GetComponent<RectTransform>();

            float screenHeight = Screen.height;
            float screenWidth = Screen.width;

            float canvasWidth = clientControlCanvas.rect.width;
            float canvasHeight = clientControlCanvas.rect.height;

            //inspector value
            float inspectorX = rectTransform.anchoredPosition.x;
            float inspectorY = rectTransform.anchoredPosition.y;

            //anchor point
            float canvasAnchorPositionX = rectTransform.anchorMin.x * canvasWidth;
            float canvasAnchorPositionY = rectTransform.anchorMin.y * canvasHeight;

            //pivot delta
            float canvasPivotAdjustX = rectTransform.pivot.x * rectTransform.rect.width;
            float canvasPivotAdjustY = rectTransform.pivot.y * rectTransform.rect.height;

            //normalize the value
            float normalizedPositionX = (inspectorX - canvasPivotAdjustX + canvasAnchorPositionX) / canvasWidth;
            float normalizedPositionY = (inspectorY - canvasPivotAdjustY + canvasAnchorPositionY) / canvasHeight; 
          
            //adjust the height/width
            float heightFactor = screenHeight / canvasHeight;
            float widthFactor = screenWidth / canvasWidth;
            
            //we now have anchor point now adjust for our recttransform
            float screenPositionX = Mathf.Floor(normalizedPositionX * screenWidth);
            float screenPositionY = Mathf.Floor(normalizedPositionY * screenHeight);


            return new Rect(screenPositionX, screenPositionY, rectTransform.rect.width * heightFactor, rectTransform.rect.height * widthFactor);
        }

        //This method is used to calculate a vector3 from a 2 axis control
        public static Vector3 getControllerVector3(float horizontalMovement,float verticalMovement, AXIS horizontalAxis, AXIS verticalAxis)
        {
            Vector3 controllerVector;
            controllerVector.x = 0f; controllerVector.y = 0f; controllerVector.z = 0f;

            //if either axis are mapped to x or negative x
            if (horizontalAxis == AXIS.XAxis)
                controllerVector.x += horizontalMovement;
            if (verticalAxis == AXIS.XAxis)
                controllerVector.x += verticalMovement;
            if (horizontalAxis == AXIS.NegativeXAxis)
                controllerVector.x -= horizontalMovement;
            if (verticalAxis == AXIS.NegativeXAxis)
                controllerVector.x -= verticalMovement;

            //if either axis are mapped to y
            if (horizontalAxis == AXIS.YAxis)
                controllerVector.y += horizontalMovement;
            if (verticalAxis == AXIS.YAxis)
                controllerVector.y += verticalMovement;
            if (horizontalAxis == AXIS.NegativeYAxis)
                controllerVector.y -= horizontalMovement;
            if (verticalAxis == AXIS.NegativeYAxis)
                controllerVector.y -= verticalMovement;

            //if either axis are mapped to z
            if (horizontalAxis == AXIS.ZAxis)
                controllerVector.z += horizontalMovement;
            if (verticalAxis == AXIS.ZAxis)
                controllerVector.z += verticalMovement;
            if (horizontalAxis == AXIS.NegativeZAxis)
                controllerVector.z -= horizontalMovement;
            if (verticalAxis == AXIS.NegativeZAxis)
                controllerVector.z -= verticalMovement;

            //if either axis is none it isn't taken into account so no action needed


            //return the vector
            return controllerVector;
        }

        //This method looks through the data structure and scans for timedout connections
        public static void CheckForClientTimeout(float timeout)
        {
            BaseControllerType temp;
            bool foundDeadConnection = false;
            if (WiFiInputController.controllerDataDictionary != null)
            {
                for (int keyIndex = 0; keyIndex < WiFiInputController.controllerDataDictionaryKeys.Count; ++keyIndex)
                {
                    string key = WiFiInputController.controllerDataDictionaryKeys[keyIndex];
                    temp = WiFiInputController.controllerDataDictionary[key];
                    if (!temp.controllerType.Contains(WiFiInputConstants.BACKCHANNEL_FILTER) && temp.logicalPlayerNumber != WiFiInputConstants.PLAYERNUMBER_DISCONNECTED && (DateTime.UtcNow - temp.lastReceivedPacketTime).TotalSeconds > timeout && (DateTime.UtcNow - temp.lastReceivedPacketTime).TotalSeconds < 150d)
                    {
                        temp.previousConnectionPlayerNumber = temp.logicalPlayerNumber;
                        temp.logicalPlayerNumber = WiFiInputConstants.PLAYERNUMBER_DISCONNECTED;
                        foundDeadConnection = true;
                        WiFiInputController.lastConnectedPlayerNumber = temp.previousConnectionPlayerNumber;
                    }
                }
            }

            //if we found a dead connection let the callback know
            if (foundDeadConnection)
            {
                WiFiInputController.isConnect = false;
                WiFiInputController.forceConnectionRefresh();
            }
        }

        //This method looks through the data structure and returns it if found otherwise returns null
        public static T checkForClient<T>(string control, int player)
            where T : BaseControllerType
        {
            BaseControllerType lookupObject;

            if (WiFiInputController.controllerDataDictionary != null)
            {
                for (int keyIndex = 0; keyIndex < WiFiInputController.controllerDataDictionaryKeys.Count; ++keyIndex)
                {
                    string key = WiFiInputController.controllerDataDictionaryKeys[keyIndex];

                    //serverside the key will also have the IP of the client so we need to lookup additional one time
                    if (key.EndsWith(control))
                    {
                        lookupObject = WiFiInputController.controllerDataDictionary[key];
                        if (lookupObject.logicalPlayerNumber == (int)player || (int)player == WiFiInputConstants.PLAYERNUMBER_ANY)
                        {
                            return (T)lookupObject;
                        }
                    }
                }
            }

            return null;
        }

        //This method looks through the data structure and returns it if found otherwise returns null
        public static int getHighestPlayerNumber()
        {
            BaseControllerType control;
            int highestPlayer = 0;

            if (WiFiInputController.controllerDataDictionary != null)
            {
                foreach (string key in WiFiInputController.controllerDataDictionary.Keys)
                {
                    control = WiFiInputController.controllerDataDictionary[key];
                    //serverside the key will also have the IP of the client so we need to lookup additional one time
                    if (control.logicalPlayerNumber > highestPlayer)
                    {
                        highestPlayer = control.logicalPlayerNumber;
                    }
                }
            }
            return highestPlayer;
        }

        //This method looks through the data structure and returns it if found otherwise returns null
        public static bool isPlayerNumberOccupied(int playerNumber, string controlName)
        {
            foreach (string key in WiFiInputController.controllerDataDictionary.Keys)
            {
                //serverside the key will also have the IP of the client so we need to lookup additional one time
                if (key.Contains(controlName) && WiFiInputController.controllerDataDictionary[key].logicalPlayerNumber == playerNumber)
                {
                    return true;
                }
            }
            return false;
        }

        //This method looks through the data structure and returns it if found otherwise returns null
        public static void swapPlayerNumber(int oldPlayerNumber, int newPlayerNumber)
        {
            BaseControllerType control;

            if (WiFiInputController.controllerDataDictionary != null)
            {
                foreach (string key in WiFiInputController.controllerDataDictionary.Keys)
                {
                    control = WiFiInputController.controllerDataDictionary[key];
                    //serverside the key will also have the IP of the client so we need to lookup additional one time
                    if (control.logicalPlayerNumber == oldPlayerNumber)
                    {
                        control.logicalPlayerNumber = newPlayerNumber;
                    }
                }
            }
        }

        //Implementation limitation: the vectors cannot be parallel to n
        public static float relativeAngleInAxis(Vector3 firstVector, Vector3 secondVector, Vector3 axisVector)
        {
		    Vector3 tmp, a, b;

		    axisVector.Normalize();

		    tmp = Vector3.Cross(axisVector,firstVector); 
		    a = Vector3.Cross(tmp,axisVector);

		    tmp = Vector3.Cross(axisVector,secondVector); 
		    b = Vector3.Cross(tmp,axisVector);

		    tmp = Vector3.Cross(a,b);
		    float angleSignTmp = Vector3.Angle(tmp,axisVector) - 90;
		    float angleSign = Mathf.Sign(angleSignTmp);
		    return (Vector3.Angle(a,b) * angleSign);
	    }
    }
}