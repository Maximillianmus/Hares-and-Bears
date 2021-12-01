using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class WaterFinder : MonoBehaviour
{
    [SerializeField] private MeshGenerator _generator;
    public  IEnumerable<Vector3> pointInWater;
    public bool pointsGenerated;

    private void Start()
    {
        pointsGenerated = false;
    }

    private void Update()
    {
        if (_generator.enabled || pointsGenerated)
        {
            return;
        }
        float waterY = _generator.waterLevel * _generator.maxHeight;
        var vertices = _generator.Vertices;
        pointInWater = from vertice in vertices
            where vertice.y <= waterY
            select vertice + _generator.transform.position;
        pointsGenerated = true;
    }

    public IEnumerable<Vector3> waterNear(Vector3 sourcePos, float viewDistance)
    {
        return from point in pointInWater
            where Vector3.Distance(point, sourcePos) <= viewDistance
            select point;
    }
}
