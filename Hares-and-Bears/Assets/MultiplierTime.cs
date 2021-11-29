using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class MultiplierTime : MonoBehaviour
{
    [SerializeField] private TimeManager _timeManager;

    private void Update()
    {
        var mult = _timeManager.GetMultiplier().ToString(CultureInfo.InvariantCulture);
        if (mult.Length > 3)
        {
            mult = mult[..3];
        }

        gameObject.GetComponent<Text>().text = mult + "x";
    }
}
