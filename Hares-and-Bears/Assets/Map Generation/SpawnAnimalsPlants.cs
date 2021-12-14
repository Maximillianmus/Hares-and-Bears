using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class SpawnAnimalsPlants : MonoBehaviour
{

    public MeshGenerator terrain;
    public LayerMask terrainLayer;
    public GameObject rabbitPrefab;
    public GameObject foxPrefab;
    public List<GameObject> plantPrefabs = new List<GameObject>();
    public List<int> plantSpawnNumber = new List<int>();

    public GameObject treePrefab;
    public GameObject deerPrefab;
    public GameObject bearPrefab;
    
    public float minBoundryX;
    public float minBoundryZ;
    public float maxBoundryX;
    public float maxBoundryZ;

    public int numberOfPlantsBegin = 6;
    public int numberOfEachAnimalsHerbivor = 4;
    public int minNumberOfTrees = 2;
    public int maxNumberOfTrees = 5;

    // Start is called before the first frame update
    void Start()
    {
        minBoundryX = terrain.transform.position.x - terrain.TerrainSizeX * terrain.QuadSize/2;
        minBoundryZ = terrain.transform.position.z - terrain.TerrainSizeZ * terrain.QuadSize/2;
        maxBoundryX = terrain.transform.position.x + terrain.TerrainSizeX * terrain.QuadSize/2;
        maxBoundryZ = terrain.transform.position.z + terrain.TerrainSizeZ * terrain.QuadSize/2;
    }

    public void startSpawn()
    {

        for (int i = 0; i < numberOfEachAnimalsHerbivor; ++i)
        {
            Spawn(rabbitPrefab);
            Spawn(deerPrefab);
        }
        Spawn(foxPrefab);
        Spawn(bearPrefab);

        if (plantSpawnNumber.Count != plantPrefabs.Count)
        {
            Debug.LogError("A spawn number must be given for each plant !");
            return;
        }

        for (int i = 0; i < plantPrefabs.Count; ++i)
        {
            var toSpawn = plantPrefabs[i];
            var numbersToSpawn = plantSpawnNumber[i];
            for (int j = 0; j < numbersToSpawn; ++j)
            {
                Spawn(toSpawn);
            }
        }


        int numTrees = Random.Range(minNumberOfTrees, maxNumberOfTrees);
        for(int i = 0; i < numTrees; i++)
        {
            Spawn(treePrefab);
        }
    }

    public void Spawn(GameObject toSpawn)
    {
        Vector3 randomPos = new Vector3(Random.Range(minBoundryX, maxBoundryX), 20, Random.Range(minBoundryZ, maxBoundryZ));
        RaycastHit hit;
        if(Physics.Raycast(randomPos, Vector3.down, out hit, 1000, terrainLayer))
        {
            Instantiate(toSpawn, hit.point, Quaternion.identity);
        }
    }


}
