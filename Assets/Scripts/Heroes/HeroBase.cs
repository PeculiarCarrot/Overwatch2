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
	public bool ai;

	protected Rigidbody body;
	public Material outlineMaterial;
	public Material heroMaterial;

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
		gameObject.layer = LayerMask.NameToLayer(team ? "Team1" : "Team2");
		foreach(MonoBehaviour c in GetComponents<MonoBehaviour>())
		{
			c.enabled = true;
		}
		body = GetComponent<Rigidbody>();

		HeroInput[] inputs = GetComponents<HeroInput>();
		camera = GetComponentInChildren<Camera>().gameObject;
		if(ai)
		{
			GetComponent<PlayerInput>().enabled = false;
			foreach(AudioListener al in GetComponentsInChildren<AudioListener>())
				al.enabled = false;
			foreach(Camera cam in GetComponentsInChildren<Camera>())
				cam.enabled = false;
		}
		else
		{
			GetComponent<AIInput>().enabled = false;
		}

		foreach (HeroInput i in inputs)
		{
			if (i.enabled)
			{
				input = i;
			}
		}

		if(ai)
		{
			Material[] theMaterials = new Material[2];
			theMaterials[1] = heroMaterial;
			theMaterials[0] = new Material(outlineMaterial);
			theMaterials[0].SetColor("_OutlineColor", team ? Color.blue : Color.red);
			GetComponent<Renderer>().materials = theMaterials;
		}
		else {
			GetComponent<Renderer>().material = heroMaterial;
		}
	}
	
	public void Awake () {
		Reset();
	}
	
	void Update () {
		
	}

	public void FixedUpdate()
	{
		if(body.velocity.y > -2)
			body.velocity += new Vector3(0, -4 * Time.deltaTime, 0);
	}
}
