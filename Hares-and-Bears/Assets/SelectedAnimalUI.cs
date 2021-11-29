using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectedAnimalUI : MonoBehaviour
{
    [SerializeField] private Image selectUI;
    [SerializeField] private GameObject animalPrefabLinked;
    [SerializeField] private UnityEvent<GameObject> onSelectEvent;
    [SerializeField] private UnityEvent unselectAllOther;

    private bool selected = false;
    public void OnSelect()
    {
        unselectAllOther.Invoke();
        selectUI.gameObject.SetActive(true);
        if (!selected)
        {
            selected = true;
            onSelectEvent.Invoke(animalPrefabLinked);
        }
    }

    public void OnUnselect()
    {
        selectUI.gameObject.SetActive(false);
        selected = false;
    }
}
