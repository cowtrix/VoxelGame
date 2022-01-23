using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Voxul.Utilities;

namespace CardGame_Babo
{
	public enum ECardType
	{
		Num_1, Num_2, Num_3, Num_4, Num_5, Num_6,
		LookSelf,
		LookOther,
		Swap,
		LookSwap,
	}

	[Serializable]
	public struct RotationLimits
	{
		public Vector2 X, Y, Z;
	}

	public class CardGame : FocusableInteractable
	{
		public override string DisplayName => "Card Game";

		public Card CardPrefab;
		public List<Card> Deck { get; private set; } = new List<Card>();
		public List<Card> DiscardPile { get; private set; } = new List<Card>();

		[Serializable]
		public class CardTextureMapping
		{
			public ECardType CardType;
			public Texture2D CardTexture;
		}
		public List<CardTextureMapping> CardTextures;
		public Texture2D GetCardTexture(ECardType type) => CardTextures?.FirstOrDefault(c => c.CardType == type)?.CardTexture;

		public RotationLimits CameraRotationLimits;
		public float LookSpeed = 1;

		public Transform DeckPosition, DiscardPosition;
		public float CardStackHeight = .01f;
		public int StartingPlayerIndex;

		public UnityEvent OnPlayerTurnStart, OnOpponentTurnStart;

		public List<BaboPlayer> Players;
		public BaboPlayer RealPlayer => Players.Last();
		public BaboPlayer ActivePlayer { get; private set; }
		public bool WaitingForPlayer { get; private set; }

		public void BeginActivate(Actor actor)
		{
			RealPlayer.Owner = actor;
			StartCoroutine(SetupRound());
		}

		public void EndActivate(Actor actor)
		{
			RealPlayer.Owner = null;
		}

		private IEnumerator SetupRound()
		{
			ActivePlayer = null;
			for (var i = 0; i < 4; ++i)
			{
				for (var j = ECardType.Num_1; j <= ECardType.LookSwap; ++j)
				{
					var newCard = Instantiate(CardPrefab.gameObject).GetComponent<Card>();
					newCard.name = $"{j}_{i}";
					newCard.transform.SetParent(transform);
					newCard.transform.Reset(true);
					newCard.Game = this;
					newCard.CardType = j;
					newCard.Invalidate();
					newCard.TargetPosition = DeckPosition.localPosition;
					newCard.TargetNormal = Vector3.up;
					newCard.TargetRotation = DeckPosition.localRotation;
					var interactable = newCard.GetComponent<Interactable>();
					interactable.enabled = false;
					interactable.InteractionSettings.OnUsed.AddListener((a) => OnCardUsed(newCard));
					Deck.Add(newCard);
					yield return null;
				}
			}
			ShuffleDeck();
			yield return new WaitForSeconds(1);
			for (var i = 0; i < 4; ++i)
			{
				foreach (var player in Players)
				{
					var topCard = Deck.Last();
					Deck.RemoveAt(Deck.Count - 1);
					player.DealToPlayer(topCard, i, true);
					yield return new WaitForSeconds(.1f);
				}
			}
			yield return new WaitForSeconds(3);
			StartCoroutine(PlayRound());
		}

		private void OnCardUsed(Card newCard)
		{
			if (!WaitingForPlayer)
			{
				return;
			}

			var currentPreviewCard = RealPlayer.GetPreviewCard();
			if (currentPreviewCard)
			{
				if (newCard == currentPreviewCard)
				{
					return;
				}

				var handIndex = RealPlayer.GetCardIndex(newCard);
				if (handIndex >= 0)
				{
					// Swap
					RealPlayer.Discard(newCard);
					RealPlayer.DealToPlayer(currentPreviewCard, handIndex, false);
					WaitingForPlayer = false;

					currentPreviewCard.CardCanvas.enabled = false;
				}
				newCard.CardCanvas.enabled = false;
			}
		}

