using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class ReinhardtShield : MonoBehaviour {

	private bool active;
	private int hp, maxHP;
	private new MeshRenderer renderer;
	private new Collider collider;
	
	void Start () {
		gameObject.layer = LayerMask.NameToLayer(GetComponentInParent<HeroBase>().team ? "Team1" : "Team2");
		maxHP = 2000;
		hp = maxHP;
		renderer = GetComponent<MeshRenderer>();
		collider = GetComponent<Collider>();
		SetActive(false);
	}

	public bool IsActive()
	{
		return active;
	}

	public void SetActive(bool active)
	{
		this.active = active;
		renderer.enabled = active;
		collider.enabled = active;
	}
	
	void Update () {
		
	}
}
