using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimalsPlants : MonoBehaviour
{

    public MeshGenerator terrain;
    public LayerMask terrainLayer;
    public GameObject rabbit;

    public float minBoundryX;
    public float minBoundryZ;
    public float maxBoundryX;
    public float maxBoundryZ;

    // Start is called before the first frame update
    void Start()
    {
        minBoundryX = terrain.transform.position.x - terrain.TerrainSizeX * terrain.QuadSize;
        minBoundryZ = terrain.transform.position.z - terrain.TerrainSizeZ * terrain.QuadSize;
        maxBoundryX = terrain.transform.position.x + terrain.TerrainSizeX * terrain.QuadSize;
        maxBoundryZ = terrain.transform.position.z + terrain.TerrainSizeZ * terrain.QuadSize;
    }

    public void startSpawn()
    {
        print("hello");
        Vector3 randomPos = new Vector3(Random.Range(minBoundryX, maxBoundryX), 20, Random.Range(minBoundryZ, maxBoundryZ));
        RaycastHit hit;
        print(randomPos);
        if(Physics.Raycast(randomPos, Vector3.down, out hit, 40, terrainLayer))
        {
            print("Hit!");
            Instantiate(rabbit, hit.point, Quaternion.identity);
        }
        else
        {
            Debug.DrawLine(randomPos, randomPos + Vector3.down*20);
            print("Miss");
        }
    }


}
