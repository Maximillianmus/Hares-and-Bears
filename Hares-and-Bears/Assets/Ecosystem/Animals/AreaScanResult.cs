using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScanResult
{
    public GameObject closestFood;
    public Vector3 closestWater;
    public bool waterClose;
    public AnimalBehavior closestMate;
    public List<GameObject> nearbyPredators;

    public AreaScanResult()
    {
        nearbyPredators = new List<GameObject>();
    }


}
