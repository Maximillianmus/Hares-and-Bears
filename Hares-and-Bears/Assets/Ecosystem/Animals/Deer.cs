using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Animal
{
    // Deer male mesh
    public Mesh maleModelMesh;
    public Animator animator;

    new void Start()
    {
        // Call start method from base class
        base.Start();
        checkGender();
    }

    new void Update()
    {
        base.Update();

    }

    // Check if male and change mesh to male mesh in that case
    private void checkGender()
    {
        Transform deerPrefab = prefab.transform.Find("Deer");
        if(deerPrefab != null) {
            animator = deerPrefab.gameObject.GetComponent<Animator>();
        }

        if (male)
        {
            print("is male");

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
