using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Animal
{
    new void Start() {

        // Call start method from base class
        base.Start();
        getBearAnimator();

    }

    private void getBearAnimator()
    {

        Transform bearPrefab = prefab.transform.Find("Bear");
        if (bearPrefab != null)
        {
            animator = bearPrefab.gameObject.GetComponent<Animator>();
        }

    }
}
