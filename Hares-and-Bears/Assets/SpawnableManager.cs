using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject spawnablePrefab;
    [SerializeField] Camera arCam;
    
    private GameObject spawnedObject;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool onPlacement;

    [SerializeField] private UnityEvent onPlacementBegin, onPlacementEnd;




    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        onPlacement = false;
    }

    private void SpawnPrefab(Vector3 position)
    {
        spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);

    }

    public void PlacementUnlock()
    {
        onPlacement = true;
        onPlacementBegin.Invoke();
    }

    public void PlacementLock()
    {
        onPlacement = false;
        onPlacementEnd.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (!onPlacement)
            return;

        var screenCenter = arCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds))
        {
            if (spawnedObject == null)
            {
                SpawnPrefab(hits[0].pose.position);
            }

            spawnedObject.transform.position = hits[0].pose.position;
        }
        
    }
}
