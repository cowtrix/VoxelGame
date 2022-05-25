using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Generation/Instrument Collection")]
public class InstrumentCollection : ScriptableObject
{
    public List<AudioClip> Notes;
	public string DisplayName = "Untitled Instrument";
}
