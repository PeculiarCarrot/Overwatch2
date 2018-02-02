using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PulseBomb : MonoBehaviour {

	public GameObject pulseExplosionPrefab;
	public GameObject emptyPrefab;

	private new Collider collider;
	private new Rigidbody rigidbody;
	private float impulse = 100f;
	private GameObject stuck;
	private float timeUntilDetonate = 1f;
	private float explosionForce = 40f;
	private float explosionRadius = 3f;
	Vector3 lastPos;
	private GameObject posHolder;

	void Start () {
		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.AddForce(transform.forward * impulse);
		rigidbody.AddForce(transform.up * (impulse / 10f));
	}
	
	void Update () {
		lastPos = transform.position;
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
		Destroy(posHolder);

		foreach(Rigidbody b in bodies)
		{
			b.AddExplosionForce(explosionForce, transform.position, explosionRadius, .5f);
		}

		Instantiate(pulseExplosionPrefab,transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision other)
	{
		Collide(other.collider);
	}

	void OnTriggerEnter(Collider other)
	{
		Collide(other);
	}

	private void Collide(Collider collider)
	{
		if(!stuck)
		{
			//Stick us to the hit object and disable physics and collisions
			//we create a new position holder and use that as the parent to prevent skewing from attaching to scaled objects
			posHolder = Instantiate(emptyPrefab, collider.transform);
			rigidbody.isKinematic = true;
			rigidbody.velocity = Vector3.zero;
			this.collider.enabled = false;
			transform.rotation = Quaternion.Euler(0, 0, 0);
			transform.SetParent(posHolder.transform);
			stuck = transform.parent.gameObject;
			transform.position = lastPos;
		}
	}
}
