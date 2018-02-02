using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthShatterProjectile : MonoBehaviour {

	float speed = 50f;
	float lifetime = 0;
	public GameObject explosion;

	void Start () {
		Destroy(gameObject, .7f);
	}

	void Update () {
		Vector3 newPos = transform.position;
		newPos += transform.forward * speed * Time.deltaTime;
		transform.position = newPos;
		lifetime += Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			Instantiate(explosion, transform.position, Quaternion.Euler(-transform.forward));
			Destroy(gameObject);
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer("PhysicsObject"))
		{
			Rigidbody b = other.gameObject.GetComponent<Rigidbody>();
			if (b != null)
				b.AddExplosionForce(40f, transform.position, 5);
		}
	}
}
