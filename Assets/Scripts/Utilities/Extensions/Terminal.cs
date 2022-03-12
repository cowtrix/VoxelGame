using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common
{
	public class Terminal : EditorWindow
	{
		[Serializable]
		public struct TerminalEntry
		{
			public string Value;
			public DateTime Timestamp;
		}

		[MenuItem("Tools/Terminal")]
		public static void OpenWindow()
		{
			var w = GetWindow<Terminal>();
			w.titleContent = new GUIContent("Terminal");
		}

		public int HistoryLength
		{
			get => EditorPrefs.GetInt("Common.Terminal.HistoryLength", 0);
			set => EditorPrefs.SetInt("Common.Terminal.HistoryLength", value);
		}

		public TerminalEntry? GetEntry(int index)
		{
			if(m_historyCache.TryGetValue(index, out var entry))
			{
				return entry;
			}
			var key = $"Common.Terminal.History[{index}]";
			if (!EditorPrefs.HasKey(key))
			{
				return null;
			}
			entry = JsonUtility.FromJson<TerminalEntry>(EditorPrefs.GetString(key));
			m_historyCache[index] = entry;
			return entry;
		}

		public void ExecuteCommand(string cmd)
		{
			var newCommand = new TerminalEntry
			{
				Value = cmd,
				Timestamp = DateTime.Now,
			};
			AddEntry(newCommand);
		}

		public void Log(string msg)
		{
			var newCommand = new TerminalEntry
			{
				Value = msg,
				Timestamp = DateTime.Now,
			};
			AddEntry(newCommand);
		}

		private void AddEntry(TerminalEntry entry)
		{
			EditorPrefs.SetString($"Common.Terminal.History[{HistoryLength}]", JsonUtility.ToJson(entry));
			m_historyCache[HistoryLength] = entry;
			HistoryLength++;
		}

		private Dictionary<int, TerminalEntry> m_historyCache = new Dictionary<int, TerminalEntry>();
		private string m_currentInput;
		private Vector2 m_scroll;

		private void OnGUI()
		{
			m_scroll = EditorGUILayout.BeginScrollView(m_scroll, false, true, GUILayout.ExpandHeight(true));
			for(var i = 0; i < HistoryLength; i++)
			{
				var entry = GetEntry(i);
				EditorGUILayout.BeginHorizontal("Box");
				EditorGUILayout.SelectableLabel(entry.Value.Timestamp.ToShortTimeString(), GUILayout.Width(100));
				EditorGUILayout.SelectableLabel(entry.Value.Value);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			m_currentInput = EditorGUILayout.TextField(m_currentInput, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
			if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return && !string.IsNullOrEmpty(m_currentInput))
			{
				ExecuteCommand(m_currentInput);
				m_currentInput = "";
			}
		}
	}
}