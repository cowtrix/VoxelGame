using Common;
using System.Linq;
using UnityEngine;

namespace SceneDisk
{
    public class MissingLODAnalyzer : IAnalyzer
    {
        public AnalysisMessage Analyze(Object obj)
        {
            if (!(obj is GameObject go))
            {
                return null;
            }
            var renderer = go.GetComponent<Renderer>();
            if (!renderer)
            {
                return null;
            }
            var lodGroup = go.GetComponent<LODGroup>();
            if (!lodGroup)
                lodGroup = go.transform.GetComponentInAncestors<LODGroup>();
            if (!lodGroup || !lodGroup.GetLODs().Any(l => l.renderers.Contains(renderer)))
            {
                return new AnalysisMessage
                {
                    Message = "LODGroup missing"
                };
            }
            return null;
        }
    }

    public class InSceneMeshAnalyzer : IAnalyzer
    {
        public AnalysisMessage Analyze(Object obj)
        {
            if (!(obj is GameObject go))
            {
                return null;
            }
            var filter = go.GetComponent<MeshFilter>();
            if (filter && filter.sharedMesh && string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(filter.sharedMesh)))
            {
                return new AnalysisMessage
                {
                    Message = "MeshFilter references in-scene mesh"
                };
            }
            return null;
        }
    }
}
