using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Animal
{
    // Deer male mesh
    public Mesh maleModelMesh;

    new void Start()
    {
        // Call start method from base class
        base.Start();

        checkGender();
    }

    // Check if male and change mesh to male mesh in that case
    private void checkGender()
    {
        if (male)
        {
            //print("is male");

            Transform deerPrefab = prefab.transform.Find("Deer");
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
