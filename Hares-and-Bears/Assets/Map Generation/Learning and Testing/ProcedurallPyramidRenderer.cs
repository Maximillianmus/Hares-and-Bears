using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedurallPyramidRenderer : MonoBehaviour
{


    [Tooltip("A mesh to extrude the pyramids from")]
    [SerializeField] private Mesh sourceMesh = default;
    [Tooltip("The pyramid geometry creating compute shader")]
    [SerializeField] private ComputeShader pyramidComputeShader = default;
    [Tooltip("The material to render the pyramid mesh")]
    [SerializeField] private Material material = default;
    [Tooltip("Wheather the pyramid should cast shadows")]
    [SerializeField] private float pyramidHeight = 1;
    [Tooltip("speed of animation")]
    [SerializeField] private float animationFrequency = 1;


    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct SourceVertex
    {
        public Vector3 position;
        public Vector2 uv;
    }

    private bool initialized;
    private ComputeBuffer sourceVertBuffer;
    private ComputeBuffer sourceTriBuffer;
    private ComputeBuffer drawBuffer;
    private ComputeBuffer argsBuffer;
    private int idPyramidKernel;
    private int dispatchSize;
    private Bounds localBounds;

    private const int SOURCE_VERT_STRIDE = sizeof(float) * (3 + 2);
    private const int SOURCE_TRI_STRIDE = sizeof(int);
    private const int DRAW_STRIDE = sizeof(float) * (3 +(3+2)* 3);
    private const int ARGS_STRIDE = sizeof(int) * 4;


    private void OnEnable()
    {
        if (initialized)
        {
            OnDisable();
        }
        initialized = true;

        Vector3[] positions = sourceMesh.vertices;
        Vector2[] uvs = sourceMesh.uv;
        int[] tris = sourceMesh.triangles;

        SourceVertex[] vertices = new SourceVertex[positions.Length];
        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new SourceVertex()
            {
                position = positions[i],
                uv = uvs[i],

            };
        }
        int numTriangles = tris.Length / 3;


        sourceVertBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceVertBuffer.SetData(vertices);
        sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceTriBuffer.SetData(tris);
        drawBuffer = new ComputeBuffer(numTriangles * 3, DRAW_STRIDE, ComputeBufferType.Append);
        drawBuffer.SetCounterValue(0);
        argsBuffer = new ComputeBuffer(1, ARGS_STRIDE, ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(new int[] { 0, 1, 0, 0 });


        idPyramidKernel = pyramidComputeShader.FindKernel("Main");

        pyramidComputeShader.SetBuffer(idPyramidKernel, "_SourceVertices", sourceVertBuffer);
        pyramidComputeShader.SetBuffer(idPyramidKernel, "_SourceTriangles", sourceTriBuffer);
        pyramidComputeShader.SetBuffer(idPyramidKernel, "_DrawTriangles", drawBuffer);
        pyramidComputeShader.SetInt("_NumSourceTriangles", numTriangles);

        material.SetBuffer("_DrawTriangles", drawBuffer);

        pyramidComputeShader.GetKernelThreadGroupSizes(idPyramidKernel, out uint threadGroupSize, out _, out _);
        dispatchSize = Mathf.CeilToInt((float)numTriangles / threadGroupSize);

        localBounds = sourceMesh.bounds;
        localBounds.Expand(pyramidHeight);
    }

    private void OnDisable()
    {
        if (initialized)
        {
            sourceVertBuffer.Release();
            sourceTriBuffer.Release();
            drawBuffer.Release();
            argsBuffer.Release();
        }
        initialized = false;
    }

    public Bounds TransformBounds(Bounds boundsOS)
    {
        var center = transform.TransformPoint(boundsOS.center);

        var extents = boundsOS.extents;
        var axisX = transform.TransformVector(extents.x, 0, 0);
        var axisy = transform.TransformVector(0, extents.y, 0);
        var axisZ = transform.TransformVector(0, 0, extents.z);


        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisy.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisy.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisy.z) + Mathf.Abs(axisZ.z);

        return new Bounds { center = center, extents = extents };
    }


    private void LateUpdate()
    {
        drawBuffer.SetCounterValue(0);

        Bounds bounds = TransformBounds(localBounds);


        pyramidComputeShader.SetMatrix("_LocalWorld", transform.localToWorldMatrix);
        pyramidComputeShader.SetFloat("pyramidHeight", pyramidHeight * Mathf.Sin(animationFrequency * Time.timeSinceLevelLoad));

        pyramidComputeShader.Dispatch(idPyramidKernel, dispatchSize, 1, 1);

        ComputeBuffer.CopyCount(drawBuffer, argsBuffer, 0);

        Graphics.DrawProceduralIndirect(material, bounds, MeshTopology.Triangles, argsBuffer, 0, null , null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
    }
}
