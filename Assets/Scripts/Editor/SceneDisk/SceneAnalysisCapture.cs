using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneDisk
{
    public interface IAnalyzer
    {
        AnalysisMessage Analyze(Object obj);
    }

    public class AnalysisMessage
    {
        public string Message;
    }

    public class ObjectData
    {
        public bool Expanded;
        public List<AnalysisMessage> Messages = new List<AnalysisMessage>();

        public ObjectData(UnityEngine.Object obj)
        {
            foreach (var m in SceneDisk.AllAnalyzers)
            {
                var message = m.Analyze(obj);
                if (message != null)
                {
                    Messages.Add(message);
                }
            }
        }
    }

    public class ScriptableObjectData : ObjectData
    {
        public ScriptableObject ScriptableObject;

        public ScriptableObjectData(ScriptableObject so) : base(so)
        {
            ScriptableObject = so;
        }
    }

    public class GameObjectData : ObjectData
    {
        public GameObject GameObject;
        public List<GameObjectData> Children = new List<GameObjectData>();
        public int TotalMessageCount => Messages.Count + Children.Sum(c => c.TotalMessageCount);
        public GameObjectData(GameObject go) : base(go)
        {
            GameObject = go;
            foreach(Transform child in go.transform)
            {
                Children.Add(new GameObjectData(child.gameObject));
            }
        }
    }

    public class SceneAnalysisCapture
    {
        public List<SceneData> Scenes = new List<SceneData>();
        public List<ScriptableObjectData> ScriptableObjects = new List<ScriptableObjectData>();

        public class SceneData
        {
            public bool Expanded;
            public Scene Scene;
            public List<GameObjectData> GameObjects = new List<GameObjectData>();
        }
    }
}
