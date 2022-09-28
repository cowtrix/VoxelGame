using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Generation
{
    public class PileGenerator : ExtendedMonoBehaviour
    {
        [Serializable]
        public class TransformInfo
        {
            public GameObject Prefab;
            public Matrix4x4 Matrix;
            public Transform Instance;
        }
        public List<GameObject> Prefabs;
        public List<TransformInfo> Data;
        public Vector3 SpawnPosition;
        public Vector2 SpawnSize;
        public float DefaultMass = 10;
        public float DefaultDrag = 2;

        [ContextMenu("Bake")]
        public void Bake()
        {
            foreach(var info in Data)
            {
                info.Matrix = info.Instance.localToWorldMatrix;
            }
        }

        [ContextMenu("Generate Piece")]
        public void GeneratePiece()
        {
            var prefab = Prefabs.Random();
            var newPiece = Instantiate(prefab);
            newPiece.isStatic = false;
            var rb = newPiece.AddComponent<Rigidbody>();
            rb.mass = DefaultMass;
            rb.drag = DefaultDrag;
            newPiece.transform.SetParent(transform, false);
            newPiece.AddComponent<GravityRigidbody>();
            rb.position = transform.localToWorldMatrix.MultiplyPoint3x4(SpawnPosition)
                + new Vector3(UnityEngine.Random.Range(-SpawnSize.x, SpawnSize.x), 0, UnityEngine.Random.Range(-SpawnSize.y, SpawnSize.y));
            var newInfo = new TransformInfo
            {
                Prefab = prefab,
                Instance = newPiece.transform,
            };
            Data.Add(newInfo);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(SpawnPosition, new Vector3(SpawnSize.x * 2, 0, SpawnSize.y * 2));
        }
    }
}