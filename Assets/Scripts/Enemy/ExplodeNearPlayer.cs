using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeNearPlayer : MonoBehaviour
{
	public GameObject Explosion;
	public float ExplosionDamage = 3;
	public float ExplosionRadius = 1;
	public float ExplosionForce = 3;

	public void TriggerEnter(Collider other)
	{
		var mc = other.GetComponent<MovementController>();
		if (!mc)
		{
			return;
		}
		if(Explosion)
		{
			Instantiate(Explosion)
				.transform.position = transform.position;
		}		
		mc.Rigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
		Destroy(gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, ExplosionRadius);
	}
}
