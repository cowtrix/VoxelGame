using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace NodeCanvas.DialogueTrees.UI.Examples
{
	public class DialogueUGUI : MonoBehaviour
	{
		[System.Serializable]
		public class SubtitleDelays
		{
			public float characterDelay = 0.05f;
			public float sentenceDelay = 0.5f;
			public float commaDelay = 0.1f;
			public float finalDelay = 1.2f;
		}

		//Options...
		[Header("Input Options")]
		public bool waitForInput;

		//Group...
		[Header("Subtitles")]
		public RectTransform subtitlesGroup;
		public Text actorSpeech;
		public Text actorName;
		public RectTransform waitInputIndicator;
		public SubtitleDelays subtitleDelays = new SubtitleDelays();
		public List<AudioClip> typingSounds;
		private AudioSource playSource;

		private HashSet<string> m_history = new HashSet<string>();

		public Vector3 Offset;

		//Group...
		[Header("Multiple Choice")]
		public RectTransform optionsGroup;
		public Button optionButton;
		private List<Button> m_cachedButtons = new List<Button>();
		private bool isWaitingChoice;
		private bool m_printingSubtitles;
		private bool m_waitingForSkip;

		public GameObject SkipText;

		private AudioSource _localSource;
		private AudioSource localSource
		{
			get 
			{
				if (!_localSource)
				{
					_localSource = gameObject.AddComponent<AudioSource>();
					_localSource.spatialBlend = 1;
				}
				return _localSource;
			}
		}

		void Awake() { Hide(); }

		public void OnDialogueStarted(DialogueTree dlg)
		{
			var actor = dlg.GetActorReferenceByName(DialogueTree.SELF_NAME);
			transform.parent.SetParent(actor.GetDialogueContainer());
			transform.parent.localPosition = Vector3.zero;
			transform.parent.localRotation = Quaternion.identity;
		}

		public void OnDialoguePaused(DialogueTree dlg)
		{
			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
			StopAllCoroutines();
			if (playSource != null) playSource.Stop();
		}

		public void OnDialogueFinished(DialogueTree dlg)
		{
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(FinishDialogDelayed(dlg));
			}
		}

		private void Update()
		{
			transform.localPosition = Offset;
			SkipText.SetActive(m_printingSubtitles && !m_waitingForSkip);
		}

		public void Hide()
		{
			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
			optionButton.gameObject.SetActive(false);
			waitInputIndicator.gameObject.SetActive(false);
		}

		IEnumerator FinishDialogDelayed(DialogueTree dlg)
		{
			yield return new WaitForSeconds(subtitleDelays.finalDelay);
			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
			foreach (var button in m_cachedButtons)
			{
				button.gameObject.SetActive(false);
			}
			StopAllCoroutines();
			if (playSource != null) playSource.Stop();
		}

		public void Skip()
		{
			if (!m_printingSubtitles)
			{
				return;
			}
			m_waitingForSkip = true;
		}

		public void OnSubtitlesRequest(SubtitlesRequestInfo info)
		{
			m_printingSubtitles = true;
			StartCoroutine(Internal_OnSubtitlesRequestInfo(info));
		}

		IEnumerator Internal_OnSubtitlesRequestInfo(SubtitlesRequestInfo info)
		{
			optionsGroup.gameObject.SetActive(false);
			var text = info.statement.text;
			var audio = info.statement.audio;
			var actor = info.actor;

			subtitlesGroup.gameObject.SetActive(true);
			actorSpeech.text = "";

			actorName.text = actor.DisplayName;
			if (audio != null)
			{
				var actorSource = actor.transform != null ? actor.transform.GetComponent<AudioSource>() : null;
				playSource = actorSource != null ? actorSource : localSource;
				playSource.clip = audio;
				playSource.Play();
				actorSpeech.text = text;
				var timer = 0f;
				while (timer < audio.length)
				{
					if (m_waitingForSkip)
					{
						playSource.Stop();
						break;
					}
					timer += Time.deltaTime;
					yield return null;
				}
			}

			if (audio == null)
			{
				var tempText = "";
				for (int i = 0; i < text.Length; i++)
				{
					if (m_waitingForSkip)
					{
						actorSpeech.text = text;
						yield return null;
						break;
					}

					if (subtitlesGroup.gameObject.activeSelf == false)
					{
						yield break;
					}

					char c = text[i];
					tempText += c;
					yield return StartCoroutine(DelayPrint(subtitleDelays.characterDelay));
					PlayTypeSound();
					actorSpeech.text = tempText;
					if (c == '.' || c == '!' || c == '?')
					{
						yield return StartCoroutine(DelayPrint(subtitleDelays.sentenceDelay));
						PlayTypeSound();
					}
					if (c == ',')
					{
						yield return StartCoroutine(DelayPrint(subtitleDelays.commaDelay));
						PlayTypeSound();
					}
				}

				yield return StartCoroutine(DelayPrint(subtitleDelays.finalDelay));
			}

			m_waitingForSkip = false;
			m_printingSubtitles = false;
			yield return null;
			info.Continue();
		}

		void PlayTypeSound()
		{
			if (typingSounds.Count > 0)
			{
				var sound = typingSounds[Random.Range(0, typingSounds.Count)];
				if (sound != null)
				{
					localSource.PlayOneShot(sound, Random.Range(0.6f, 1f));
				}
			}
		}

		IEnumerator DelayPrint(float time)
		{
			var timer = 0f;
			while (timer < time)
			{
				timer += Time.deltaTime;
				yield return null;
			}
		}

		public void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			StartCoroutine(MultipleChoiceRequest(info));
		}

		IEnumerator MultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			var ev = EventSystem.current;
			while (m_printingSubtitles)
			{
				yield return null;
			}
			optionsGroup.gameObject.SetActive(true);
			int i = 0;
			foreach (KeyValuePair<IStatement, int> pair in info.options)
			{
				Button btn;
				if (i < m_cachedButtons.Count)
				{
					btn = m_cachedButtons[i];
				}
				else
				{
					btn = (Button)Instantiate(optionButton);
					m_cachedButtons.Add(btn);
				}
				btn.gameObject.SetActive(true);
				btn.transform.SetParent(optionsGroup.transform, false);
				btn.GetComponentInChildren<Text>().text = pair.Key.text;

				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(() => { Finalize(info, pair.Key, pair.Value); });

				if (ev.currentSelectedGameObject == btn.gameObject)
				{
					ev.SetSelectedGameObject(null);
				}

				var cg = btn.GetComponent<CanvasGroup>();
				cg.alpha = m_history.Contains($"{info.actor.transform.GetInstanceID()}_{pair.Key}") ? .5f : 1f;
				i++;
			}
			for (var j = m_cachedButtons.Count; j > i; j--)
			{
				m_cachedButtons[i].gameObject.SetActive(false);
			}
			if (info.showLastStatement)
			{
				subtitlesGroup.gameObject.SetActive(true);
			}
			if (info.availableTime > 0)
			{
				StartCoroutine(CountDown(info));
			}
		}

		IEnumerator CountDown(MultipleChoiceRequestInfo info)
		{
			isWaitingChoice = true;
			var timer = 0f;
			while (timer < info.availableTime)
			{
				if (isWaitingChoice == false)
				{
					yield break;
				}
				timer += Time.deltaTime;
				SetMassAlpha(optionsGroup, Mathf.Lerp(1, 0, timer / info.availableTime));
				yield return null;
			}

			if (isWaitingChoice)
			{
				var last = info.options.Last();
				Finalize(info, last.Key, last.Value);
			}
		}

		void Finalize(MultipleChoiceRequestInfo info, IStatement statement, int index)
		{
			isWaitingChoice = false;
			SetMassAlpha(optionsGroup, 1f);
			optionsGroup.gameObject.SetActive(false);
			subtitlesGroup.gameObject.SetActive(false);
			m_printingSubtitles = false;
			foreach (var btn in m_cachedButtons)
			{
				btn.gameObject.SetActive(false);
			}
			m_history.Add($"{info.actor.transform.GetInstanceID()}_{statement}");
			info.SelectOption(index);
		}

		void SetMassAlpha(RectTransform root, float alpha)
		{
			foreach (var graphic in root.GetComponentsInChildren<CanvasRenderer>())
			{
				graphic.SetAlpha(alpha);
			}
		}
	}
}