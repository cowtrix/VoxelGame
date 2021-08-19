using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InstrumentCollection : ScriptableObject
{
    public List<AudioClip> Notes;
	public string DisplayName = "Untitled Instrument";
}
