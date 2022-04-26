using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

public class GrassSpawner : ExtendedMonoBehaviour
{
    public string GrassMeshDirectory = "Assets/Meshes/Grass";

    [Serializable]
    public class GrassPatch
    {
        public MeshFilter Filter, LOD;
        public Mesh Mesh;

        public void Regenerate(GrassSpawner spawner, List<Mesh> planeMeshes, Bounds spawnBounds, float amountScale)
        {
            if (!Filter)
            {
                Filter = new GameObject("GrassPatch").AddComponent<MeshFilter>();
                Filter.transform.SetParent(spawner.transform);
                Filter.transform.localPosition = spawnBounds.min;
            }
            var renderer = Filter.gameObject.GetOrAddComponent<MeshRenderer>();
            renderer.sharedMaterial = spawner.GrassMaterial;
            renderer.gameObject.isStatic = true;
            renderer.gameObject.layer = spawner.gameObject.layer;
            var combineInstances = new List<CombineInstance>();
            var density = spawnBounds.size.x * spawnBounds.size.z * spawner.Density * amountScale;

            var collisionGridWidth = Mathf.CeilToInt(spawnBounds.size.x);
            var collisionGridHeight = Mathf.CeilToInt(spawnBounds.size.z);
            var collisionMask = new bool[collisionGridWidth, collisionGridHeight];
            for (var x = 0; x < collisionGridWidth; x++)
            {
                for (var y = 0; y < collisionGridHeight; y++)
                {
                    var castDistance = 3;
                    var castPos = spawner.VoxelRenderer.transform.localToWorldMatrix.MultiplyPoint3x4(spawnBounds.min
                        + new Vector3(((x + .5f) / (float)collisionGridWidth) * spawnBounds.size.x, castDistance, ((y + .5f) / (float)collisionGridHeight) * spawnBounds.size.z));

                    if (!Physics.Raycast(castPos, Vector3.down, out var hit, castDistance + 1, spawner.HitCheckMask))
                    {
                        Debug.DrawLine(castPos, castPos + Vector3.down * (castDistance + 1), Color.red, 20);
                        continue;
                    }
                    if (hit.collider.gameObject != spawner.VoxelRenderer.gameObject)
                    {
                        Debug.DrawLine(castPos, hit.point, Color.magenta, 20);
                        continue;
                    }
                    collisionMask[x, y] = true;
                    /*if (collisionMask[x, y])
                        Debug.Log($"Hit {hit.collider}", hit.collider);
                    */
                }
            }

            for (var i = 0; i < density; ++i)
            {
                var position = new Vector3(UnityEngine.Random.value * spawnBounds.size.x, 0, UnityEngine.Random.value * spawnBounds.size.z);
                var gridPoint = (Mathf.Clamp(Mathf.FloorToInt((position.x / (spawnBounds.size.x * .95f)) * collisionGridWidth), 0, collisionGridWidth - 1), Mathf.Clamp(Mathf.FloorToInt((position.z / (spawnBounds.size.z * .95f)) * collisionGridHeight), 0, collisionGridHeight - 1));

                if (!collisionMask[gridPoint.Item1, gridPoint.Item2])
                {
                    //DebugHelper.DrawPoint(renderer.transform.localToWorldMatrix.MultiplyPoint3x4(position), .1f, Color.red, 20);
                    continue;
                }
                //DebugHelper.DrawPoint(renderer.transform.localToWorldMatrix.MultiplyPoint3x4(position), .1f, Color.green, 20);
                var newInstance = new CombineInstance
                {
                    mesh = planeMeshes.Random(),
                    transform = Matrix4x4.TRS(
                        position,
                        Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0),
                        Vector3.one * UnityEngine.Random.Range(spawner.Scale.x, spawner.Scale.y)),
                };
                combineInstances.Add(newInstance);
            }
            bool newAsset = false;
            if (!Mesh)
            {
                Mesh = new Mesh();
                newAsset = true;
            }
            try
            {
                Mesh.CombineMeshes(combineInstances.ToArray(), true, true);
            }
            catch (ArgumentException ex)
            {
                Debug.LogException(ex, Filter.gameObject);
            }
            Filter.sharedMesh = Mesh;
            Mesh.TrySetDirty();

            if (newAsset)
            {
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(Mesh, $"{spawner.GrassMeshDirectory}/{Mathf.Abs(Mesh.GetHashCode())}_GrassMesh.asset");
#endif
            }

            // Create LOD quad
            if (!LOD)
            {
                LOD = new GameObject("GrassLod").AddComponent<MeshFilter>();
            }
            LOD.transform.SetParent(Filter.transform);
            LOD.transform.Reset();
            LOD.transform.localPosition = Vector3.up * .02f;
            LOD.gameObject.isStatic = true;
            LOD.gameObject.layer = spawner.gameObject.layer;
            var quadRenderer = LOD.gameObject.GetOrAddComponent<MeshRenderer>();
            quadRenderer.sharedMaterial = spawner.LODMaterial;
            LOD.sharedMesh = spawner.LODMesh;
            LOD.transform.localScale = spawnBounds.size;

