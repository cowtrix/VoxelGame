using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using TMPro;
using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using Actors.NPC.Player;
using Common;
using Interaction.Activities;
using Interaction;

namespace Actors
{
    public class DialogueUI : MonoBehaviour, IDialogueUI
    {
        [System.Serializable]
        public class SubtitleDelays
        {
            public float characterDelay = 0.05f;
            public float sentenceDelay = 0.5f;
            public float commaDelay = 0.1f;
            public float finalDelay = 1.2f;
        }

        public StringEvent onWordsSaidEvent => OnWordSaid;
        public NPCInteractable Self { get; private set; }
        public PlayerActor Player => CameraController.Instance.Actor as PlayerActor;

        //Options...
        [Header("Input Options")]
        public bool waitForInput;

        //Group...
        [Header("Subtitles")]
        public RectTransform subtitlesGroup;
        public TextMeshProUGUI actorSpeech;
        public TextMeshProUGUI actorName;
        public RectTransform waitInputIndicator;
        public StringEvent OnWordSaid;
        public SubtitleDelays subtitleDelays = new SubtitleDelays();

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
            Self = actor.transform.GetComponentInChildren<NPCInteractable>();
            actor.OnDialogueStarted(dlg, this);
            transform.parent.SetParent(actor.GetDialogueContainer());
            transform.parent.localPosition = Vector3.zero;
            transform.parent.localRotation = Quaternion.identity;
        }

        public void OnDialoguePaused(DialogueTree dlg)
        {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            StopAllCoroutines();
        }

        public void OnDialogueFinished(DialogueTree dlg)
        {
            Player.TryStopActivity(Self);
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
            var actor = info.actor;
            if (info.statement.takeFocus)
            {
                Player.TryStartActivity(Self);
            }
            else
            {
                Player.TryStopActivity(Self);
            }
            optionsGroup.gameObject.SetActive(false);
            var text = info.statement.text;

            subtitlesGroup.gameObject.SetActive(true);
            actorSpeech.text = "";

            //actorName.text = actor.DisplayName;

            var tempText = "";
            var wordSplit = text.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int wordCounter = 0; wordCounter < wordSplit.Length; wordCounter++)
            {
                var w = wordSplit[wordCounter];
                OnWordSaid?.Invoke(w);
                for (int i = 0; i < w.Length; i++)
                {
                    var c = w[i];
                    var strC = $"{w[i]}";
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

                    if (info.statement.translate)
                    {
                        strC = LanguageUtility.GetSpriteKeyForChar(c);
                    }
                    tempText += strC;
                    yield return StartCoroutine(DelayPrint(subtitleDelays.characterDelay));
                    actorSpeech.text = tempText;
                    if (c == '.' || c == '!' || c == '?')
                    {
                        yield return StartCoroutine(DelayPrint(subtitleDelays.sentenceDelay));
                    }
                    if (c == ',')
                    {
                        yield return StartCoroutine(DelayPrint(subtitleDelays.commaDelay));
                    }
                }
                tempText += ' ';
            }

            yield return StartCoroutine(DelayPrint(subtitleDelays.finalDelay));

            m_waitingForSkip = false;
            m_printingSubtitles = false;
            yield return null;
            info.Continue();
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
                btn.GetComponentInChildren<TMP_Text>().text = pair.Key.text;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => { Finalize(info, pair.Key, pair.Value); });

                if (ev.currentSelectedGameObject == btn.gameObject)
                {
                    ev.SetSelectedGameObject(null);
                }

                var cg = btn.GetComponent<CanvasGroup>();
                cg.alpha = info.actor.HasSaid(pair.Key) ? .5f : 1f;
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
            info.actor.RecordSaid(statement);
            //m_history.Add($"{info.actor.transform.GetInstanceID()}_{statement}");
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