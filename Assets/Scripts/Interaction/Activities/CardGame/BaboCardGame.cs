using Actors;
using Actors.NPC.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Voxul.Utilities;

namespace Interaction.Activities.CardGame_Babo
{
	public enum ECardType
	{
		Num_1, Num_2, Num_3, Num_4, Num_5, Num_6,
		LookSelf,
		LookOther,
		Swap,
		LookSwap,
	}


	public class BaboCardGame : Activity
	{
		public override string DisplayName => "Card Game";

		public Card CardPrefab;
		public List<Card> Deck { get; private set; } = new List<Card>();
		public List<Card> DiscardPile { get; private set; } = new List<Card>();
		public IEnumerable<Card> ActiveCards
		{
			get
			{
				foreach (var p in Players)
				{
					foreach (var c in p.Cards)
					{
						if (c == null)
							continue;
						yield return c;
					}
				}
			}
		}
		public IEnumerable<Card> AllCards
		{
			get
			{
				foreach (var c in Deck) yield return c;
				foreach (var c in DiscardPile) yield return c;
				foreach (var c in ActiveCards) yield return c;
			}
		}

		[Serializable]
		public class CardTextureMapping
		{
			public ECardType CardType;
			public Texture2D CardTexture;
		}
		public List<CardTextureMapping> CardTextures;
		public Texture2D GetCardTexture(ECardType type) => CardTextures?.FirstOrDefault(c => c.CardType == type)?.CardTexture;
		public SimpleInteractable DeckInteractive, DiscardInteractive, Bell;
		public float CardStackHeight = .01f;
		public int StartingPlayerIndex;

		public Card OtherFocusedCard;

		public UnityEvent OnPlayerTurnStart, OnOpponentTurnStart;

		public BaboPlayer BaboPlayer;

		public List<BaboPlayer> Players;

		public BaboPlayer RealPlayer => Players.Last();
		public BaboPlayer ActivePlayer { get; private set; }
		public bool WaitingForPlayer { get; private set; }

		public void SetCardsInteractive(bool enabled, IEnumerable<Card> enumerable = null)
		{
			enumerable = enumerable ?? AllCards;
			foreach (var c in enumerable)
			{
				c.Activated &= enabled;
				c.GetComponent<Interactable>().enabled = enabled;
			}
		}

		public void PlayerBabo()
		{
			if (BaboPlayer)
			{
				Debug.LogError("A player has already Babo'd, this shouldn't happen.");
				return;
			}
			Babo(RealPlayer);
			WaitingForPlayer = false;
		}

		public void Babo(BaboPlayer player)
		{
			if (BaboPlayer)
			{
				Debug.LogError("A player has already Babo'd, this shouldn't happen.");
				return;
			}
			BaboPlayer = player;
		}

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
			SetCardsInteractive(false);
			DeckInteractive.enabled = false;
			DiscardInteractive.enabled = false;
			ActivePlayer = null;
			Bell.enabled = false;
			BaboPlayer = null;

			if (!AllCards.Any())
			{
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
						newCard.TargetPosition = DeckInteractive.transform.localPosition;
						newCard.TargetNormal = Vector3.up;
						newCard.TargetRotation = DeckInteractive.transform.localRotation;
						var interactable = newCard.GetComponent<Interactable>();
						interactable.InteractionSettings.OnUsed.AddListener((a, c) => StartCoroutine(OnCardUsed(newCard)));
						Deck.Add(newCard);
						yield return null;
					}
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

		private BaboPlayer GetPlayerWithCard(Card c, out int index)
		{
			foreach (var p in Players)
			{
				index = 0;
				foreach (var c2 in p.Cards)
				{
					if (c == c2)
					{
						return p;
					}
					index++;
				}
			}
			index = -1;
			return null;
		}

