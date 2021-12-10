using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpawnOnPress : MonoBehaviour
{
    [SerializeField] private GameObject animalPrefab;

    private bool activated = false;

    public Canvas generalCanvas;

    private float cooldown = 0.5f;
    private float lastTime = 0.0f;


    int UILayer;
 
    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
    }
 
    // https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
 
    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
 
 
    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
 
 
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.GetTouch(0).position;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    
    
    

    public void OnPlacementEnd()
    {
        activated = true;
        generalCanvas.gameObject.SetActive(true);
    }

    public void OnChangePrefab(GameObject newPrefab)
    {
        animalPrefab = newPrefab;
    }

    private bool Inside(RectTransform rect, Vector2 pos)
    {
        var rect1 = rect.rect;
        return pos.x >= rect1.x && pos.x <= rect1.x + rect1.width && pos.y >= rect1.y &&
               pos.y <= rect1.y + rect1.height;
    }

    
    void FixedUpdate()
    {
        if (Input.touchCount == 0 || Time.time - lastTime < cooldown)
        {
            return;
        }

        if (IsPointerOverUIElement())
            return;
        


        //create ray from camera to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hit;
        
        //only if ray hits plane and button was pressed
        if (Physics.Raycast(ray, out hit) && activated)
        {
            lastTime = Time.time;

            Instantiate(animalPrefab, hit.point, Quaternion.identity);

        }
        
    }

}
