using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : AnimalBehavior
{
    new void Start()
    {
        // Call start method from base class
        base.Start();
        getFoxAnimator();

    }

    private void getFoxAnimator()
    {

        Transform foxPrefab = prefab.transform.Find("Fox");
        if (foxPrefab != null)
        {
            animator = foxPrefab.gameObject.GetComponent<Animator>();
        }

    }
}
