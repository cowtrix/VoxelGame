using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul.Utilities;

public class ObjectGenerator : MonoBehaviour
{
    [Serializable]
    public class Entry
	{
        public enum EntryType
		{
            SingleObject,
            Collection,
            SubGenerator,
		}
        public string Name;
        public EntryType Type;
        public UnityEngine.Object Target;
    }

    public List<Entry> Entries;

}
