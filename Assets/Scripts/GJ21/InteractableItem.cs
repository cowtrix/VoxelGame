using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractableItem : MonoBehaviour
{
	public virtual string Verb => "Use";
}
