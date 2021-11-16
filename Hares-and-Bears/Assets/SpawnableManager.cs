using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] private GameObject spawnablePrefab;

    [SerializeField] Camera arCam;
    private bool mapSpawned = false;
    private bool locked = false;
    private GameObject spawnedObject;




    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
    }

    private void SpawnPrefab(Vector3 position)
    {
        spawnedObject = Instantiate(spawnablePrefab, position, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);
        if (raycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("Spawnable"))
                    {
                        spawnedObject = hit.collider.gameObject;
                    }
                    else if (!mapSpawned)
                    {
                        SpawnPrefab(hits[0].pose.position);
                        mapSpawned = true;
                    }
                }
            } else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null && !locked)
            {
                spawnedObject.transform.position = hits[0].pose.position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObject = null;
            }
        }
        
    }
}
