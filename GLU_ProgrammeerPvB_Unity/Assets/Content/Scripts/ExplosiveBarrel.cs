using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExplosiveBarrel : MonoBehaviour
{


	[SerializeField] private float _damage;
	[SerializeField] private float _force = 500f;
	[SerializeField] private float _radius = 500f;
	[SerializeField] private float _rotationForce = 3f;
	[SerializeField] private LayerMask _damageLayerMask;
	[SerializeField] private GameObject _explodeVFX;

	
	private void OnCollisionEnter(Collision collision)
	{
		Explode(collision);
	}

	private void Explode(Collision collision)
	{
		var overLapped = Physics.OverlapSphere(transform.position, _radius,_damageLayerMask);

		
		for (int i = 0; i < overLapped.Length; i++)
		{
			var damageable = overLapped[i].gameObject.GetComponentInParent<IDamageable>();
			if (damageable != null)
			{
				damageable.OnHealthChange(-_damage);
			}
			
			Debug.Log(damageable);	

			var rigidbody = overLapped[i].gameObject.GetComponentInParent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.AddExplosionForce(_force, transform.position, _radius);
				rigidbody.angularVelocity = new Vector3(Random.Range(-1f, 1f) * _rotationForce, Random.Range(-1f, 1f) * _rotationForce, Random.Range(-1f, 1f) * _rotationForce);
			}
			
			Debug.Log(rigidbody);
		}

		if (_explodeVFX != null)
		{
			Destroy(Instantiate(_explodeVFX, base.transform.position, Quaternion.identity).gameObject, 10f);
		}

		gameObject.SetActive(false);
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position,_radius);
	}
#endif
}


