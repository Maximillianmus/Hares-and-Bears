using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ManagerMap : MonoBehaviour
{

    public void OnPlacementBegin()
    {
        ARPlaneManager planeManager = gameObject.GetComponent<ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
    }
    
    
    
}
