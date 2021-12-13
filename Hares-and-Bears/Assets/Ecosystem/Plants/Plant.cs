using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

struct AreaPlantScan
{
    public int numberOfAnimalInRange;
    public int numberOfPlantInRange;
}


public abstract class Plant : Lifeform
{
    public float nutritionValue;
    [SerializeField] private float fruitProducing = 0.0f;

    [Header("Propagation")]
    [SerializeField] private float tickPropagation = 0.5f;
    [SerializeField] private float toPropagageThreshold = 18.0f;
     private bool canPropagate = false;
    [SerializeField] private float plantEffectInArea = 1.0f;
    [SerializeField] private float animalEffectInArea = 0.3f;
    [SerializeField] private float RadiusAreaOfEffect = 0.5f;
    [SerializeField] private int maxNumberOfPropagation = 2;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private GameObject prefabToPropagate;
    [SerializeField] private int maxPlants;
    [SerializeField] private SpawnAnimalsPlants spawnAnimalPlants;
    

    // Start is called before the first frame update
    public virtual void Start()
    {
        TimeManager.onTimeAdvance += onTick;
        var meshGenerator = GameObject.FindGameObjectWithTag("Terrain");
        spawnAnimalPlants = meshGenerator.GetComponent<SpawnAnimalsPlants>();


    }

    public virtual void Update()
    {
        var aps = ScanArea();
        Act(aps);

    }

    private void Act(AreaPlantScan aps)
    {
        if (canPropagate)
        {
            var coefPropagation = aps.numberOfAnimalInRange * animalEffectInArea +
                                  (aps.numberOfPlantInRange - 1) * plantEffectInArea;
            canPropagate = false;
            var plants = GameObject.FindGameObjectsWithTag("Plant");
            if (plants.Length >= maxPlants)
                return;
            for (int i = 0; i < Math.Min((int) coefPropagation, maxNumberOfPropagation); ++i)
            {
                PropagatePlant();
            }
        }
    }

    private void PropagatePlant()
    {
        var x = Random.Range(Math.Max(spawnAnimalPlants.minBoundryX, transform.position.x - RadiusAreaOfEffect),
            Math.Min(spawnAnimalPlants.maxBoundryX, transform.position.x + RadiusAreaOfEffect));
        var z = Random.Range(Math.Max(spawnAnimalPlants.minBoundryZ, transform.position.z - RadiusAreaOfEffect),
            Math.Min(spawnAnimalPlants.maxBoundryZ, transform.position.z + RadiusAreaOfEffect));
        
        
        
        
        Vector3 noise = (new Vector3(x, 0, z));
        
        RaycastHit hit;
        Vector3 sourcePoint =  noise + new Vector3(0, 20, 0);
        if(Physics.Raycast(sourcePoint, Vector3.down, out hit, 1000, terrainLayer))
        {
            Instantiate(prefabToPropagate, hit.point, Quaternion.identity);
        } else
        {
            print("Nope");
        }
    }

    private AreaPlantScan ScanArea()
    {
        var hits = Physics.OverlapSphere(transform.position, RadiusAreaOfEffect);
        AreaPlantScan aps = new AreaPlantScan();
        aps.numberOfAnimalInRange = 0;
        aps.numberOfPlantInRange = 0;
        foreach (var collider in hits)
        {
            if (collider.CompareTag("Animal"))
            {
                aps.numberOfAnimalInRange++;
            } else if (collider.CompareTag("Plant"))
            {
                aps.numberOfPlantInRange++;
            } 
        }

        return aps;

    }

    void onTick()
    {
        fruitProducing += tickPropagation;
        if (fruitProducing > toPropagageThreshold)
        {
            fruitProducing = 0;
            canPropagate = true;
        }
    }

}
