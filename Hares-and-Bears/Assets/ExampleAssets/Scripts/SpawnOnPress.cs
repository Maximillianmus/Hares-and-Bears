using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpawnOnPress : MonoBehaviour
{
    [SerializeField] private GameObject animalPrefab;

    private bool activated = false;

    public Canvas generalCanvas;
    public RectTransform panelTouch;

    private float cooldown = 0.5f;
    private float lastTime = 0.0f;
    
    

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
