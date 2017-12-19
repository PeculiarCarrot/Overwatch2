using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Reinhardt : HeroBase {

	public RigidbodyFirstPersonController firstPersonController;
	public Camera firstPersonCam, thirdPersonCam;
	public GameObject shieldObject, hammerObject;
	public GameObject firestrikePrefab;
	private ReinhardtHammer hammer;
	private ReinhardtShield shield;
	private float regularSpeed = 6f, shieldingSpeed = 2.75f;
	public Texture crosshair;

	private float chargeStartupTimer, chargeStartupTime = .7f;
	private float chargeTimer, chargeTime = 1f;
	private float ultTimer, ultTime = 3f;
	private float chargeSpeed = 15f, chargeDurationTimer, chargeDuration = 3.3f;
	private bool charging;

	private float firestrikeTimer, firestrikeTime = 6f;

	private Rigidbody body;
	
	void Start () {
		shield = shieldObject.GetComponent<ReinhardtShield>();
		body = GetComponent<Rigidbody>();
		firestrikeTimer = firestrikeTime;
		hammer = hammerObject.GetComponent<ReinhardtHammer>();
	}

	public bool IsSwinging()
	{
		return GetInput().GetKey(KeyCode.Mouse0);
	}

	public bool IsSwingingAnimation()
	{
		string current = hammer.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
		return current != "HammerFirestrike" && current != "IdleHammer";
	}

	void OnGUI()
	{
		if(!shield.IsActive())
		{
			float xMin = (Screen.width / 2) - (crosshair.width / 2);
			float yMin = (Screen.height / 2) - (crosshair.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
		}
	}

	void Firestrike()
	{
		hammer.animator.Play("HammerFirestrike");
		firestrikeTimer = 0;
	}

	void Swing()
	{
		if(IsHammerIdle())
			hammer.animator.Play("SwingLeftHammer");
	}

	bool CanUlt()
	{
		return ultTimer >= ultTime && !charging && IsHammerIdle() && !shield.IsActive();
	}

	void Ult()
	{
		hammer.animator.Play("Earthshatter");
		ultTimer = 0;
	}

	public void SpawnFirestrike()
	{
		Instantiate(firestrikePrefab, transform.position + firstPersonCam.transform.forward * 1.7f + new Vector3(0, .7f, 0), firstPersonCam.transform.rotation);
	}

	private bool CanFireStrike()
	{
		return firestrikeTimer >= firestrikeTime && IsHammerIdle() && !shield.IsActive();
	}

	private bool IsHammerIdle()
	{
		if (!hammerObject.activeInHierarchy)
			return false;
		string current = hammer.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
		return current == "IdleHammer";
	}

	private bool CanCharge()
	{
		return !charging && chargeTimer >= chargeTime && IsHammerIdle() && !shield.IsActive();
	}

	private void Charge()
	{
		charging = true;
		chargeTimer = 0;
		chargeDurationTimer = 0;
		firstPersonController.SetCanTurn(false);
		firstPersonController.allowMoveBody = true;
	}
	
	void Update () {

		if(!charging)
		{
			if (firestrikeTimer < firestrikeTime)
				firestrikeTimer += Time.deltaTime;
			if (chargeTimer < chargeTime)
				chargeTimer += Time.deltaTime;
			if (ultTimer < ultTime)
				ultTimer += Time.deltaTime;

			if(CanFireStrike() && GetInput().GetKeyDown(KeyCode.E))
			{
				Firestrike();
			}

			if(CanCharge() && GetInput().GetKeyDown(KeyCode.LeftShift))
			{
				Charge();
			}

			if(CanUlt() && GetInput().GetKeyDown(KeyCode.Q))
			{
				Ult();
			}

			if(IsSwinging() && !shield.IsActive())
			{
				Swing();
			}

			if(GetInput().GetKey(KeyCode.Mouse1) && (IsHammerIdle() || shield.IsActive()))
				shield.SetActive(true);
			else
				shield.SetActive(false);
		}
		else
		{
			chargeDurationTimer += Time.deltaTime;
			RaycastHit hit;
			bool hitWall = Physics.Raycast(transform.position, transform.forward, out hit, .4f, LayerMask.GetMask("Ground"));

			if(chargeDurationTimer > chargeDuration || hitWall)
			{
				charging = false;
				firstPersonController.SetCanTurn(true);
				firstPersonController.allowMoveBody = false;
			}
		}

		if(shield.IsActive() || charging)
		{
			firstPersonCam.enabled = false;
			thirdPersonCam.enabled = true;
			hammerObject.SetActive(false);
			if (charging)
				firstPersonController.movementSettings.JumpForce = 0;
			else
				firstPersonController.movementSettings.JumpForce = 55;
			firstPersonController.movementSettings.ForwardSpeed = charging ? 0 : shieldingSpeed;
			firstPersonController.movementSettings.BackwardSpeed = charging ? 0 : shieldingSpeed;
			firstPersonController.movementSettings.StrafeSpeed = charging ? 0 : shieldingSpeed;
			firstPersonController.moveCam = thirdPersonCam;
		}
		else
		{
			firstPersonCam.enabled = true;
			thirdPersonCam.enabled = false;
			hammerObject.SetActive(true);
			firstPersonController.movementSettings.JumpForce = 55;
			firstPersonController.movementSettings.ForwardSpeed = regularSpeed;
			firstPersonController.movementSettings.BackwardSpeed = regularSpeed;
			firstPersonController.movementSettings.StrafeSpeed = regularSpeed;
			firstPersonController.moveCam = firstPersonCam;
		}
	}

	void FixedUpdate()
	{
		if(charging)
		{
			Vector3 newPos = transform.position;
			newPos += transform.forward * chargeSpeed * Time.deltaTime;
			body.MovePosition(newPos);
		}
	}
}
