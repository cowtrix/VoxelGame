using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Generation
{
    public class DynamicGeneratorRoot : ExtendedMonoBehaviour 
    {
        public bool GenerateOnStart;

        private void Start()
        {
            if (GenerateOnStart)
            {
                Generate();
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            var nodes = new List<DynamicGeneratorNode>(GetComponentsInChildren<DynamicGeneratorNode>());
            var childToParent = new Dictionary<DynamicGeneratorNode, DynamicGeneratorNode>();
            foreach(var node in nodes)
            {
                if (!node.Children.Any())
                {
                    continue;
                }
                foreach(var child in node.Children)
                {
                    childToParent.Add(child, node);
                }
            }
            foreach(var node in nodes)
            {
                if (childToParent.ContainsKey(node))
                {
                    continue;
                }
                node.Seed = Random.Range(int.MinValue, int.MaxValue);
                node.Generate();
            }
        }
    }
}