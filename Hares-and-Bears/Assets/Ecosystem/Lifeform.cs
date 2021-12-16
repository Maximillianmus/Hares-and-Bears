using System.Collections;
using System.Collections.Generic;
using Ecosystem;
using UnityEngine;

public abstract class Lifeform : MonoBehaviour, Eatable
{
    public Species species;
    
    
    public void Eat()
    {
        Destroy(this.gameObject);
    }
}
