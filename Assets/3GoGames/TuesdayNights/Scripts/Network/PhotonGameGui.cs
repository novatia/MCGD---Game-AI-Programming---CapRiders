#if PHOTON

using UnityEngine;
using System.Collections;

public class PhotonGameGui : MonoBehaviour 
{
    public Rect WindowRect = new Rect(300, 20, 120, 60);
    public int WindowId = 200;

    public bool Visible = false;

    // MonoBehaviour's INTERFACE

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            this.Visible = !this.Visible;
        }
    }

    void OnGUI()
    {
        if (!this.Visible)
        {
            return;
        }

        if (!PhotonNetwork.offlineMode && !PhotonNetwork.inRoom)
        {
            return;
        }

        this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, this.WindowFunc, "Menu");
    }

    // INTERNALS

    private void WindowFunc(int i_WindowId)
    {
        if (GUILayout.Button("Leave Match"))
        {
            PhotonNetwork.LeaveRoom();
        }

        GUI.DragWindow();
    }
}

#endif // PHOTON