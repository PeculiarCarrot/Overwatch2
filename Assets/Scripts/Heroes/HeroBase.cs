using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeroInput))]
public class HeroBase : MonoBehaviour {

	private int maxAmmo, ammo;
	private HeroInput input;
	public new GameObject camera;

	protected void SetMaxAmmo(int max)
	{
		maxAmmo = max;
		ammo = max;
	}

	public void SetAmmo(int ammo)
	{
		this.ammo = ammo;
	}

	public int GetAmmo()
	{
		return ammo;
	}

	public int GetMaxAmmo()
	{
		return maxAmmo;
	}

	public HeroInput GetInput()
	{
		return input;
	}
	
	public void Awake () {
		HeroInput[] inputs = GetComponents<HeroInput>();
		foreach (HeroInput i in inputs)
		{
			if (i.enabled)
			{
				input = i;
			}
		}
		camera = GetComponentInChildren<Camera>().gameObject;
		if(!(input is PlayerInput))
		{
			GetComponentInChildren<Camera>().enabled = false;
			GetComponentInChildren<AudioListener>().enabled = false;
		}
	}
	
	void Update () {
		
	}
}
