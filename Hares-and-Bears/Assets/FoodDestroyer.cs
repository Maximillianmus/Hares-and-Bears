using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDestroyer : MonoBehaviour
{
    private FoodSpawner FoodSpawner;
    // Start is called before the first frame update
    void Start()
    {
        FoodSpawner = GameObject.FindGameObjectWithTag("FoodSpawner").GetComponent<FoodSpawner>();

    }

    private void OnDestroy()
    {
        FoodSpawner.EatFood();

    }
}
