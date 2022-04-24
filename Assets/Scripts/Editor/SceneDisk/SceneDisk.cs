using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace SceneDisk
{
    public static class SceneDisk
    {
        private static List<Type> GetAllTypesImplementingInterface(this Type interfaceType, Type assemblyType = null)
        {
            if (!interfaceType.IsInterface)
            {
                throw new Exception("Must be an interface type!");
            }
            var result = new List<Type>();

            var assembly = Assembly.GetAssembly(assemblyType ?? interfaceType);
            var allTypes = assembly.GetTypes();
            for (var i = 0; i < allTypes.Length; i++)
            {
                if (allTypes[i].GetInterfaces().Contains(interfaceType))
                {
                    result.Add(allTypes[i]);
                }
            }

            return result;
        }

        public static IEnumerable<IAnalyzer> AllAnalyzers => GetAllTypesImplementingInterface(typeof(IAnalyzer))
            .Select(a => Activator.CreateInstance(a) as IAnalyzer);

        public static SceneAnalysisCapture AnalyzeScenes()
        {
            var capture = new SceneAnalysisCapture();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var allGameObjects = scene.GetRootGameObjects();
                var allScriptableObjects = UnityEngine.Object.FindObjectsOfType<ScriptableObject>();
                var sceneData = new SceneAnalysisCapture.SceneData
                {
                    Scene = scene,
                    GameObjects = allGameObjects
                        .Select(go => new GameObjectData(go))
                        .ToList(),
                };
                capture.Scenes.Add(sceneData);
            }
            return capture;
        }
    }
}