		public void PlayerDiscard()
		{
			if (!WaitingForPlayer)
			{
				return;
			}
			var currentPreviewCard = RealPlayer.GetPreviewCard();
			if (currentPreviewCard)
			{
				RealPlayer.Discard(currentPreviewCard);
				WaitingForPlayer = false;
			}
			else
			{
				RealPlayer.TakeFromList(DiscardPile)
					.CardCanvas.enabled = true;
			}
		}

		public void PlayerDeck()
		{
			if (!WaitingForPlayer)
			{
				return;
			}
			var currentPreviewCard = RealPlayer.GetPreviewCard();
			if (!currentPreviewCard)
			{
				RealPlayer.TakeFromList(Deck)
					.CardCanvas.enabled = true;
			}
		}

		IEnumerator PlayRound()
		{
			int currentPlayerIndex = StartingPlayerIndex;
			StartingPlayerIndex++;
			if (StartingPlayerIndex >= Players.Count)
			{
				StartingPlayerIndex = 0;
			}
			while (Deck.Any())
			{
				ActivePlayer = Players[currentPlayerIndex];
				Debug.Log($"It is {ActivePlayer}'s turn. They currently have a score of {ActivePlayer.CurrentScore}");
				if (ActivePlayer.Owner is PlayerActor)
				{
					OnPlayerTurnStart.Invoke();
					WaitingForPlayer = true;
					yield return StartCoroutine(PlayTurn(ActivePlayer));
				}
				else
				{
					OnOpponentTurnStart.Invoke();
					yield return StartCoroutine(PlayAITurn(ActivePlayer));
				}
				yield return null;

				currentPlayerIndex++;
				if (currentPlayerIndex >= Players.Count)
				{
					currentPlayerIndex = 0;
				}
			}
		}

		IEnumerator PlayTurn(BaboPlayer player)
		{
			while (WaitingForPlayer)
			{
				yield return null;
			}
		}

		IEnumerator PlayAITurn(BaboPlayer player)
		{
			yield return new WaitForSeconds(1);
			Card newCard;
			var discard = DiscardPile.LastOrDefault();
			if (discard && discard.Cost < player.Cards.Max(c => c.Cost))
			{
				newCard = player.TakeFromList(DiscardPile);
				Debug.Log($"Player {player} pulled {newCard} from discard");
			}
			else
			{
				newCard = player.TakeFromList(Deck);
				Debug.Log($"Player {player} pulled {newCard} from deck");
			}

			yield return new WaitForSeconds(2);

			var bestSwapIndex = -1;
			var index = 0;
			foreach (var existingCard in player.Cards)
			{
				if (existingCard.Cost > newCard.Cost)
				{
					bestSwapIndex = index;
				}
				index++;
			}
			if (bestSwapIndex >= 0)
			{
				player.Discard(player.GetCardAtIndex(bestSwapIndex));
				player.DealToPlayer(newCard, bestSwapIndex, true);

				Debug.Log($"Player {player} swaps with card {bestSwapIndex}.");
			}
			else
			{
				player.Discard(newCard);
				Debug.Log($"Player {player} discards.");
			}

			yield return new WaitForSeconds(2);
		}

		private void ShuffleDeck()
		{
			Deck = Deck.OrderBy(c => UnityEngine.Random.value).ToList();
			for (int i = 0; i < Deck.Count; i++)
			{
				var card = Deck[i];
				card.TargetPosition = DeckPosition.transform.localPosition + Vector3.up * i * CardStackHeight;
				card.TargetRotation = DeckPosition.transform.localRotation;
			}
		}

		public override void Look(Actor actor, Vector2 direction)
		{
			direction *= Time.deltaTime * LookSpeed;
			LookRotation = new Vector3(Mathf.Clamp(LookRotation.x - direction.y, CameraRotationLimits.X.x, CameraRotationLimits.X.y),
				Mathf.Clamp(LookRotation.y + direction.x, CameraRotationLimits.Y.x, CameraRotationLimits.Y.y),
				Mathf.Clamp(LookRotation.z, CameraRotationLimits.Z.x, CameraRotationLimits.Z.y));
		}
	}
}