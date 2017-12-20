using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeroInput))]
[RequireComponent(typeof(Rigidbody))]
public class HeroBase : MonoBehaviour {

	public bool team;
	private int maxAmmo, ammo;
	private HeroInput input;
	public new GameObject camera;

	protected Rigidbody body;

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

	public void Reset()
	{
		body = GetComponent<Rigidbody>();

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

		GetComponent<Renderer>().material = Resources.Load<Material>(team ? "Textures/ally" : "Textures/enemy");
	}
	
	public void Awake () {
		Reset();
	}
	
	void Update () {
		
	}

	public void FixedUpdate()
	{
		if(body.velocity.y > -2)
			body.velocity += new Vector3(0, -3 * Time.deltaTime, 0);
	}
}
