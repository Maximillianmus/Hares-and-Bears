using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ManagerMap : MonoBehaviour
{

    public void OnPlacementBegin()
    {
        gameObject.GetComponent<ARPlaneManager>().enabled = false;
    }
    
    
    
}
