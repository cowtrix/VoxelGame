using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class test : MonoBehaviour
{
    public Vector3 Offset;
    public Vector3 Size;
    public EVoxelDirection Direction;
    public MeshFilter Filter;

    public Mesh Mesh;

    void Update()
    {
        var data = new IntermediateVoxelMeshData();
        VoxelMeshUtility.GetPlane(Offset, Size, Direction, data);
        if(!Mesh)
		{
            Mesh = new Mesh();
		}
        Mesh.SetVertices(data.Vertices);
        Mesh.SetColors(data.Color1);
        Mesh.SetTriangles(data.Triangles, 0);
        Mesh.SetUVs(0, data.UV1);
        Mesh.SetUVs(1, data.UV2);
        Mesh.RecalculateNormals();
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(Mesh);
        meshSimplifier.SimplifyMesh(1f);
        Mesh = meshSimplifier.ToMesh();
        Filter.sharedMesh = Mesh;
    }
}
