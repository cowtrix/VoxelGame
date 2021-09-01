using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

public class ObjectArranger : ExtendedMonoBehaviour
{
	public TextAnchor Alignment;

	public GameObject[] Prefabs;
	public int Count;
	public Vector3 Offset;
	public Vector3 StepOffset = Vector3.one;
	public Vector3 Rotation;
	public Vector3 MinSize = Vector3.one, MaxSize = Vector3.one;
	public Gradient ColourTint;
	public List<Transform> Instances;

	Bounds GetBounds(GameObject obj)
	{
		var meshBounds = obj.GetComponent<MeshRenderer>().bounds;
		var mat = transform.worldToLocalMatrix;
		return new Bounds(mat.MultiplyPoint3x4(meshBounds.center), mat.MultiplyVector(meshBounds.size));
	}

	[ContextMenu("Rearrange")]
	public void Rearrange()
	{
		foreach(var i in Instances)
		{
			i?.gameObject.SafeDestroy();
		}
		Instances.Clear();
		var lastPoint = Vector3.zero;

		for (var i = 0; i < Count; ++i)
		{
			if (Instances.Count <= i)
			{
#if UNITY_EDITOR
				var newObj = UnityEditor.PrefabUtility.InstantiatePrefab(Prefabs.Random())
					as GameObject;
#else
				var newObj = Instantiate(Prefabs.Random());
#endif
				newObj.transform.SetParent(transform);
				Instances.Add(newObj.transform);
			}
			var instance = Instances[i];
			
			instance.transform.localRotation = Quaternion.Euler(Rotation);
			instance.transform.localScale = new Vector3(Random.Range(MinSize.x, MaxSize.x), Random.Range(MinSize.y, MaxSize.y), Random.Range(MinSize.z, MaxSize.z));

			var bounds = GetBounds(instance.gameObject);
			instance.transform.localPosition = lastPoint + bounds.extents + Offset;
			lastPoint += new Vector3(bounds.size.x * StepOffset.x, bounds.size.y * StepOffset.y, bounds.size.z * StepOffset.z);
			
			var tint = instance.GetComponent<VoxelColorTint>();
			if (tint)
			{
				tint.Color = ColourTint.Evaluate(Random.value);
				tint.Invalidate();
			}
		}
	}
}
