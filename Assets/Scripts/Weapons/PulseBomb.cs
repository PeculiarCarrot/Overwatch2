using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PulseBomb : MonoBehaviour {

	public GameObject pulseExplosionPrefab;

	private new Collider collider;
	private new Rigidbody rigidbody;
	private float impulse = 50f;
	private GameObject stuck;
	private float timeUntilDetonate = 1f;
	private float explosionForce = 40f;
	private float explosionRadius = 3f;

	void Start () {
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.AddForce(transform.forward * impulse);
		rigidbody.AddForce(transform.up * (impulse / 4f));
	}
	
	void Update () {
		if (stuck != null)
			timeUntilDetonate -= Time.deltaTime;
		if (timeUntilDetonate <= 0)
			Explode();
	}

	private void Explode()
	{
		Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();
		foreach(Rigidbody b in bodies)
		{
			b.AddExplosionForce(explosionForce, transform.position, explosionRadius, .5f);
		}
		Instantiate(pulseExplosionPrefab,transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision other)
	{
		if(!stuck)
		{
			if(other.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
				transform.parent = other.collider.gameObject.transform.root;
			else
				transform.parent = other.collider.gameObject.transform;
			rigidbody.isKinematic = true;
			stuck = transform.parent.gameObject;
		}
	}
}
