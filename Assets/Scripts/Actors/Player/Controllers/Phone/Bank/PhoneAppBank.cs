using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
	public class PhoneAppBank : PhoneApp
	{
		public Text BalanceText;
		public int Credits { get; set; }

		private void Update()
		{
			BalanceText.text = $"<b>BALANCE:</b> {Credits}¢";
		}
	}
}