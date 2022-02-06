using Common;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation
{
	public class LinearObjectGenerator : ExtendedMonoBehaviour, IGenerationCallback
	{
		[MinMax(0, 32)]
		public Vector2Int Count;
		public Vector3 InitialOffset, InitialRotation;
		public Vector3 StepOffset, StepRotation;
		public List<GameObject> Objects;
		public GameObject Cap;

		[HideInInspector]
		public List<GameObject> InstantiatedObjects = new List<GameObject>();
		public System.Guid LastGenerationID { get; set; }

		public void Generate(ObjectGenerator objectGenerator)
		{
			Clear();
			var count = Random.Range(Count.x, Count.y);
			var offset = InitialOffset;
			var rotation = Quaternion.Euler(InitialRotation);
			for (var i = 0; i < count; ++i)
			{
				var pick = Objects.Random();
				if(Cap && i == count - 1)
				{
					pick = Cap;
				}
#if UNITY_EDITOR
				var newObj = UnityEditor.PrefabUtility.InstantiatePrefab(pick, transform) as GameObject;
#else
				var newObj = Prefab Instantiate(Objects.Random(), transform);
#endif
				InstantiatedObjects.Add(newObj);
				newObj.transform.localPosition = offset;
				newObj.transform.localRotation = rotation;
				offset += StepOffset;
				rotation *= Quaternion.Euler(StepRotation);
			}
		}

		public void Clear()
		{
			foreach (var g in InstantiatedObjects)
			{
				g.SafeDestroy();
			}
			InstantiatedObjects.Clear();
		}
	}
}