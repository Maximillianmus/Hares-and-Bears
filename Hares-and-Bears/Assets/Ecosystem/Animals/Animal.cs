using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public abstract class Animal : Lifeform
{
    [Header("Animal Attributes")]
    public GameObject prefab;
    public bool male;
    public List<Species> diet;
    public List<Species> predators;

    [Space]
    public float maxHunger = 5f;
    public float maxThirst = 5f;
    public float maxAge = 10f;
    public float ageRequiredToMate = 2f;
    [Range(0, 1)]
    public float requiredDesireForMating = 0.7f;
    public int pregnantForTicks = 10;
    public int minOffspring = 1;
    public int maxOffspring = 4;

    [Space]
    public float maxMovespeed = 1f;
    public float viewDistance = 5f;
    public float interactRange = 1f;
    public float timeBetweenExploring = 2f;
    [Range(0, 1)]
    public float comfortableHungerLevel = 0.5f;
    [Range(0, 1)]
    public float comfortableThirstLevel = 0.5f;

    [Header("Status")]
    public float hunger;
    public float thirst;
    public float age;
    public float desireToMate;
    public bool alive;
    public bool exploring;
    public bool pregnant;
    public int currentPregnantTicks;
    public bool scaredOfPlayer = false;

    [Header("Other")]
    public TimeManager timeManager;
    public NavMeshAgent agent;
    public Transform player;


    public void Start()
    {
        if(timeManager == null)
            GameObject.Find("TimeManager").TryGetComponent<TimeManager>(out timeManager);

        // Make it so gameUpdate is called every in game tick
        TimeManager.onTimeAdvance += gameUpdate;

        male = Random.Range(0, 2) == 1;
        hunger = maxHunger;
        thirst = maxThirst;
        age = 0;
        alive = true;
        exploring = false;
        pregnant = false;
        desireToMate = 0;
        currentPregnantTicks = 0;
        agent.speed = maxMovespeed;
    }

    public void Update()
    {
        if (!alive)
            agent.speed = 0;

        if(!timeManager.paused)
        {
            // If game is fast forwarded, scale movementspeed
            agent.speed = maxMovespeed * timeManager.GetMultiplier();

            if(alive)
            {
                AreaScanResult asr = scanArea();
                Act(asr);
            }
        }
        else
        {
            agent.speed = 0;
        }
    }

    public virtual AreaScanResult scanArea()
    {
        if (scaredOfPlayer)
            return new AreaScanResult();

        float distToFood = viewDistance + 1;
        float distToWater = distToFood;
        float distToMate = distToFood;
        float distToPredator = distToFood;
        float dist;

        // Class that holds results from the scan
        AreaScanResult asr = new AreaScanResult();

        // Check for colliders within viewDistance of animal
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);
        foreach (var hitCollider in hitColliders)
        {
            // Check for water
            if (hitCollider.tag == "WaterSource")
            {
                dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                // Animal will go to closest water source
                if (dist < distToWater)
                {
                    asr.closestWater = hitCollider.gameObject;
                    distToWater = dist;
                }
            }
            // Check for food
            if (hitCollider.tag == "Animal" || hitCollider.tag == "Plant")
            {
                Lifeform lf;
                hitCollider.TryGetComponent<Lifeform>(out lf);

                // Animals don't eat others of the same species, and only eats things in their diet
                if(species != lf.species && diet.Contains(lf.species))
                {
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    // Animal will go to closest food source
                    if (dist < distToFood)
                    {
                        asr.closestFood = hitCollider.gameObject;
                        distToFood = dist;
                    }
                }

            }
            // Check for mate and predator
            if (hitCollider.tag == "Animal")
            {
                Animal animal;
                hitCollider.TryGetComponent<Animal>(out animal);

                // Can only mate with other of its species and opposite sex
                // potential mate must desire to mate and be of age
                if (animal.species == species && animal.male != male && animal.desireToMate >= animal.requiredDesireForMating && animal.age >= animal.ageRequiredToMate)
                {
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (dist < distToMate)
                    {
                        asr.closestMate = hitCollider.gameObject;
                        distToMate = dist;
                    }
                }
                // Check for possible predators
                if (predators.Contains(animal.species))
                {
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (dist < distToPredator)
                    {
                        asr.closestPredator = hitCollider.gameObject;
                        distToPredator = dist;
                    }
                }
            }
        }
        return asr;
    }

    public void OnDestroy()
    {
        alive = false;
        TimeManager.onTimeAdvance -= gameUpdate;
    }

    public virtual void Act(AreaScanResult asr)
    {
        if(scaredOfPlayer)
        {
            Vector3 vToPred = Vector3.Normalize(player.position - transform.position);
            agent.SetDestination(transform.position - vToPred * viewDistance);
        }
        else
        {
            // If no predator near
            if (asr.closestPredator == null)
            {
                // Look for Mate
                if (asr.closestMate != null)
                {

                    // Must have desire to mate, and be of age
                    if (desireToMate >= requiredDesireForMating && age >= ageRequiredToMate)
                    {

                        // Close enough to mate
                        if (Vector3.Distance(asr.closestMate.transform.position, transform.position) <= interactRange)
                        {

                            print("MATING!");
                            desireToMate = 0;
                            if (!male)
                            {
                                pregnant = true;
                            }
                        }
                        // go to mate
                        else
                        {
                            agent.SetDestination(Vector3.MoveTowards(transform.position, asr.closestMate.transform.position, viewDistance));
                            //agent.SetDestination(asr.closestMate.transform.position);
                        }
                    }
                }

                // Animal wants to eat
                else if (hunger <= comfortableHungerLevel * maxHunger && asr.closestFood != null)
                {
                    // If close enough, eat!
                    if (Vector3.Distance(asr.closestFood.transform.position, transform.position) <= interactRange)
                    {
                        Destroy(asr.closestFood);
                        hunger = maxHunger;
                        agent.SetDestination(transform.position);
                    }
                    // Go to foodsource
                    else
                    {
                        agent.SetDestination(asr.closestFood.transform.position);
                    }

                }
                // Animal wants to drink
                else if (thirst <= comfortableThirstLevel * maxThirst && asr.closestWater != null)
                {
                    // If close enough, drink!
                    if (Vector3.Distance(asr.closestWater.transform.position, transform.position) <= interactRange)
                    {
                        thirst = maxThirst;
                        agent.SetDestination(transform.position);
                    }
                    // Go to watersource
                    else
                    {
                        agent.SetDestination(asr.closestWater.transform.position);
                    }
                }
                // If not found anything interesting, go explore
                else
                {
                    if (!exploring)
                    {
                        StartCoroutine("Explore");
                    }

                }
            }
            // If a predator is near, move away from it
            else
            {
                Vector3 vToPred = Vector3.Normalize(asr.closestPredator.transform.position - transform.position);
                agent.SetDestination(transform.position - vToPred * viewDistance);
            }
        }
    }

    // Is called every ingame tick
    public void gameUpdate()
    {
        if(alive)
        {
            if(pregnant)
            {
                if(currentPregnantTicks >= pregnantForTicks)
                {
                    int numKids = Random.Range(minOffspring, maxOffspring);
                    for(int i = 0; i < numKids; i++)
                    {
                        Instantiate(prefab, transform.position, Quaternion.identity);
                    }
                    pregnant = false;
                    currentPregnantTicks = 0;
                }
                currentPregnantTicks++;
            }
            else
            {
                desireToMate += 0.05f;
            }

            age += 0.05f;

            if(age >= maxAge)
                alive = false;

            hunger -= 0.05f;

            if (hunger <= 0)
                alive = false;

            thirst -= 0.05f;

            if (thirst <= 0)
                alive = false;
        }
        else
        {
            NavMeshAgent agent;
            if(gameObject.TryGetComponent<NavMeshAgent>(out agent))
            {
                agent.enabled = false;
            }
            TimeManager.onTimeAdvance -= gameUpdate;
            // Have had trouble with using destroy, using this for now to remove corpses
            transform.position = transform.position + new Vector3(0, -4, 0);
        }
    }

    public IEnumerator Explore()
    {
        exploring = true;
        agent.SetDestination(transform.position + Quaternion.Euler(0, Random.Range(0, 360), 0) * new Vector3(viewDistance, 0, 0));

        yield return new WaitForSeconds(timeBetweenExploring / timeManager.GetMultiplier());

        exploring = false;
    }


}
