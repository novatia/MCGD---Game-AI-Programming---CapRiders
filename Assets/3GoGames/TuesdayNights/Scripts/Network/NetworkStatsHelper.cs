#if PHOTON

using UnityEngine;
using System.Collections;

public class NetworkStatsHelper : MonoBehaviour 
{
	void Update () 
    {
        if (Input.GetButtonDown("ShowNetworkGUI"))
        {
            PhotonStatsGui statsGui = GameObject.FindObjectOfType<PhotonStatsGui>();
            if (statsGui == null)
            {
                GameObject go = new GameObject("PhotonStatsGUI");
                statsGui = go.AddComponent<PhotonStatsGui>();
            }
            else
            {
                statsGui.enabled = !statsGui.enabled;
            }
        }

        if (Input.GetButtonDown("ShowLagSimulation"))
        {
            PhotonLagSimulationGui lagSimulationGui = GameObject.FindObjectOfType<PhotonLagSimulationGui>();
            if (lagSimulationGui == null)
            {
                GameObject go = new GameObject("PhotonLagSimulationGUI");
                lagSimulationGui = go.AddComponent<PhotonLagSimulationGui>();
            }
            else
            {
                lagSimulationGui.enabled = !lagSimulationGui.enabled;
            }
        }
	}
}

#endif // PHOTON