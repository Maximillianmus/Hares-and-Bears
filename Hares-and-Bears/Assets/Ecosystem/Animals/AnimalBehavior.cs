using System.Collections;
using System.Collections.Generic;
using Ecosystem;
using UnityEngine;
using UnityEngine.AI;

public class AnimalBehavior : Lifeform, Eatable
{

    public GameObject prefab;
    public bool male;
    public List<Species> diet;
    public List<Species> predators;

    public AudioSource Audiosource;

    [System.Serializable]
    public class Action
    {
        public AnimalActions action;
        public int actionDuration;
        public string animationTag;
        public ParticleSystem ps;
        public AudioClip sound;
    }
    [Header("Animal Actions")]
    public Action movingAction;
    public Action eatingAction;
    public Action drinkingAction;
    public Action matingAction;
    public Action waitingAction;
    public Action fleeingAction;
    public Action exploringAction;
    public Action fleeFromPlayerAction;
    [Space]
    public Action currentAction;
    public Vector3 currentTarget;
    public int ticksUntillNewAction = 0;
    private AreaScanResult asr;

    [Header("Animal Attributes")]
    public float viewDistance;
    public float interactDistance;
    public float movementSpeed;
    [Space]
    public float maxAge;
    public float ageRequiredToMate;
    public float maxHunger;
    public float comfortableHunger;
    public float hungerRequiredToMate;
    public float maxThirst;
    public float comfortableThirst;
    public float thirstRequiredToMate;
    public float requredDesireToMate;
    public float pregnantForTicks;
    public int minKids;
    public int maxKids;

    [Header("Animal Stats")]
    public bool alive;
    public float age;
    public float hunger;
    public float thirst;
    public float desireToMate;
    public bool pregnant;
    public float currentPregnantTicks;
    public bool scaredOfPlayer;


    [Header("Other")]
    public TimeManager timeManager;
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    private WaterFinder waterFinder;
    private SpawnAnimalsPlants spawnAnimalsPlants;

    [Header("Particle effects")]
    public ParticleSystem mateEffect;
    public ParticleSystem watersplash;
    public ParticleSystem skull;
    
    [Header("Interaction with Hand recognition")]
    private HandFoodSpawner _handFoodSpawner;
    public float addedViewDistanceForHandFood;


    
    
    public void Start()
    {
        if (timeManager == null)
            GameObject.Find("TimeManager").TryGetComponent<TimeManager>(out timeManager);

        // Finding foodSpawner 
        var foodSpawnerGo = GameObject.FindGameObjectWithTag("HandFoodSpawner");
        if (foodSpawnerGo == null)
        {
            Debug.LogError("The object FoodSpawner is not in the scene or don't have the correct tag. \n" +
                           "Needed for interaction with hand recognition");
        }
        else
        {
            _handFoodSpawner = foodSpawnerGo.GetComponent<HandFoodSpawner>();
            if (_handFoodSpawner == null)
                Debug.LogError("The object should have a FoodSpawner script.");
        }

        
        
        // Make it so gameUpdate is called every in game tick
        TimeManager.onTimeAdvance += gameUpdate;

        if (waterFinder == null)
        {
            if (!GameObject.FindGameObjectWithTag("Terrain").TryGetComponent<WaterFinder>(out waterFinder))
            {
                Debug.LogError("WaterFinder can not be found ! ");
                return;
            }
        }

        if(spawnAnimalsPlants == null)
        {
            waterFinder.gameObject.TryGetComponent<SpawnAnimalsPlants>(out spawnAnimalsPlants);
        }

        if(player == null)
        {
            GameObject.Find("AR Session Origin/AR Camera").TryGetComponent<Transform>(out player);
        }

        male = Random.Range(0, 2) == 1;
        alive = true;
        age = 0;
        hunger = 0.6f * maxHunger;
        thirst = 0.6f * maxThirst;
        desireToMate = 0;
        pregnant = false;
        currentPregnantTicks = 0;
        scaredOfPlayer = false;

        currentAction = waitingAction;
        ticksUntillNewAction = waitingAction.actionDuration;
        asr = new AreaScanResult();
    }

