using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Meshing;
using Common;
using Generation;
using System;

namespace Generation
{
    public abstract class DynamicGeneratorNode : ExtendedMonoBehaviour
    {
        public List<DynamicGeneratorNode> Children;
        [Seed]
        public int Seed;

        [ContextMenu("Generate")]
        public virtual void Generate()
        {
            GenerateInternal();
            foreach(var child in Children)
            {
                child.Seed = Seed;
                child.Generate();
            }
        }

        protected abstract void GenerateInternal();
    }
}