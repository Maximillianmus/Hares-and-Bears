using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ecosystem;
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
    public string currentAction = "default";
    public float hunger;
    public float thirst;
    public float age;
    public float desireToMate;
    public bool alive;
    public bool exploring;
    public Vector3 prevExploreLocation;
    public bool pregnant;
    public int currentPregnantTicks;
    public bool scaredOfPlayer = false;

    [Header("Other")]
    public TimeManager timeManager;
    public NavMeshAgent agent;
    public Transform player;
    public ParticleSystem mateEffect;
    public ParticleSystem watersplash;
    public ParticleSystem skull;
    private WaterFinder waterFinder;

    public Animator animator;
    public bool eatingDrinking;

    private FoodSpawner foodSpawner;

    public void Start()
    {
        if(timeManager == null)
            GameObject.Find("TimeManager").TryGetComponent<TimeManager>(out timeManager);

        var foodSpawnerGo = GameObject.FindGameObjectWithTag("FoodSpawner");
        foodSpawner = foodSpawnerGo.GetComponent<FoodSpawner>();

        // Make it so gameUpdate is called every in game tick
        TimeManager.onTimeAdvance += gameUpdate;

        male = Random.Range(0, 2) == 1;
        hunger = maxHunger * 2/3;
        thirst = maxThirst * 2/3;
        age = 0;
        alive = true;
        exploring = false;
        pregnant = false;
        desireToMate = 0;
        currentPregnantTicks = 0;
        agent.speed = maxMovespeed;
        eatingDrinking = false;

        //player = GameObject.Find("AR Session Origin/AR Camera").transform;
    }

    public void Update()
    {

        if (waterFinder == null)
        {
            if (!GameObject.FindGameObjectWithTag("Terrain").TryGetComponent<WaterFinder>(out waterFinder))
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

            if (!scaredOfPlayer)
            {
                agent.speed = maxMovespeed * timeManager.GetMultiplier();
            }
            else
            {
                // While agent is scared, move a bit faster
                agent.speed = maxMovespeed * timeManager.GetMultiplier() * 2.0f;
            }

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

        if(eatingDrinking == true) {
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

        if (waterFinder.pointsGenerated)
        {

            var foundPoints = waterFinder.waterNear(transform.position, distToWater);
            if (foundPoints.Count == 0)
            {
                asr.waterClose = false;
            }
            else
            {
                asr.closestWater = foundPoints[0]; 
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
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    // Animal will go to closest food source
                    if (dist < distToFood)
                    {
                        asr.closestFood = hitCollider.gameObject;
                        asr.foodEatable = lf;
                        distToFood = dist;

                    }
                }

            }
            // Check for mate and predator
            if (hitCollider.tag == "Animal")
            {
                AnimalBehavior animal;
                hitCollider.TryGetComponent<AnimalBehavior>(out animal);

                // Can only mate with other of its species and opposite sex
                // potential mate must desire to mate and be of age
                if (animal.species == species && animal.male != male && animal.desireToMate >= animal.requredDesireToMate && animal.age >= animal.ageRequiredToMate)
                {
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (dist < distToMate)
                    {
                        asr.closestMate = animal;
                        distToMate = dist;
                    }
                }
                // Check for possible predators
                if (predators.Contains(animal.species))
                {
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (dist < distToPredator)
                    {
                        //asr.closestPredator = hitCollider.gameObject;
                        distToPredator = dist;
                    }
                }
            }
        }

        if (foodSpawner.Hand != null && Vector3.Distance(foodSpawner.Hand.transform.position, transform.position) <= viewDistance
            && foodSpawner.Food != null)
        {
            asr.closestFood = foodSpawner.Food;
            asr.foodEatable = foodSpawner.Food.GetComponent<FoodDestroyer>();
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
            currentAction = "Running away from player";
            Vector3 vToPred = Vector3.Normalize(player.position - transform.position);
            agent.SetDestination(transform.position - vToPred * viewDistance);
        }
        else
        {
            // If no predator near
            if (asr.nearbyPredators == null)
            {
                // Look for Mate
                if (asr.closestMate != null)
                {
                    // Must have desire to mate, and be of age. And must have high enough hunger and thirst
                    if (desireToMate >= requiredDesireForMating && age >= ageRequiredToMate && hunger >= comfortableHungerLevel * maxHunger && thirst >= comfortableThirstLevel)
                    {
                        // Close enough to mate
                        if (Vector3.Distance(asr.closestMate.transform.position, transform.position) <= interactRange)
                        {
                            currentAction = "mating";
                            desireToMate = 0;
                            hunger -= 2f;
                            thirst -= 2f;
                            if (!male)
                            {
                                print("bla");
                                pregnant = true;
                                ParticleSystem ps = Instantiate(mateEffect, transform.position, Quaternion.identity);
                                StartCoroutine(destroyParticleSystem(ps));
                            }
                        }
                        // go to mate
                        else
                        {
                            currentAction = "moving to mate";
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
                        currentAction = "eating";
                        asr.foodEatable.Eat();
                        hunger = maxHunger;
                        agent.SetDestination(transform.position);

                        if (animator != null)
                        {
                            eatOrDrink();
                            StartCoroutine(waitEatDrink(4.5f));
                        }
                    }
                    // Go to foodsource
                    else
                    {
                        currentAction = "moving to eat";
                        agent.SetDestination(asr.closestFood.transform.position);
                    }

                }
                // Animal wants to drink
                else if (thirst <= comfortableThirstLevel * maxThirst && asr.waterClose)
                {
                    // If close enough, drink!
                    if (Vector3.Distance(asr.closestWater, transform.position) <= interactRange)
                    {
                        currentAction = "drinking";
                        Instantiate(watersplash, transform.position, Quaternion.identity);
                        thirst = maxThirst;
                        agent.SetDestination(transform.position);

                        if (animator != null)
                        {
                            eatOrDrink();
                            StartCoroutine(waitEatDrink(4.5f));
                        }
                    }
                    // Go to watersource
                    else
                    {
                        currentAction = "moving to drink";
                        agent.SetDestination(asr.closestWater);
                    }
                }
                // If not found anything interesting, go explore
                else
                {
                    currentAction = "exploring";
                    if (!exploring)
                    {
                        StartCoroutine("Explore");
                    }
                    else
                    {
                        //agent.SetDestination(prevExploreLocation);
                    }
                }
            }
            // If a predator is near, move away from it
            else
            {
                
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

            age += 0.1f;

            if(age >= maxAge)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                Instantiate(skull, transform.position, rotation);
                alive = false;
            }
            hunger -= 0.1f;

            if (hunger <= 0)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                Instantiate(skull, transform.position, rotation);
                alive = false;
            }
            thirst -= 0.1f;

            if (thirst <= 0)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                Instantiate(skull, transform.position, rotation);
                alive = false;
            }
        }
        else
        {
            NavMeshAgent agent;
            if(gameObject.TryGetComponent<NavMeshAgent>(out agent))
            {
                agent.enabled = false;
            }
            TimeManager.onTimeAdvance -= gameUpdate;
            Destroy(gameObject);
        }
    }

    public IEnumerator Explore()
    {
        exploring = true;
        prevExploreLocation = transform.position + Quaternion.Euler(0, Random.Range(0, 360), 0) * new Vector3(viewDistance, 0, 0);
        agent.SetDestination(prevExploreLocation);

        yield return new WaitForSeconds(timeBetweenExploring / timeManager.GetMultiplier());

        exploring = false;
    }

    public IEnumerator destroyParticleSystem(ParticleSystem ps)
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(ps.gameObject);
    }

    private void eatOrDrink() {

        eatingDrinking = true;
        animator.SetTrigger("Eat_Drink");
    }

    public IEnumerator waitEatDrink(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        eatingDrinking = false;
    }

}
