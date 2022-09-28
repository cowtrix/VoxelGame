using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Meshing;
using Common;
using Generation;
using System;
using Voxul.Utilities;

namespace Generation
{
    public abstract class DynamicGeneratorNode : ExtendedMonoBehaviour
    {
        public List<DynamicGeneratorNode> Children;
        [Seed]
        public int Seed;

        [ContextMenu("Randomize Seed")]
        public void RandomizeSeed()
        {
            Seed = UnityEngine. Random.Range(int.MinValue, int.MaxValue);
            this.TrySetDirty();
        }

        [ContextMenu("Generate")]
        public virtual void Generate()
        {
            GenerateInternal();
            if(Children == null)
            {
                return;
            }
            foreach(var child in Children)
            {
                child.Seed = Seed;
                child.Generate();
            }
        }

        protected abstract void GenerateInternal();
    }
}