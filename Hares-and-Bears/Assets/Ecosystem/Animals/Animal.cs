using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;

public abstract class Animal : Lifeform
{
    [Header("Animal Attributes")]
    public bool male;
    public List<Species> diet;
    public List<Species> predators;

    [Space]
    public float maxHunger = 5f;
    public float maxThirst = 5f;
    public float maxAge = 10f;
    [Space]
    public float maxMovespeed = 1f;
    public float viewDistance = 5f;
    public float eatRange = 1f;
    public float timeBetweenExploring = 2f;
    [Range(0, 1)]
    public float comfortableHungerLevel = 0.5f;
    [Range(0, 1)]
    public float comfortableThirstLevel = 0.5f;

    [Header("Status")]
    public float hunger;
    public float thirst;
    public float age;
    public bool alive;
    public bool exploring;

    [Header("Other")]
    public TimeManager timeManager;
    public NavMeshAgent agent;
    private WaterFinder waterFinder;

    public void Start()
    {
        if(timeManager == null)
            GameObject.Find("TimeManager").TryGetComponent<TimeManager>(out timeManager);

        // Make it so gameUpdate is called every in game tick
        TimeManager.onTimeAdvance += gameUpdate;

        male = Random.Range(0, 1) < 0.5f;
        hunger = maxHunger;
        thirst = maxThirst;
        age = 0;
        alive = true;
        agent.speed = maxMovespeed;
    }

    public void Update()
    {
        if (waterFinder == null)
        {
            if (!GameObject.FindGameObjectWithTag("WaterFinder").TryGetComponent<WaterFinder>(out waterFinder))
            {
                Debug.LogError("WaterFinder can not be found ! ");
                return;
            }
        }
        if (!alive)
            agent.speed = 0;

        if(!timeManager.paused)
        {
            // If game is fast forwarded, scale movementspeed
            agent.speed = maxMovespeed * timeManager.GetMultiplier();

            AreaScanResult asr = scanArea();
            Act(asr);
        }
        else
        {
            agent.speed = 0;
        }
    }

    public virtual AreaScanResult scanArea()
    {
        float distToFood = viewDistance + 1;
        float distToWater = viewDistance + 1;

        // Class that holds results from the scan
        AreaScanResult asr = new AreaScanResult();

        if (waterFinder.pointsGenerated)
        {
            var waterNear = waterFinder.waterNear(transform.position, distToWater);
            var orderingPoint = waterNear.OrderBy(point => Vector3.Distance(transform.position, point));
            if (orderingPoint.Count() == 0)
            {
                asr.waterClose = false;
            }
            else
            {
                asr.closestWater = orderingPoint.First(); 
                asr.waterClose = true;
            }
        }
        else
        {
            asr.waterClose = false;

        }

        // Check for colliders within viewDistance of animal
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);
        foreach (var hitCollider in hitColliders)
        {
            // Check for food
            if (hitCollider.tag == "Animal" || hitCollider.tag == "Plant")
            {
                Lifeform lf;
                hitCollider.TryGetComponent<Lifeform>(out lf);

                // Animals don't eat others of the same species, and only eats things in their diet
                if(species != lf.species && diet.Contains(lf.species))
                {
                    float dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    // Animal will go to closest food source
                    if (dist < distToFood)
                    {
                        asr.closestFood = hitCollider.gameObject;
                        distToFood = dist;
                    }
                }

            }

            // Check for possible mates
            // TODO

            // Check for possible predators
            // TODO
        }
        return asr;
    }

    public abstract void Act(AreaScanResult asr);

    public void gameUpdate()
    {
        if(alive)
        {
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
    }

    public IEnumerator Explore()
    {
        exploring = true;
        agent.SetDestination(transform.position + Quaternion.Euler(0, Random.Range(0, 360), 0) * new Vector3(viewDistance, 0, 0));

        yield return new WaitForSeconds(timeBetweenExploring / timeManager.GetMultiplier());

        exploring = false;
    }


}
