using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinhardtHammer : MonoBehaviour {

	public Reinhardt reinhardt;
	public Animator animator;
	public new Collider collider;
	public GameObject hitbox;
	
	void Start () {
		collider.gameObject.layer = LayerMask.NameToLayer(reinhardt.team ? "Team1Hitbox" : "Team2Hitbox");
	}

	public void Shatter()
	{
		reinhardt.EarthShatter();
	}

	public void EnableShatter()
	{
		reinhardt.EnableShatter();
	}

	public void DisableCollider()
	{
		hitbox.SetActive(false);
		collider.enabled = false;
	}

	public void EnableCollider()
	{
		hitbox.SetActive(true);
		collider.enabled = true;
	}

	void Update () {
		animator.SetBool("stillSwinging", reinhardt.IsSwinging());
	}

	void FixedUpdate()
	{
	}

	public void SpawnFirestrike()
	{
		reinhardt.SpawnFirestrike();
	}
}
