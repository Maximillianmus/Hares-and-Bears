using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hare : AnimalBehavior
{
    new void Start()
    {
        // Call start method from base class
        base.Start();
        getHareAnimator();
    }

    private void getHareAnimator()
    {

        Transform harePrefab = prefab.transform.Find("Hare");
        if (harePrefab != null)
        {
            animator = harePrefab.gameObject.GetComponent<Animator>();
        }

    }
}
