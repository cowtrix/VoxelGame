using Actors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities.CardGame_Babo
{
	public class BaboPlayer : ExtendedMonoBehaviour
	{
		public int CurrentScore => Cards.Sum(c => c != null ? Card.GetCost(c.CardType) : 0);

		public Text ScoreText;
		public BaboCardGame Game;
		public Transform Card1Anchor, Card2Anchor, Card3Anchor, Card4Anchor;
		public Transform PreviewTransform, PeekTransform;
		public Actor Owner;

		public ParticleSystem WinParticles;

		private Transform IndexToAnchor(int index)
		{
			switch (index)
			{
				case 0:
					return Card1Anchor;
				case 1:
					return Card2Anchor;
				case 2:
					return Card3Anchor;
				case 3:
					return Card4Anchor;
			}
			throw new System.Exception("Invalid index " + index);
		}

		public Card GetPreviewCard()
		{
			return PreviewTransform.GetComponentInChildren<Card>();
		}

		public Card GetCardAtIndex(int index)
		{
			return GetCardInAnchor(IndexToAnchor(index));
		}

		public int GetCardIndex(Card card)
		{
			var index = 0;
			foreach (var c in Cards)
			{
				if (c == card)
				{
					return index;
				}
				index++;
			}
			return -1;
		}

		private Card GetCardInAnchor(Transform cardAnchor)
		{
			return cardAnchor.GetComponentInChildren<Card>();
		}

		public IEnumerable<Card> Cards
		{
			get
			{
				for (var i = 0; i < 4; ++i)
				{
					yield return GetCardAtIndex(i);
				}
			}
		}

		public void DealToPlayer(Card card, int index, bool showToPlayer)
		{
			StartCoroutine(DealToPlayerRoutine(card, index, showToPlayer));
		}

		private IEnumerator DealToPlayerRoutine(Card card, int index, bool showToPlayer)
		{
			if (GetCardAtIndex(index))
			{
				throw new System.Exception($"Player {this} already had a card at index {index}");
			}
			var t = IndexToAnchor(index);
			card.transform.SetParent(t, true);
			card.TargetPosition = Vector3.zero;
			card.TargetNormal = Vector3.up;
			card.TargetRotation = Quaternion.identity;

			while ((card.TargetPosition - card.transform.localPosition).magnitude < .01f)
			{
				yield return null;
			}

			card.TargetPosition = new Vector3(0, 0, -0.12f);
			card.TargetNormal = Vector3.up;
			card.TargetRotation = Quaternion.Euler(-90, 0, 0);

			while ((card.TargetPosition - card.transform.localPosition).magnitude < .01f)
			{
				yield return null;
			}

			yield return new WaitForSeconds(3);

			card.TargetPosition = Vector3.zero;
			card.TargetNormal = Vector3.up;
			card.TargetRotation = Quaternion.identity;
		}

		public Card TakeFromList(List<Card> list)
		{
			if (GetPreviewCard())
			{
				Debug.LogError($"Player {this} was already preview a card");
				return null;
			}

			var newCard = list.Last();
			list.RemoveAt(list.Count - 1);

			newCard.transform.SetParent(PreviewTransform, true);
			newCard.TargetPosition = Vector3.zero;
			newCard.TargetNormal = Vector3.up;
			newCard.TargetRotation = Quaternion.identity;

			return newCard;
		}

		public void Discard(Card card)
		{
			Game.DiscardPile.Add(card);
			card.transform.SetParent(Game.transform, true);
			card.TargetPosition = Game.DiscardInteractive.transform.localPosition + Vector3.up * (Game.DiscardPile.Count + 1) * Game.CardStackHeight;
			card.TargetNormal = Game.DiscardInteractive.transform.LocalUp();
			card.TargetRotation = Game.DiscardInteractive.transform.localRotation * Quaternion.Euler(0, 0, 0);

			card.Interactable.enabled = false;
			card.CardText.gameObject.SetActive(false);
			card.Activated = false;

			foreach (var c in Cards)
			{
				if (c == null)
				{
					continue;
				}
				c.Interactable.enabled = false;
			}
		}

		public IEnumerator ShowCardAsync(int handIndex)
		{
			var card = GetCardAtIndex(handIndex);
			card.TargetPosition = new Vector3(0, 0, -0.12f);
			card.TargetNormal = Vector3.up;
			card.TargetRotation = Quaternion.Euler(-90, 0, 0);

			while ((card.TargetPosition - card.transform.localPosition).magnitude < .01f)
			{
				yield return null;
			}

			yield return new WaitForSeconds(3);

			card.TargetPosition = Vector3.zero;
			card.TargetNormal = Vector3.up;
			card.TargetRotation = Quaternion.identity;
		}

		public IEnumerator PeekCardAsync(Card card)
		{
			card.TargetPosition = card.transform.parent.worldToLocalMatrix.MultiplyPoint3x4(PeekTransform.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0, 0, -0.12f)));
			card.TargetNormal = card.transform.parent.worldToLocalMatrix.MultiplyVector(PeekTransform.transform.localToWorldMatrix.MultiplyVector(Vector3.up));
			card.TargetRotation = card.transform.parent.worldToLocalMatrix.rotation * PeekTransform.rotation;

			while ((card.TargetPosition - card.transform.localPosition).magnitude < .01f)
			{
				yield return null;
			}

			yield return new WaitForSeconds(3);

			card.TargetPosition = Vector3.zero;
			card.TargetNormal = Vector3.up;
			card.TargetRotation = Quaternion.identity;
		}

		private void Update()
		{
			ScoreText.text = CurrentScore.ToString();
		}
	}
}