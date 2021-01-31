using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	public Transform Cursor;
	public Transform HeldItemContainer;
	public Text VerbText;
	public Text HeldItemName;

	public List<RectTransform> HeldItemTraits;
}
