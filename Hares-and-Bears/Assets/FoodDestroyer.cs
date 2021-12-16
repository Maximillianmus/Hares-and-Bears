using System;
using System.Collections;
using System.Collections.Generic;
using Ecosystem;
using UnityEngine;

public class FoodDestroyer : MonoBehaviour, Eatable
{
    private HandFoodSpawner _handFoodSpawner;
    // Start is called before the first frame update
    void Start()
    {
        _handFoodSpawner = GameObject.FindGameObjectWithTag("FoodSpawner").GetComponent<HandFoodSpawner>();

    }

    private void OnDestroy()
    {
        _handFoodSpawner.EatFood();

    }

    public void Eat()
    {
        Handheld.Vibrate();
        Destroy(this.gameObject);
        
    }
}
