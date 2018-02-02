using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class ReinhardtShield : MonoBehaviour {

	public Reinhardt rein;
	private bool active;
	private int hp, maxHP;
	private new MeshRenderer renderer;
	private new Collider collider;
	public Material red, blue;
	public Animator animator;
	
	void Start () {
		gameObject.layer = LayerMask.NameToLayer(GetComponentInParent<HeroBase>().team ? "Team1" : "Team2");
		maxHP = 2000;
		hp = maxHP;
		renderer = GetComponent<MeshRenderer>();
		renderer.material = rein.IsClientFriendly() ? blue : red;
		collider = GetComponent<Collider>();
		SetActive(false);
		DisableShieldColliderAndRenderer();
	}

	public void DisableShieldColliderAndRenderer()
	{
		renderer.enabled = false;
		collider.enabled = false;
	}

	public void EnableShieldColliderAndRenderer()
	{
		renderer.enabled = true;
		collider.enabled = true;
	}

	public bool IsActive()
	{
		return active;
	}
	 
	public void SetActive(bool active)
	{
		if (active && !this.active)
		{
			animator.Play("ReinShieldGrow");
		}
		else if(!active && this.active)
			animator.Play("ReinShieldShrink");
		
		this.active = active;
		if(active)
			EnableShieldColliderAndRenderer();
	}
}
