using Common;
using UnityEngine;


namespace Weapons
{
	public class GravityGun : Weapon
	{
		protected override void OnHit(RaycastHit hit)
		{
			var gs = hit.collider.transform.GetComponentInAncestors<GravitySource>();
			if (!gs || !hit.normal.IsOnAxis())
			{
				return;
			}
			gs.SetGravity(-hit.normal);
			return;
		}
	}
}