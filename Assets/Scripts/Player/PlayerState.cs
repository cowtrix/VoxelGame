using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
	[Serializable]
	public class State
	{
		public uint Credits = 100;
		public bool HeadlightOn;
	}

	public State CurrentState;
}
