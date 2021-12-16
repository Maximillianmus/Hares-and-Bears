using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareAnimal : MonoBehaviour
{
    public TimeManager timeManager;
    public float range = 1f;
    public int scaredForTicks = 10;
    public int currentTicks = 0;
    public bool activated = false;
    public List<AnimalBehavior> animals;
    public AudioSource Audiosource;

    public void Start()
    {
        if (timeManager == null)
            GameObject.Find("TimeManager").TryGetComponent<TimeManager>(out timeManager);

        TimeManager.onTimeAdvance += gameUpdate;
    }

    public void scareAnimals()
    {
        Audiosource.Play();
        activated = true;
        animals = new List<AnimalBehavior>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Animal")
            {
                AnimalBehavior animal;
                hitCollider.TryGetComponent<AnimalBehavior>(out animal);

                animal.scaredOfPlayer = true;
                animals.Add(animal);
            }
        }
    }

    public void gameUpdate()
    {
        if(activated)
        {
            if (currentTicks > scaredForTicks)
            {
                foreach (var animal in animals)
                {
                    if (animal != null)
                        animal.scaredOfPlayer = false;
                }
                currentTicks = 0;
                activated = false;
            }
            else
            {
                currentTicks++;
            }
        }
    }
}
