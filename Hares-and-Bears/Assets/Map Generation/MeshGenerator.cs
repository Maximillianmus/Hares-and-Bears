using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;



[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(NavMeshSurface))]
public class MeshGenerator : MonoBehaviour
{
    [Tooltip("Size multiplier for quads")]
    public float QuadSize = 1;
    [Tooltip("The number of Quads in the X direction")]
    public int TerrainSizeX = 10;
    [Tooltip("The number of Quads in the Z direction")]
    public int TerrainSizeZ = 10;
    [Tooltip("Controls how high the tops are")]
    public float heightStrength = 2;
    [Tooltip("Time between quad generation")]
    public float generationDelay = 0.01f;
    [Tooltip("Interpolation multiplier for height generation")]
    public float heightGenerationSpeed = 0.1f;
    [Tooltip("Changes the size of the noise(Perlin)")]
    public float perlinZoom = 0.3f;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    private bool groundGenerationDone = false;
    private bool heightGenerationDone = false;
    private float InterpolationValue = 0;
    private float perlinStartPos = 0;
    private NavMeshSurface navMesh;
    private float positionOffsetX = 0;
    private float positionOffsetZ = 0;
    private Material material;

    public SpawnAnimalsPlants spawnAnimalsPlants;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        perlinStartPos = UnityEngine.Random.Range(0, 10000);
        GetComponent<MeshFilter>().mesh = mesh;
        navMesh = gameObject.GetComponent<NavMeshSurface>();
        positionOffsetX = -TerrainSizeX / 2;
        positionOffsetZ = -TerrainSizeZ / 2;
        material =GetComponent<MeshRenderer>().material;
     
        StartCoroutine(CreateShape());


    }

    void Update()
    {   
        //update mesh if it is being updated
        if (!groundGenerationDone)
        UpdateMesh();
        else if (!heightGenerationDone)
        {
            CreateHills();
            UpdateMesh();
        }
        else
        {
            UpdateMesh();
            //we are done with the generation
            gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            navMesh.BuildNavMesh();
            spawnAnimalsPlants.startSpawn();
            this.enabled = false;
        }
    }
    IEnumerator CreateShape()
    {
        vertices = new Vector3[(TerrainSizeX + 1) * (TerrainSizeZ + 1)];

        for (int i = 0, z = 0; z <= TerrainSizeZ; z++)
        {
            for (int x = 0; x <= TerrainSizeX; x++)
            {
                vertices[i] = new Vector3((positionOffsetX + x) * QuadSize, 0, (positionOffsetZ + z)* QuadSize);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[TerrainSizeX * TerrainSizeZ * 6];


        for(int z = 0; z < TerrainSizeZ; z++)
        {
            for (int x = 0; x < TerrainSizeX; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + TerrainSizeX + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + TerrainSizeX + 1;
                triangles[tris + 5] = vert + TerrainSizeX + 2;

                vert++;
                tris += 6;
            }
            //skip one connection when switching row
            yield return new WaitForSeconds(generationDelay);
            vert++;
        }

        groundGenerationDone = true;
    }

    void CreateHills()
    {
        float currentHeightStrenght = Mathf.Lerp(0, heightStrength, InterpolationValue);
        for (int i = 0, z = 0; z <= TerrainSizeZ; z++)
        {
            for (int x = 0; x <= TerrainSizeX; x++)
            {
                
                float y = Mathf.PerlinNoise(perlinStartPos+ x * perlinZoom * QuadSize, perlinStartPos + z * perlinZoom * QuadSize) * currentHeightStrenght;
                InterpolationValue += Time.deltaTime * heightGenerationSpeed;
                vertices[i] = new Vector3((positionOffsetX + x) * QuadSize, y, (positionOffsetZ + z) * QuadSize);
                i++;
            }
        }

        if (currentHeightStrenght == heightStrength)
            heightGenerationDone = true;

    }


    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    
}
