using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public class LevelBakeUtility
    {
        static int BakeLevelsFromCLI()
        {
            AssetDatabase.Refresh();

            var scenePaths = System.Environment.GetCommandLineArgs()
                .Where(arg => arg.StartsWith("Assets/"))
                .ToArray();
            BakeLevels(scenePaths);

            return 0;
        }

        [MenuItem("Tools/Bake Level")]
        public static void BakeOpenLevels()
        {
            var openScenes = new string[EditorSceneManager.sceneCount];
            for(int i = 0; i < openScenes.Length; i++)
            {
                openScenes[i] = EditorSceneManager.GetSceneAt(i).path;
            }
            BakeLevels(openScenes);
        }

        public static void BakeLevels(params string[] levelPaths)
        {
            Debug.Log($"Baking Levels:\n{string.Join('\n', levelPaths)}");
            Debug.Log("Triggering lightmap bake...");
            Lightmapping.BakeMultipleScenes(levelPaths);

            Debug.Log("Triggering navmesh bake...");
            NavMeshBuilder.BuildNavMeshForMultipleScenes(levelPaths);

            //Debug.Log("Triggering occlusion bake...");
            //StaticOcclusionCulling.Compute();

            Debug.Log("Bake finished, saving.");
            AssetDatabase.SaveAssets();
        }
    }
}