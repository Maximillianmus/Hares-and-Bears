using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareAnimal : MonoBehaviour
{
    public float range = 1f;
    public float scaredDuration = 2f;
    public List<Animal> animals;

    public bool a = false;

    // Update is called once per frame
    void Update()
    {
        if(a)
        {
            a = false;
            scareAnimals();
        }
    }

    public void scareAnimals()
    {
        animals = new List<Animal>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Animal")
            {
                Animal animal;
                hitCollider.TryGetComponent<Animal>(out animal);

                animal.scaredOfPlayer = true;
                animals.Add(animal);
                StartCoroutine("turnOffScared");
            }
        }
    }

    public IEnumerator turnOffScared()
    {
        yield return new WaitForSeconds(scaredDuration);
        foreach(var animal in animals)
        {
            if(animal != null)
                animal.scaredOfPlayer = false;
        }
    }

}