            var lodGroup = Filter.gameObject.GetOrAddComponent<LODGroup>();
            var lods = new LOD[]
            {
                new LOD
                {
                    renderers = new[]{ renderer },
                    screenRelativeTransitionHeight = spawner.ScreenRelativeTransitionHeight * spawnBounds.size.x,
                },
                new LOD
                {
                    renderers = new[]{ quadRenderer },
                    screenRelativeTransitionHeight = 0,
                }
            };
            lodGroup.SetLODs(lods);
        }
    }

    public float TextureCellSize;
    public Material GrassMaterial;
    public Texture2D VegetationTextures;
    public VoxelRenderer VoxelRenderer;
    public Vector2 Scale = new Vector2(.1f, .2f);
    public float Density = 1;
    public float ScreenRelativeTransitionHeight = .01f;
    public LayerMask HitCheckMask;
    public Mesh LODMesh;
    public Material LODMaterial;

    public List<GrassPatch> GrassPatches = new List<GrassPatch>();

    private Mesh GetLODMesh()
    {
        var m = new Mesh();
        m.SetVertices(new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
        });
        m.SetTriangles(new[]
        {
            0, 1, 2,
            1, 3, 2,
        }, 0);
        m.SetNormals(new[]
        {
            Vector3.up, Vector3.up, Vector3.up, Vector3.up,
        });
        return m;
    }

    private List<Mesh> GetDoubleSidedPlaneMeshes()
    {
        var meshes = new List<Mesh>();
        var step = TextureCellSize / VegetationTextures.width;
        var stepPixels = Mathf.FloorToInt(step * VegetationTextures.width);
        for (var x = 0f; x < 1; x += step)
        {
            for (var y = 0f; y < 1; y += step)
            {
                var pixels = VegetationTextures.GetPixels(Mathf.FloorToInt(x * VegetationTextures.width), Mathf.FloorToInt(y * VegetationTextures.height), stepPixels, stepPixels);
                if (pixels.Sum(p => p.a) <= 0)
                {
                    continue;
                }

                var m = new Mesh();
                m.SetVertices(new[]
                {
                    new Vector3(-.5f, 0, 0),
                    new Vector3(-.5f, 1, 0),
                    new Vector3(.5f, 0, 0),
                    new Vector3(.5f, 1, 0),
                });
                m.SetTriangles(new[]
                {
                    0, 1, 2,
                    1, 3, 2,
                }, 0);
                m.SetUVs(0, new[]
                {
                    new Vector2(x, y),
                    new Vector2(x, y + step),
                    new Vector2(x + step, y),
                    new Vector2(x + step, y + step),
                });
                m.SetColors(new[]
                {
                    Color.white, Color.black,
                    Color.white, Color.black,
                });
                m.RecalculateBounds();
                m.RecalculateNormals();
                meshes.Add(m);
            }
        }
        return meshes;
    }


    [ContextMenu("Regenerate")]
    public void Regenerate()
    {
        if (!LODMesh)
        {
            LODMesh = GetLODMesh();
        }
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(LODMesh)))
        {
            UnityEditor.AssetDatabase.CreateAsset(LODMesh, $"{GrassMeshDirectory}/LODMesh_GrassMesh.asset");
        }
#endif
        var planeMeshes = GetDoubleSidedPlaneMeshes();
        var voxels = VoxelRenderer.Mesh.Voxels;
        var coordinates = voxels.Select(v => v.Key).ToList();
        var updatedPatches = new HashSet<GrassPatch>();
        foreach (var vox in voxels)
        {
            if (coordinates.CollideCheck(vox.Key + new VoxelCoordinate(0, 1, 0, vox.Key.Layer), out _))
            {
                continue;
            }
            var voxBounds = vox.Key.ToBounds();
            var topSurfaceBounds = new Bounds(voxBounds.center + new Vector3(0, voxBounds.extents.y, 0), voxBounds.size.Flatten());
            var surface = vox.Value.Material.GetSurface(EVoxelDirection.YPos);
            if (surface.TextureFade == 1)
            {
                continue;
            }
            var patch = GrassPatches.SingleOrDefault(p => Vector3.Distance(p.Filter.transform.localPosition, topSurfaceBounds.min) < .1f);
            if (patch == null)
            {
                patch = new GrassPatch();
                GrassPatches.Add(patch);
            }
            patch.Regenerate(this, planeMeshes, topSurfaceBounds, Mathf.Clamp01(1 - surface.TextureFade));
            updatedPatches.Add(patch);
        }
        foreach (var p in GrassPatches.Where(p => !updatedPatches.Contains(p)).ToList())
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(p.Mesh));
#endif
            p.Filter.gameObject.SafeDestroy();
            GrassPatches.Remove(p);
        }
    }
}