		private void UsePreviewCard(Card card)
		{
			// Toggle Activation
			if (card.CardType > ECardType.Num_6 && !card.Activated)
			{
				card.Activated = true;
				card.Interactable.Actions.Clear();
				switch (card.CardType)
				{
					case ECardType.LookOther:
						foreach (var c in ActiveCards.Where(c => !RealPlayer.Cards.Contains(c)))
						{
							c.Interactable.enabled = true;
							c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Look At Someone Else's Card" });
						}
						break;
					case ECardType.LookSelf:
						foreach (var c in ActiveCards.Where(c => RealPlayer.Cards.Contains(c)))
						{
							c.Interactable.enabled = true;
							c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Look At Your Own Card" });
						}
						break;
					case ECardType.Swap:
						foreach (var c in ActiveCards.Where(c => !RealPlayer.Cards.Contains(c)))
						{
							c.Interactable.enabled = true;
							c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Swap Any Two Cards" });
						}
						break;
					case ECardType.LookSwap:
						foreach (var c in ActiveCards.Where(c => !RealPlayer.Cards.Contains(c)))
						{
							c.Interactable.enabled = true;
							c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Look Any Two, Swap Any Two" });
						}
						break;
				}
			}
			else
			{
				card.Activated = false;
				if (card.CardType > ECardType.Num_6)
				{
					card.Interactable.enabled = true;
					card.Interactable.Actions.Clear();
					card.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Activate Card" });
				}
				foreach (var c in ActiveCards)
				{
					if (c == null)
					{
						continue;
					}
					if (RealPlayer.Cards.Contains(c))
					{
						c.Interactable.enabled = true;
						c.Interactable.Actions.Clear();
						c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Swap Card" });
					}
					else
					{
						c.Interactable.enabled = false;
					}
				}
			}
		}

		private IEnumerator OnCardUsed(Card card)
		{
			if (!WaitingForPlayer)
			{
				yield break;
			}

			var currentPreviewCard = RealPlayer.GetPreviewCard();
			if (!currentPreviewCard)
			{
				yield break;
			}

			if (card == currentPreviewCard)
			{
				UsePreviewCard(currentPreviewCard);
				yield break;
			}

			var playerHolding = GetPlayerWithCard(card, out var handIndex);
			if (!playerHolding)
			{
				yield break;
			}

			if (!currentPreviewCard.Activated)
			{
				if (playerHolding == RealPlayer)
				{
					// Swap with one of your own cards
					RealPlayer.Discard(card);
					RealPlayer.DealToPlayer(currentPreviewCard, handIndex, false);
					WaitingForPlayer = false;
				}
				yield break;
			}

			// We're using a power card


			// Look Self
			if (currentPreviewCard.CardType == ECardType.LookSelf && playerHolding == RealPlayer)
			{
				yield return StartCoroutine(RealPlayer.ShowCardAsync(handIndex));
				RealPlayer.Discard(currentPreviewCard);
				WaitingForPlayer = false;
				yield break;
			}

			// Look Other
			if (currentPreviewCard.CardType == ECardType.LookOther && playerHolding != RealPlayer)
			{
				yield return StartCoroutine(RealPlayer.PeekCardAsync(card));
				RealPlayer.Discard(currentPreviewCard);
				WaitingForPlayer = false;
				yield break;
			}

			// The other two require a focused card
			if (!OtherFocusedCard)
			{
				OtherFocusedCard = card;
				OtherFocusedCard.Activated = true;
				yield break;
			}

			// Swap
			if (currentPreviewCard.CardType == ECardType.Swap)
			{
				Swap(OtherFocusedCard, card);
				RealPlayer.Discard(currentPreviewCard);
				WaitingForPlayer = false;
				yield break;
			}
		}

