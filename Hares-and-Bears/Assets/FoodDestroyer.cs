using System;
using System.Collections;
using System.Collections.Generic;
using Ecosystem;
using UnityEngine;

public class FoodDestroyer : MonoBehaviour, Eatable
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

    public void Eat()
    {
        Handheld.Vibrate();
        Destroy(this.gameObject);
        
    }
}
