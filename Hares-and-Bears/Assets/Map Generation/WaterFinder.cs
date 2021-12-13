using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class WaterFinder : MonoBehaviour
{
    [SerializeField] private MeshGenerator _generator;
    public List<Vector3> pointInWater;
    public bool pointsGenerated;
    [SerializeField]
    [Tooltip("If this is turned on the water nodes will be visable")]
    private bool _debugMode;
    [Tooltip("Prefab used to show where water nodes are")]
    [SerializeField]
    private GameObject debugingNode;
    [Tooltip("The lower limit of points we accept as water")]
    [SerializeField]
    private float _waterThreshold = 0.1f;

    private void Start()
    {
        _generator = GetComponent<MeshGenerator>();
        pointsGenerated = false;
    }

    private void Update()
    {
        if (_generator.enabled || pointsGenerated)
        {
            this.enabled = false;
            return;
        }

        //Robins old thing
        /*
        float waterY = _generator.waterLevel * _generator.maxHeight;
       
        var vertices = _generator.Vertices;
        pointInWater = from vertice in vertices
            where vertice.y <= waterY
            select vertice + _generator.transform.position;
        pointsGenerated = true;
        */
        


    }

    public void FindWater()
    {
        
        GameObject debugWater = new GameObject("debug Water");

        float waterY = _generator.waterLevel * _generator.maxHeight;
        for (int x = 0; x < _generator.Vertices.Length; x++)
                if (_generator.Vertices[x] != null && (_generator.Vertices[x]+_generator.transform.position).y <= waterY && (_generator.Vertices[x] + _generator.transform.position).y > waterY - _waterThreshold)
                {

                if(_debugMode)
                {
                    print("Water Point Saved");
                    GameObject waterPoint = Instantiate(debugingNode, _generator.Vertices[x] + _generator.transform.position, new Quaternion());
                    waterPoint.transform.parent = debugWater.transform;

                }
                    


                pointInWater.Add(_generator.Vertices[x] + _generator.transform.position);
                }


        if (_debugMode)
            print("There are " + pointInWater.Count + " water points");

        pointsGenerated = true;
    }

    public List<Vector3> waterNear(Vector3 sourcePos, float viewDistance)
    {
        List<Vector3> foundPoints = new List<Vector3>();
        foreach (Vector3 waterPoint in pointInWater)
        {
   
            if (Vector3.Distance(waterPoint, sourcePos) <= viewDistance)
            {
                foundPoints.Add(waterPoint);
            }
               
        }
        //point that is very far away, if we are not close enough
        return foundPoints;
    }
}
