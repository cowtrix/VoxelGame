using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
	public class PhoneAppBank : PhoneApp
	{
		[Serializable]
		public struct Transaction
		{
			public string Name;
			public GameDateTime Date;
			public int Delta;
			public int Balance;
			public bool Successful;
		}

		public Text BalanceText;
		public int Credits { get; set; }

		public List<Transaction> Transactions;

		public override void Initialise(PhoneController controller)
		{
			base.Initialise(controller);
			Phone.Actor.State.OnStateUpdate.AddListener(OnStateUpdate);
		}

		private void OnStateUpdate(Actor actor, StateUpdate<float> update)
		{
			var transaction = new Transaction
			{
				Name = update.Description,
				Date = GameManager.Instance.CurrentTime,
				Delta = Mathf.FloorToInt(update.Delta),
				Balance = Mathf.FloorToInt(update.Value),
				Successful = update.Success
			};
			Transactions.Add(transaction);
			Phone.NotificationManager.CreateNotification(this, $"{transaction.Name}: {transaction.Delta}¢ {(transaction.Successful ? "✓" : "! Transaction Failed")}");
		}

		private void Update()
		{
			BalanceText.text = $"<b>BALANCE:</b> {Credits}¢";
		}
	}
}