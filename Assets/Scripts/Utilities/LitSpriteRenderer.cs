using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[ExecuteAlways]
public class LitSpriteRenderer : ExtendedMonoBehaviour
{
    
    public Sprite Sprite;
    public Material Material;
    [ColorUsage(false, true)]
    public Color Color = Color.white;
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;

    private MaterialPropertyBlock m_propertyBlock;

    private Mesh GetQuadMesh()
    {
        var mesh = Resources.Load<Mesh>("SpriteQuad");
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.SetVertices(new[]
            {
                new Vector3(-.5f, -.5f),
                new Vector3(-.5f, .5f),
                new Vector3(.5f, -.5f),
                new Vector3(.5f, .5f),
            });
            mesh.SetUVs(0, new[]{
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(1, 1),
            });
            mesh.SetTriangles(new[]
            {
                0, 1, 2,
                1, 3, 2,
            }, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Resources/SpriteQuad.asset");
#endif
        }
        return mesh;
    }

    private Material GetSpriteMaterial()
    {
        if (Material)
        {
            return Material;
        }
        return Resources.Load<Material>("SpriteLitMaterial");
    }

    [ContextMenu("Invalidate")]
    public void Invalidate()
    {
        if (!MeshRenderer || MeshRenderer.Equals(null))
        {
            MeshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
        }
        MeshRenderer.sharedMaterial = GetSpriteMaterial();

        if (!MeshFilter || MeshFilter.Equals(null))
        {
            MeshFilter = gameObject.GetOrAddComponent<MeshFilter>();
        }
        MeshFilter.sharedMesh = GetQuadMesh();
        OnValidate();
    }

    private void Start()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (!MeshRenderer)
        {
            return;
        }
        if(m_propertyBlock == null)
        {
            m_propertyBlock = new MaterialPropertyBlock();
            MeshRenderer.GetPropertyBlock(m_propertyBlock);
        }
        m_propertyBlock.SetTexture("MainTex", Sprite.texture);
        var w = Sprite.texture.width;
        var h = Sprite.texture.height;
        m_propertyBlock.SetVector("TexRect", new Vector4(Sprite.textureRect.x / w, Sprite.textureRect.y / h, Sprite.textureRect.width / w, Sprite.textureRect.height / h));
        m_propertyBlock.SetColor("_Color", Color);
        MeshRenderer.SetPropertyBlock(m_propertyBlock);
    }
}
