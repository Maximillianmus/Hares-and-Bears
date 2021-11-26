using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;



[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(NavMeshSurface))]
public class MeshGenerator : MonoBehaviour
{

    [System.Serializable]
    public class Octave
    {

        [Tooltip("This is basicly zoom")]
        public float frequency;
        [Tooltip("This is the strength")]
        public float amplitude;
        public float[,] noiseMap;

        public void CreateNoise(int width, int height)
        {
            noiseMap = new float[width+1, height+1];
            float perlinRandomOffset = UnityEngine.Random.Range(0.0f, 1.0f);

            for (int y = 0; y <= height; y++)
                for(int x = 0; x <= width; x++)
                {
                    noiseMap[x, y] = Mathf.PerlinNoise((perlinRandomOffset + x * frequency), (perlinRandomOffset + y * frequency)) * amplitude;

                }

        }

    }



    [Tooltip("Size multiplier for quads")]
    public float QuadSize = 1;
    [Tooltip("The number of Quads in the X direction")]
    public int TerrainSizeX = 10;
    [Tooltip("The number of Quads in the Z direction")]
    public int TerrainSizeZ = 10;
    [Tooltip("Controls the hight")]
    public float maxHeight = 2;
    [Tooltip("Time between quad generation")]
    public float generationDelay = 0.01f;
    [Tooltip("Interpolation multiplier for height generation")]
    public float heightGenerationSpeed = 0.1f;
    [Tooltip("List of octaves in the order they are applied")]
    public Octave[] octaves;
    [Tooltip("The level that water should be placed at. 0 is the lowest point and 1 is the heighest")]
    [Range(0.0f, 1f)]
    public float waterLevel = 0.5f;
    [Tooltip("The prefab that will be spawned as water")]
    public GameObject waterPrefab;
    [Tooltip("The level that grass interpolation starts. 0 is the lowest point and 1 is the heighest")]
    [Range(0.0f, 1f)]
    public float grassStart = 0.5f; 
    [Tooltip("The level wehre grass interpolation has reached it's max. 0 is the lowest point and 1 is the highest")]
    [Range(0.0f, 1f)]
    public float grassMax = 0.7f; 


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
    private float[,] combinedNoiseMap;
    private float highestPoint = 0;
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
        material = GetComponent<MeshRenderer>().material;
        material.SetFloat("_Grass_start", grassStart * maxHeight);
        material.SetFloat("_Grass_end", grassMax * maxHeight);


        Instantiate(waterPrefab,  new Vector3(transform.position.x, transform.position.y + waterLevel * maxHeight, transform.position.z), transform.rotation) ;

        StartCoroutine(CreateShape());
    
        combinedNoiseMap = new float[TerrainSizeX+1,TerrainSizeZ+1];
        // generates the final noise map
        foreach(Octave noise in octaves){
            noise.CreateNoise(TerrainSizeX, TerrainSizeZ);
            highestPoint += noise.amplitude;
        }

        

        //find the highest point
        for (int y = 0; y <= TerrainSizeZ; y++)
            for (int x = 0; x <= TerrainSizeX; x++)
            {

                foreach (Octave noise in octaves)
                {
                    combinedNoiseMap[x, y] += noise.noiseMap[x, y];
                   
                }
                //normalizes value between 0 and 1
                combinedNoiseMap[x, y] = combinedNoiseMap[x, y] / highestPoint;

            }

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
        float currentHeightStrenght = Mathf.Lerp(0, maxHeight, InterpolationValue);
        for (int i = 0, z = 0; z <= TerrainSizeZ; z++)
        {
            for (int x = 0; x <= TerrainSizeX; x++)
            {

                float y = combinedNoiseMap[x, z] * maxHeight ;
                InterpolationValue += Time.deltaTime * heightGenerationSpeed;
                vertices[i] = new Vector3((positionOffsetX + x) * QuadSize, y, (positionOffsetZ + z) * QuadSize);
                i++;
            }
        }

        if (currentHeightStrenght == 1)
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
