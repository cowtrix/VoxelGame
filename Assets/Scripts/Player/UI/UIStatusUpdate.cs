using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIStatusUpdate : MonoBehaviour
{
	public List<UIStateUpdateEntry> Entries => GetComponentsInChildren<UIStateUpdateEntry>().ToList();

	public List<UIStateUpdateEntry> ActiveEntries { get; private set; } = new List<UIStateUpdateEntry>();

	private void Update()
	{
		ActiveEntries = Entries.Where(e => e.Active).ToList();
	}
}