    public void Update()
    {
        if (!timeManager.paused)
        {
            // If game is fast forwarded, scale movementspeed
            agent.speed = movementSpeed * timeManager.GetMultiplier();
        }
        else
        {
            agent.speed = 0;
        }
        Debug.DrawRay(transform.position, transform.forward*viewDistance, Color.red);
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.blue);
    }


    public void OnDestroy()
    {
        TimeManager.onTimeAdvance -= gameUpdate;
    }

    public void gameUpdate()
    {
        updateAnimalValues();

        // Player interaction should probably overwrite anything the animal is doing
        if(scaredOfPlayer)
        {
            currentAction = fleeFromPlayerAction;
            performNextAction();
        }
        else
        {
            if (ticksUntillNewAction < currentAction.actionDuration)
            {
                ticksUntillNewAction++;
            }
            else
            {
                ticksUntillNewAction = 0;
                scanArea();
                decideNextAction();
                performNextAction();
            }
        }
    }
    
    

    public void updateAnimalValues()
    {
        age += 0.1f;
        if(age >= maxAge)
            alive = false;

        hunger -= 0.1f;
        if (hunger <= 0)
            alive = false;

        thirst -= 0.1f;
        if(thirst <= 0)
            alive = false;

        if (!alive)
        {
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            Instantiate(skull, transform.position, rotation);
            Destroy(gameObject);
        }

        if(pregnant)
        {
            if(currentPregnantTicks >= pregnantForTicks)
            {
                pregnant = false;
                currentPregnantTicks = 0;

                int numKids = Random.Range(minKids, maxKids);
                for(int i = 0; i<numKids; i++)
                {
                    Instantiate(prefab, transform.position, Quaternion.identity);
                }
            }
            else
            {
                currentPregnantTicks++;
            }
        }
        else
        {
            desireToMate += 0.05f;
        }

    }

    public void scanArea()
    {
        asr = new AreaScanResult();

        // Scan for water
        if (waterFinder.pointsGenerated)
        {

            var foundPoints = waterFinder.waterNear(transform.position, viewDistance);
            if (foundPoints.Count == 0)
            {
                asr.waterClose = false;
            }
            else
            {
                asr.closestWater = foundPoints[0];//[Random.Range(0, foundPoints.Count-1)];
                asr.waterClose = true;
            }
        }
        else
        {
            asr.waterClose = false;
        }

        float distToFood = viewDistance + 1;
        float distToMate = distToFood;
        float dist;

        // Check for colliders within viewDistance of animal
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject)
                continue;

            // Check for food
            if (hitCollider.CompareTag("Animal") || hitCollider.CompareTag("Plant"))
            {
                Lifeform lf;
                hitCollider.TryGetComponent<Lifeform>(out lf);

                // Animals don't eat others of the same species, and only eats things in their diet
                if (species != lf.species && diet.Contains(lf.species))
                {
                    dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    // Animal will go to closest food source
                    if (dist < distToFood)
                    {
                        asr.closestFood = hitCollider.gameObject;
                        if (hitCollider.CompareTag("Animal"))
                        {
                            asr.foodEatable = hitCollider.gameObject.GetComponent<AnimalBehavior>();
                        }
                        else
                        {
                            asr.foodEatable = hitCollider.gameObject.GetComponent<Plant>();
                        }
                        distToFood = dist;
                    }
                }

            }
            
            // Check for mate and predator
            if (hitCollider.CompareTag("Animal"))
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
                    asr.nearbyPredators.Add(animal.gameObject);
                }
            }
        }
        if (_handFoodSpawner != null &&
            _handFoodSpawner.Hand != null && Vector3.Distance(_handFoodSpawner.Hand.transform.position, transform.position) <= viewDistance + addedViewDistanceForHandFood
            && _handFoodSpawner.Food != null)
        {
            asr.closestFood = _handFoodSpawner.Food;
            asr.foodEatable = _handFoodSpawner.Food.GetComponent<FoodDestroyer>();
        }
        
    }

    public void decideNextAction()
    {
        currentAction = null;
        // If no predator nearby
        if (asr.nearbyPredators.Count == 0)
        {
            // If there is a mate
            if(asr.closestMate != null && hunger >= hungerRequiredToMate && thirst >= thirstRequiredToMate)
            {
                if(Vector3.Distance(asr.closestMate.transform.position, transform.position) <= interactDistance)
                {
                    currentAction = matingAction;
                }
                else
                {
                    currentAction = movingAction;
                    currentTarget = asr.closestMate.transform.position;
                }
            }
            // If hungry and there is food nearby, go eat!
            else if (hunger <= comfortableHunger && asr.closestFood != null)
            {    
                if(Vector3.Distance(asr.closestFood.transform.position, transform.position) <= interactDistance)
                {
                    currentAction = eatingAction;
                }
                else
                {
                    currentAction = movingAction;
                    currentTarget = asr.closestFood.transform.position;
                }
            }
            // If thirsty and there is water nearby, go drink!
            else if (thirst <= comfortableThirst && asr.waterClose)
            {
                if (Vector3.Distance(asr.closestWater, transform.position) <= interactDistance)
                {
                    currentAction = drinkingAction;
                }
                else
                {
                    currentAction = movingAction;
                    currentTarget = asr.closestWater;
                }
            }
            // Explore
            else
            {
                // small chance for animal to just stand and wait
                if(Random.Range(0f, 1f) <= 0.3f)
                {
                    currentAction = waitingAction;
                }
                // Most likely to explore
                else
                {
                    currentAction = exploringAction;
                    currentTarget = transform.position + Quaternion.Euler(0, Random.Range(0, 360), 0) * new Vector3(viewDistance, 0, 0);
                    
                    // If exploretarget is outside of map, adjust it

                    if(currentTarget.x < spawnAnimalsPlants.minBoundryX || currentTarget.x > spawnAnimalsPlants.maxBoundryX)
                    {
                        currentTarget.x += 2 * (transform.position.x - currentTarget.x);
                    }
                        
                    if(currentTarget.z < spawnAnimalsPlants.minBoundryZ || currentTarget.z > spawnAnimalsPlants.maxBoundryZ)
                    {
                        currentTarget.z += 2 * (transform.position.z - currentTarget.z);
                    }
                }
            }
        }
        else
        {
            // Flee from predators
            currentAction = fleeingAction;
        }
    }

    public void performNextAction()
    {
        agent.SetDestination(transform.position);
        switch(currentAction.action)
        {
            case AnimalActions.Moving:
                agent.SetDestination(currentTarget);
                break;
            case AnimalActions.Eating:
                asr.foodEatable.Eat();
                hunger = maxHunger;
                break;
            case AnimalActions.Drinking:
                thirst = maxThirst;
                break;
            case AnimalActions.Mating:
                desireToMate = 0;
                asr.closestMate.desireToMate = 0;
                if (!male)
                {
                    pregnant = true;
                }
                else
                {
                    asr.closestMate.pregnant = true;
                }
                break;
            case AnimalActions.Waiting:
                break;
            case AnimalActions.Fleeing:
                Vector3 direction = new Vector3(0,0,0);
                foreach(GameObject predator in asr.nearbyPredators)
                {
                    direction += Vector3.Normalize(transform.position - predator.transform.position);
                }
                agent.SetDestination(transform.position + direction.normalized * viewDistance);
                break;
            case AnimalActions.Exploring:
                agent.SetDestination(currentTarget);
                break;
            case AnimalActions.FleeFromPlayer:
                Vector3 dir = Vector3.Normalize(transform.position - player.position);
                agent.SetDestination(transform.position + dir * viewDistance);
                break;
            default:
                print("ERROR NO CURRENT ACTION");
                break;

        }

        // if there is a particle effect related to action, play it
        if(currentAction.ps != null)
        {
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            Instantiate(currentAction.ps, transform.position, rotation);
        }
        // if there is a specific animation related to action, do it
        if(currentAction.animationTag != "" && animator != null)
        {
            animator.SetTrigger(currentAction.animationTag);
        }

        if(currentAction.sound != null)
        {
            Audiosource.PlayOneShot(currentAction.sound);
        }

    }

    public void Eat()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        Instantiate(skull, transform.position, rotation);
        Destroy(gameObject);
    }
}
