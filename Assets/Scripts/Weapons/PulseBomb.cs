using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PulseBomb : MonoBehaviour {

	public GameObject pulseExplosionPrefab;

	private new Collider collider;
	private new Rigidbody rigidbody;
	private float impulse = 100f;
	private GameObject stuck;
	private float timeUntilDetonate = 1f;
	private float explosionForce = 40f;
	private float explosionRadius = 3f;

	void Start () {
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.AddForce(transform.forward * impulse);
		rigidbody.AddForce(transform.up * (impulse / 10f));
	}
	
	void Update () {
		Vector3 v = rigidbody.velocity;
		v.Scale(new Vector3(.99f, 1f, .99f));
		v.y -= Time.deltaTime * 10;
		rigidbody.velocity = v;
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
			//Stick us to the hit object and disable physics and collisions
			rigidbody.isKinematic = true;
			rigidbody.velocity = Vector3.zero;
			collider.enabled = false;
			if(other.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
				transform.SetParent(other.collider.gameObject.transform.root);
			else
				transform.SetParent(other.collider.transform);
			stuck = transform.parent.gameObject;
		}
	}
}
