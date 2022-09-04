using Actors;
using Common;
using Interaction.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

namespace Interaction.Activities
{
    public class BookItem : EquippableItemComponent
    {
        [Serializable]
        public class Page
        {
            public Vector3 OpenRotation, CloseRotation;
            public TextMeshPro Content, PageNumber;
            public Transform Root, Pages;
        }

        public int PageNumber { get; set; }
        public Page FrontCover, BackCover;
        public BookData Content;
        public float HoldDistance = 1;
        public float MoveSpeed = 1;
        public float OpenSpeed = 1;
        public int Rows = 50;
        public int Columns = 30;

       // public override string DisplayName => Content ? Content.Title : LanguageUtility.GetStringForTextMesh(LanguageUtility.Generate(5, gameObject.GetHashCode()));

        private float m_targetOpen = 0;
        private float m_openAmount = 0;

        [SerializeField]
        [Range(0, 1)]
        private float m_pageTurnAmount = 0;

        protected void Start()
        {
            GeneratePages();
        }

        [ContextMenu("Generate Pages")]
        public void GeneratePages()
        {
            m_pages.Clear();
            var text = "";
            if (Content)
            {
                text = Content.Text;
            }
            else
            {
                text = LanguageUtility.GetGarbageText(UnityEngine.Random.Range(2000, 3000));
            }
            var split = text.Split(' ');
            var sb = new StringBuilder();
            int colCounter = 0, rowCounter = 0;
            foreach (var w in split)
            {
                var wlen = w.Length + 1;
                colCounter += wlen;
                if (w.Contains('\n'))
                {
                    colCounter = wlen;
                    rowCounter += w.Count(c => c == '\n');
                }
                else if (colCounter > Columns)
                {
                    colCounter = w.Length;
                    rowCounter++;
                    sb.Append('\n');
                }
                else if (rowCounter > Rows)
                {
                    rowCounter = 0;
                    colCounter = wlen;
                    if (Content)
                    {
                        m_pages.Add(sb.ToString());
                    }
                    else
                    {
                        m_pages.Add(LanguageUtility.GetStringForTextMesh($"{{{sb}}}", true));
                    }
                    sb.Clear();
                }
                sb.Append($"{w} ");
            }
            if (Content)
            {
                m_pages.Add(sb.ToString());
            }
            else
            {
                m_pages.Add(LanguageUtility.GetStringForTextMesh($"{{{sb}}}", true));
            }
        }

        private void Update()
        {
            if (!EquippedActor)
            {
                FrontCover.Pages.gameObject.SetActive(false);
                BackCover.Pages.gameObject.SetActive(false);
                return;
            }
            FrontCover.Pages.gameObject.SetActive(true);
            BackCover.Pages.gameObject.SetActive(true);
            UpdatePages();
            m_targetOpen = Mathf.Clamp01(m_targetOpen + Time.deltaTime * OpenSpeed);
        }

        List<string> m_pages = new List<string>();

        void UpdatePages()
        {
            FrontCover.Content.text = m_pages.ElementAtOrDefault(PageNumber + 1);
            FrontCover.PageNumber.text = (PageNumber + 2).ToString();

            BackCover.Content.text = m_pages.ElementAtOrDefault(PageNumber);
            BackCover.PageNumber.text = (PageNumber + 1).ToString();

            BackCover.Root.localRotation = Quaternion.Lerp(Quaternion.Euler(BackCover.CloseRotation), Quaternion.Euler(BackCover.OpenRotation), m_targetOpen);

            m_pageTurnAmount = Mathf.Clamp(m_pageTurnAmount, 0, m_openAmount);
        }

        public override void OnEquip(Actor actor)
        {
            m_targetOpen = 1;
            Item.Rigidbody.isKinematic = true;
            Item.Rigidbody.detectCollisions = false;
            base.OnEquip(actor);
        }

        public override void OnUnequip(Actor actor)
        {
            m_targetOpen = 0;
            Item.Rigidbody.isKinematic = false;
            Item.Rigidbody.detectCollisions = true;
            base.OnUnequip(actor);
        }

        public override IEnumerable<ActorAction> GetActions(Actor actor)
        {
            if (EquippedActor && actor == EquippedActor)
            {
                yield return new ActorAction(eActionKey.NEXT, "Next Page", gameObject);
                yield return new ActorAction(eActionKey.PREV, "Previous Page", gameObject);
            }
            foreach (var s in base.GetActions(actor))
            {
                yield return s;
            }
        }

        public override bool ReceiveAction(Actor actor, ActorAction action)
        {
            if (action.State == eActionState.End && EquippedActor == actor)
            {
                if (action.Key == eActionKey.NEXT && PageNumber < m_pages.Count - 2)
                {
                    PageNumber += 2;
                    return true;
                }
                if (action.Key == eActionKey.PREV && PageNumber > 0)
                {
                    PageNumber -= 2;
                    return true;
                }
            }
            return false;
        }
    }
}