using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnAnimalsPlants : MonoBehaviour
{

    public MeshGenerator terrain;
    public LayerMask terrainLayer;
    public GameObject rabbitPrefab;
    public GameObject foxPrefab;
    
    public float minBoundryX;
    public float minBoundryZ;
    public float maxBoundryX;
    public float maxBoundryZ;

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
        for (int i = 0; i < 4; ++i)
        {
            spawnOnAnimals(rabbitPrefab);
        }
        spawnOnAnimals(foxPrefab);
    }

    public void spawnOnAnimals(GameObject toSpawn)
    {
        Vector3 randomPos = new Vector3(Random.Range(minBoundryX, maxBoundryX), 20, Random.Range(minBoundryZ, maxBoundryZ));
        RaycastHit hit;
        if(Physics.Raycast(randomPos, Vector3.down, out hit, 1000, terrainLayer))
        {
            Instantiate(toSpawn, hit.point, Quaternion.identity);
            print(hit.point);
        }
    }


}
