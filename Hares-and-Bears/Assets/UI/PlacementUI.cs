using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour
{
    [SerializeField] private Button placeButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Canvas canvas;

    public void onPlacementBegin()
    {
        placeButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    public void onPlacementEnd()
    {
        Destroy(canvas);
    }
    
}
