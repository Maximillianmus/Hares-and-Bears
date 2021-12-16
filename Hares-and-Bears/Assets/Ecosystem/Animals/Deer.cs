using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : AnimalBehavior
{
    // Deer male mesh
    public Mesh maleModelMesh;

    new void Start() {

        // Call start method from base class
        base.Start();
        getDeerAnimator();
        checkGender();
    }

    private void getDeerAnimator() {

        Transform deerPrefab = prefab.transform.Find("Deer");
        if (deerPrefab != null)
        {
            animator = deerPrefab.gameObject.GetComponent<Animator>();
        }

    }

    // Check if male and change mesh to male mesh in that case
    private void checkGender() {

        Transform deerPrefab = prefab.transform.Find("Deer");

        if (male)
        {
            //print("is male");

            if (deerPrefab != null)
            {
                Transform deerModel = deerPrefab.gameObject.transform.Find("DeerModel");
                if (deerModel != null)
                {
                    SkinnedMeshRenderer smr = deerModel.GetComponent<SkinnedMeshRenderer>();
                    smr.sharedMesh = maleModelMesh;
                }
            }
        }
    }

}
