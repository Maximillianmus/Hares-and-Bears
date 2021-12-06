using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSelectionUI : MonoBehaviour
{
    public void OnSelectNew()
    {
        foreach (var selectedAnimal in GetComponentsInChildren<SelectedAnimalUI>())
        {
            selectedAnimal.OnUnselect();
        }
    }
}