		public void PlayerDiscard()
		{
			if (!WaitingForPlayer)
			{
				return;
			}
			var currentPreviewCard = RealPlayer.GetPreviewCard();
			if (currentPreviewCard && !currentPreviewCard.Activated)
			{
				RealPlayer.Discard(currentPreviewCard);
				DeckInteractive.enabled = false;
				DiscardInteractive.enabled = false;
				WaitingForPlayer = false;
			}
			else if (DiscardPile.Count > 0)
			{
				var newCard = RealPlayer.TakeFromList(DiscardPile);
				Bell.enabled = false;
				newCard.CardText.gameObject.SetActive(true);
				DeckInteractive.enabled = false;
				DiscardInteractive.enabled = true;
				DiscardInteractive.Actions.Clear();
				DiscardInteractive.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Discard Card" });
				if (newCard.CardType > ECardType.Num_6)
				{
					newCard.Interactable.enabled = true;
					newCard.Interactable.Actions.Clear();
					newCard.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Activate Card" });
				}
				foreach (var c in RealPlayer.Cards)
				{
					if (c == null)
					{
						continue;
					}
					c.Interactable.enabled = true;
					c.Interactable.Actions.Clear();
					c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Swap Card" });
				}
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
				var newCard = RealPlayer.TakeFromList(Deck);
				Bell.enabled = false;
				newCard.CardText.gameObject.SetActive(true);
				DeckInteractive.enabled = false;
				DiscardInteractive.enabled = true;
				DiscardInteractive.Actions.Clear();
				DiscardInteractive.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Discard Card" });
				if (newCard.CardType > ECardType.Num_6)
				{
					newCard.Interactable.enabled = true;
					newCard.Interactable.Actions.Clear();
					newCard.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Activate Card" });
				}
				foreach (var c in RealPlayer.Cards)
				{
					if (c == null)
					{
						continue;
					}
					c.Interactable.enabled = true;
					c.Interactable.Actions.Clear();
					c.Interactable.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Swap Card" });
				}
			}
		}

		public void Swap(Card first, Card second)
		{
			var firstParent = first.transform.parent;
			first.transform.SetParent(second.transform.parent, true);
			second.transform.SetParent(firstParent, true);

			Debug.Log($"Swapped {first.name} with {second.name}");
		}

		IEnumerator PlayRound()
		{
			int currentPlayerIndex = StartingPlayerIndex;
			StartingPlayerIndex++;
			if (StartingPlayerIndex >= Players.Count)
			{
				StartingPlayerIndex = 0;
			}
			while (Deck.Any() && (BaboPlayer == null || BaboPlayer != Players[currentPlayerIndex]))
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

			// End game
			int winningScore = Players.Min(p => p.CurrentScore);
			foreach (var p in Players)
			{
				if (p.CurrentScore == winningScore)
				{
					p.WinParticles.Play();
				}
				foreach (var c in p.Cards)
				{
					c.TargetRotation *= Quaternion.Euler(0, 180, 0);
				}
				p.ScoreText.gameObject.SetActive(true);
			}
			yield return new WaitForSeconds(5);
			foreach (var p in Players)
			{

				p.ScoreText.gameObject.SetActive(false);
				foreach (var c in p.Cards)
				{
					c.TargetRotation *= Quaternion.Euler(0, 0, 0);
				}
			}
			yield return new WaitForSeconds(1);
			foreach (var p in Players)
			{
				foreach (var c in p.Cards)
				{
					p.Discard(c);
				}
			}
			yield return new WaitForSeconds(2);
			StartCoroutine(PlayRound());
		}

		IEnumerator PlayTurn(BaboPlayer player)
		{
			SetCardsInteractive(false);
			Bell.enabled = !BaboPlayer;
			DeckInteractive.enabled = true;
			DeckInteractive.Actions.Clear();
			DeckInteractive.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Take Card From Deck" });
			if (DiscardPile.Count > 0)
			{
				DiscardInteractive.enabled = true;
				DiscardInteractive.Actions.Clear();
				DiscardInteractive.Actions.Add(new ActorAction { Key = eActionKey.USE, Description = "Take Card From Discard" });
			}
			while (WaitingForPlayer)
			{
				yield return null;
			}
			Bell.enabled = false;
			SetCardsInteractive(false);
		}

		IEnumerator PlayAITurn(BaboPlayer player)
		{
			yield return new WaitForSeconds(1);
			Card newCard;
			var discard = DiscardPile.LastOrDefault();
			if (discard && discard.Cost < player.Cards.Max(c => c ? c.Cost : 0))
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
			Deck = AllCards.OrderBy(c => UnityEngine.Random.value).ToList();
			for (int i = 0; i < Deck.Count; i++)
			{
				var card = Deck[i];
				card.TargetPosition = DeckInteractive.transform.localPosition + Vector3.up * i * CardStackHeight;
				card.TargetRotation = DeckInteractive.transform.localRotation;
			}
		}

	}
}