using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeroInput))]
[RequireComponent(typeof(Rigidbody))]
public class HeroBase : MonoBehaviour {

	public bool team;
	private int maxAmmo, ammo;
	private HeroInput input;
	[HideInInspector]
	public bool ai;

	private bool isBeingCharged;

	protected Rigidbody body;
	public Material outlineMaterial;
	public Material heroMaterial;

	//View-controlling variables
	public new Camera camera;
	public Camera firstPersonCam, thirdPersonCam;
	private Vector3 preLerpCamPos, thirdPersonCamPosition;
	private Quaternion preLerpCamRot, thirdPersonCamRotation;
	private float transitionPercentage;
	private bool firstPerson;
	private const float transitionTime = .2f;

	public void ToFirstPerson()
	{
		if (!firstPerson)
		{
			transitionPercentage = 0;
			preLerpCamPos = thirdPersonCam.transform.localPosition;
			preLerpCamRot = thirdPersonCam.transform.localRotation;
		}
		firstPerson = true;
	}

	public void ToThirdPerson()
	{
		if (firstPerson)
		{
			transitionPercentage = 0;
			preLerpCamPos = thirdPersonCam.transform.localPosition;
			preLerpCamRot = thirdPersonCam.transform.localRotation;
		}
		firstPerson = false;
		camera = thirdPersonCam;
		firstPersonCam.enabled = false;
		thirdPersonCam.enabled = true;
	}

	protected void SetMaxAmmo(int max)
	{
		maxAmmo = max;
		ammo = max;
	}

	public bool IsBeingCharged()
	{
		return isBeingCharged;
	}

	public void SetBeingCharged(bool b)
	{
		isBeingCharged = b;
	}

	public bool IsStunned()
	{
		return isBeingCharged;
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
		thirdPersonCamPosition = thirdPersonCam.transform.localPosition;
		thirdPersonCamRotation = thirdPersonCam.transform.localRotation;
		firstPerson = true;
		gameObject.layer = LayerMask.NameToLayer(team ? "Team1" : "Team2");
		foreach(MonoBehaviour c in GetComponents<MonoBehaviour>())
		{
			c.enabled = true;
		}
		body = GetComponent<Rigidbody>();

		HeroInput[] inputs = GetComponents<HeroInput>();
		camera = firstPersonCam;
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

			camera = firstPersonCam;
			firstPersonCam.enabled = true;
			thirdPersonCam.enabled = false;
		}
	}
	
	public void Awake () {
		Reset();
	}
	
	public void Update () {
		if(transitionPercentage < 1)
			transitionPercentage += Time.deltaTime / transitionTime;
		if(firstPerson)
		{
			thirdPersonCam.transform.localPosition = Vector3.Lerp(preLerpCamPos, firstPersonCam.transform.localPosition, transitionPercentage);
			thirdPersonCam.transform.localRotation = Quaternion.Lerp(preLerpCamRot, firstPersonCam.transform.localRotation, transitionPercentage);
		}
		else
		{
			thirdPersonCam.transform.localPosition = Vector3.Lerp(preLerpCamPos, thirdPersonCamPosition, transitionPercentage);
			thirdPersonCam.transform.localRotation = Quaternion.Lerp(preLerpCamRot, thirdPersonCamRotation, transitionPercentage);
		}
		if(transitionPercentage >= 1 && firstPerson)
		{
			camera = firstPersonCam;
			firstPersonCam.enabled = true;
			thirdPersonCam.enabled = false;
		}
		if(ai)
		{
			firstPersonCam.enabled = false;
			thirdPersonCam.enabled = false;
		}
	}

	public void FixedUpdate()
	{
		if(body.velocity.y > -2)
			body.velocity += new Vector3(0, -4 * Time.deltaTime, 0);
	}
}
