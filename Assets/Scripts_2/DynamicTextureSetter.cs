using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicTextureSetter : MonoBehaviour
{
    protected static MaterialPropertyBlock m_materialPropertyBlock;

	protected MeshRenderer Renderer => GetComponent<MeshRenderer>();

	public Color AlbedoTint = Color.white;
    public Texture2D Albedo;
	public float Smoothness;
	public float Metallic;

	private void OnWillRenderObject()
	{
		if (!Albedo)
		{
			return;
		}
		if(m_materialPropertyBlock == null)
		{
			m_materialPropertyBlock = new MaterialPropertyBlock();
		}
		Renderer.GetPropertyBlock(m_materialPropertyBlock);
		m_materialPropertyBlock.SetColor("_BaseColor", AlbedoTint);
		m_materialPropertyBlock.SetTexture("_BaseMap", Albedo);
		m_materialPropertyBlock.SetFloat("_Smoothness", Smoothness);
		m_materialPropertyBlock.SetFloat("_Metallic", Metallic);
		Renderer.SetPropertyBlock(m_materialPropertyBlock);
	}
}
