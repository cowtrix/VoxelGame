using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities.AsteroidDefense
{
	public class ArcadeShooter : Activity
	{
		public override string DisplayName => "Asteroid Defence";

		public Transform Grid;

		private void Update()
		{
			Grid.localScale = Vector3.MoveTowards(Grid.localScale, Actor ? Vector3.one : Vector3.one.Flatten(), Time.deltaTime * 5);
		}
	}
}