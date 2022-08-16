using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
	public class Label : Interactable
	{
		[TextArea]
		public string PlainText;
		[TextArea]
		public string TranslatedText;
		public TMPro.TMP_Text TextMeshPro;
		public override string DisplayName => $"\"{PlainText}\"";

		protected string AlienText => LanguageUtility.Translate(PlainText);

		[ContextMenu("Set TextMesh Text")]
		public void SetTextMeshText()
        {
			var t = string.IsNullOrEmpty(TranslatedText) ? PlainText : TranslatedText;
            if (TextMeshPro)
            {
				TextMeshPro.text = LanguageUtility.GetStringForTextMesh(t);
			}
        }

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			yield break;
		}
	}
}