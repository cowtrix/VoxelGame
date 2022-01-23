using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace CardGame_Babo
{
	public class BaboPlayer : ExtendedMonoBehaviour
	{
		public int CurrentScore => Cards.Sum(c => Card.GetCost(c.CardType));

		public CardGame Game;
		public Transform Card1Anchor, Card2Anchor, Card3Anchor, Card4Anchor;
		public Transform PreviewTransform;
		public Actor Owner;

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
			foreach(var c in Cards)
			{
				if(c == card)
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
				for(var i = 0; i < 4; ++i)
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

			while((card.TargetPosition - card.transform.localPosition).magnitude < .01f)
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

			card.GetComponent<Interactable>().enabled = true;
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
			card.TargetPosition = Game.DiscardPosition.transform.localPosition + Vector3.up * (Game.DiscardPile.Count + 1) * Game.CardStackHeight;
			card.TargetNormal = Game.DiscardPosition.transform.LocalUp();
			card.TargetRotation = Quaternion.Euler(180, -90, 0) * Game.DiscardPosition.localRotation;
		}
	}
}