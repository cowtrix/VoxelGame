using Actors;
using Actors.NPC.Player;
using Common;
using Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Voxul.Utilities;

namespace UI
{
    [Serializable]
    public class StringEvent : UnityEvent<string> { }

    public class HUDManager : Singleton<HUDManager>
    {
        public const int OUTLINE_LAYER = 11;

        public Image Icon;
        public UIActionLabel ActionLabel;
        public RectTransform FocusSprite;
        private Camera Camera => CameraController.GetComponent<Camera>();
        public CameraController CameraController => CameraController.Instance;
        public PlayerActor PlayerActor;
        public Material InteractionMaterial;
        public StringEvent FocusedInteractableDisplayName;

        private List<UIActionLabel> m_labels = new List<UIActionLabel>();
        private List<MeshRenderer> m_interactionOutlineRenderers = new List<MeshRenderer>();

        private GameObject m_outlineContainer;
        private Outline m_outline;

        private void Start()
        {
            ActionLabel.gameObject.SetActive(false);
            m_outlineContainer = new GameObject("OutlineContainer");
            m_outline = new GameObject("Outline").AddComponent<Outline>();
            m_outline.transform.SetParent(m_outlineContainer.transform);
        }

        private void SetFocusDisplay(IInteractable interactable)
        {
            FocusSprite.gameObject.SetActive(true);
            m_outlineContainer.SetActive(true);
            var settings = interactable.GetSettings();
            if (!m_interactionOutlineRenderers.SequenceEqual(settings.Renderers.Select(r => r.Renderer)))
            {
                for (var i = 0; i < settings.Renderers.Count; i++)
                {
                    var sourceRenderer = settings.Renderers[i];
                    if (!sourceRenderer.Renderer)
                    {
                        continue;
                    }
                    MeshRenderer targetRenderer = null;
                    MeshFilter meshfilter;
                    if (m_interactionOutlineRenderers.Count <= i)
                    {
                        targetRenderer = new GameObject($"OutlineRenderer_{i}").AddComponent<MeshRenderer>();
                        targetRenderer.gameObject.layer = OUTLINE_LAYER;
                        targetRenderer.transform.SetParent(m_outlineContainer.transform, false);
                        meshfilter = targetRenderer.gameObject.AddComponent<MeshFilter>();
                        m_interactionOutlineRenderers.Add(targetRenderer);
                    }
                    else
                    {
                        targetRenderer = m_interactionOutlineRenderers[i];
                        meshfilter = targetRenderer.GetComponent<MeshFilter>();
                    }
                    targetRenderer.sharedMaterial = InteractionMaterial;
                    meshfilter.sharedMesh = sourceRenderer.Mesh;
                    targetRenderer.transform.position = sourceRenderer.Renderer.transform.position;
                    targetRenderer.transform.rotation = sourceRenderer.Renderer.transform.rotation;
                    targetRenderer.transform.localScale = sourceRenderer.Renderer.transform.lossyScale;
                }
                for (var i = m_interactionOutlineRenderers.Count - 1; i >= settings.Renderers.Count; i--)
                {
                    var toDestroy = m_interactionOutlineRenderers[i];
                    m_interactionOutlineRenderers.RemoveAt(i);
                    toDestroy.gameObject.SafeDestroy();
                }
                if (m_interactionOutlineRenderers.Any())
                {
                    var center = m_interactionOutlineRenderers.First().transform.position;
                    foreach (var pos in m_interactionOutlineRenderers.Skip(1).Select(r => r.transform.position))
                    {
                        center += pos;
                    }
                    center /= (float)m_interactionOutlineRenderers.Count;
                    InteractionMaterial.SetVector("_WorldCenter", center);
                }
            }
            if (m_interactionOutlineRenderers.Count > 0)
            {
                m_outline.gameObject.SetActive(true);
                m_outline.SetRenderers(m_interactionOutlineRenderers);
            }
            else
            {
                m_outline.gameObject.SetActive(false);
            }

            var screenRect = new Rect(Camera.WorldToScreenPoint(interactable.transform.position), Vector2.zero);
            Bounds objBounds = interactable.GetBounds();
            foreach (var p in objBounds.AllPoints())
            {
                screenRect = screenRect.Encapsulate(Camera.WorldToScreenPoint(p));
            }
            const float nameMargin = 0f;
            if(screenRect.yMin < nameMargin)
            {
                screenRect.yMin = nameMargin;
            }
            FocusSprite.position = screenRect.center;
            FocusSprite.sizeDelta = screenRect.size;
        }

        private void ClearFocusDisplay()
        {
            m_outlineContainer.SetActive(false);
            FocusSprite.gameObject.SetActive(false);
            FocusedInteractableDisplayName.Invoke("");
        }

        private void Update()
        {
            var interactable = PlayerActor.FocusedInteractable ?? PlayerActor.State.EquippedItem as IInteractable ?? PlayerActor.CurrentActivity;
            if (interactable != null && interactable != PlayerActor.CurrentActivity)
            {
                if(interactable != (IInteractable)PlayerActor.State.EquippedItem)
                {
                    SetFocusDisplay(interactable);
                }
                else
                {
                    ClearFocusDisplay();
                }

                FocusedInteractableDisplayName.Invoke(interactable.DisplayName);
            }
            else
            {
                ClearFocusDisplay();
            }

            if (interactable != null && interactable.CanUse(PlayerActor))
            {
                int actionIndex = 0;
                foreach (var action in interactable.GetActions(PlayerActor))
                {
                    if (string.IsNullOrEmpty(action.Description))
                    {
                        // Hidden action
                        continue;
                    }
                    UIActionLabel label;
                    if (m_labels.Count <= actionIndex)
                    {
                        label = Instantiate(ActionLabel.gameObject).GetComponent<UIActionLabel>();
                        label.transform.SetParent(ActionLabel.transform.parent);
                        m_labels.Add(label);
                    }
                    else
                    {
                        label = m_labels[actionIndex];
                    }
                    label.gameObject.SetActive(true);
                    label.ActionIcon.sprite = null;
                    label.ActionName.text = action.ToString();
                    actionIndex++;
                }
                for (var i = actionIndex; i < m_labels.Count; ++i)
                {
                    m_labels[i].gameObject.SetActive(false);
                }

            }
            else
            {
                Icon.sprite = null;
                foreach (var label in m_labels)
                {
                    label.gameObject.SetActive(false);
                }
            }
            Icon.gameObject.SetActive(Icon.sprite);
        }
    }
}