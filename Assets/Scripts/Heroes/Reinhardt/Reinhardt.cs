using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Reinhardt : HeroBase {

	public RigidbodyFirstPersonController firstPersonController;
	public GameObject shieldObject, hammerObject;
	public GameObject firestrikePrefab;
	public GameObject earthShatterProjectilePrefab;
	public GameObject chargeHitbox;
	private ReinhardtHammer hammer;
	private ReinhardtShield shield;
	private float regularSpeed = 6f, shieldingSpeed = 2.75f;
	public Texture crosshair;

	private float chargeStartupTimer, chargeStartupTime = .5f;
	private float chargeTimer, chargeTime = 1f;
	private float ultTimer, ultTime = 3f;
	private float chargeSpeed = 15f, chargeDurationTimer, chargeDuration = 3.3f;
	private float chargeTurnSpeed = 20f;
	private float unmodifiedJumpForce;
	private bool charging;

	private bool shatterEnabled;
	private bool ulting;

	private float firestrikeTimer, firestrikeTime = 6f;
	
	void Start () {
		shield = shieldObject.GetComponent<ReinhardtShield>();
		firestrikeTimer = firestrikeTime;
		hammer = hammerObject.GetComponent<ReinhardtHammer>();
		unmodifiedJumpForce = firstPersonController.movementSettings.JumpForce;
	}

	public ReinhardtHammer GetHammer()
	{
		return hammer;
	}

	public ReinhardtShield GetShield()
	{
		return shield;
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

	public bool CanLandEarthshatter()
	{
		return shatterEnabled;
	}

	public void EnableShatter()
	{
		shatterEnabled = true;
	}

	public void EarthShatter()
	{
		ulting = false;

		float spread = 90;
		float num = 5;
		float anglePer = spread / num;
		float angle = -anglePer * 2;

		for (int i = 0; i < num; i++)
		{
			Instantiate(earthShatterProjectilePrefab, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, angle, 0)));
			angle += anglePer;
		}
	}

	void OnGUI()
	{
		if(GetInput() is PlayerInput)
		{
			if(!shield.IsActive() && !charging)
			{
				float xMin = (Screen.width / 2) - (crosshair.width / 2);
				float yMin = (Screen.height / 2) - (crosshair.height / 2);
				GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
			}
		}
	}

	void Firestrike()
	{
		shield.SetActive(false);
		firstPersonCam.enabled = true;
		thirdPersonCam.enabled = false;
		hammerObject.SetActive(true);
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
		return ultTimer >= ultTime && !charging && (IsHammerIdle() || shield.IsActive());
	}

	void Ult()
	{
		shield.SetActive(false);
		firstPersonCam.enabled = true;
		thirdPersonCam.enabled = false;
		hammerObject.SetActive(true);
		hammer.animator.Play("Earthshatter");
		ultTimer = 0;
		ulting = true;
	}

	public void SpawnFirestrike()
	{
		Instantiate(firestrikePrefab, transform.position + firstPersonCam.transform.forward * 1.7f + new Vector3(0, .7f, 0), firstPersonCam.transform.rotation).layer = LayerMask.NameToLayer(team ? "Team1" : "Team2");
	}

	private bool CanFireStrike()
	{
		return firestrikeTimer >= firestrikeTime && (IsHammerIdle() || shield.IsActive());
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
		return !charging && chargeTimer >= chargeTime && (IsHammerIdle() || shield.IsActive());
	}

	private HeroBase stuckHero;

	public void ChargeStick(HeroBase hero)
	{
		hero.SetBeingCharged(true);
		stuckHero = hero;
	}

	private void Charge()
	{
		shield.SetActive(false);
		charging = true;
		chargeTimer = 0;
		chargeStartupTimer = 0;
		chargeDurationTimer = 0;
		firstPersonController.SetCanTurn(false);
		firstPersonController.allowMoveBody = true;

		if(firstPersonController.IsGrounded())
		{
			firstPersonController.Jump(.5f);
		}
	}
	
	void Update () {
		base.Update();
		if(CanLandEarthshatter() && firstPersonController.IsGrounded())
		{
			hammer.animator.Play("EarthShatterComplete");
			shatterEnabled = false;
		}
		if(ulting && firstPersonController.IsGrounded())
		{
			firstPersonController.SetCanMove(false);
		}
		else
		{
			firstPersonController.SetCanMove(true);
		}

		if(!charging)
		{
			if (firestrikeTimer < firestrikeTime)
				firestrikeTimer += Time.deltaTime;
			if (chargeTimer < chargeTime)
				chargeTimer += Time.deltaTime;
			if (ultTimer < ultTime)
				ultTimer += Time.deltaTime;

			if(!IsStunned() && !ulting)
			{

				if(GetInput().GetKey(KeyCode.Mouse1) && !IsStunned() && (IsHammerIdle() || shield.IsActive()))
					shield.SetActive(true);
				else
					shield.SetActive(false);
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

				if(IsSwinging())
				{
					Swing();
				}
			}
		}
		else
		{
			chargeDurationTimer += Time.deltaTime;
			chargeStartupTimer += Time.deltaTime;

			if(GetInput().GetKey(KeyCode.A))
				transform.Rotate(0, -chargeTurnSpeed * Time.deltaTime, 0);
			if(GetInput().GetKey(KeyCode.D))
				transform.Rotate(0, chargeTurnSpeed * Time.deltaTime, 0);

			if(chargeStartupTimer >= chargeStartupTime)
			{
				RaycastHit hit;
				bool hitWall = Physics.Raycast(transform.position, transform.forward, out hit, stuckHero == null ? .4f : 1.4f, LayerMask.GetMask("Ground"));
				chargeHitbox.SetActive(true);

				if(chargeDurationTimer > chargeDuration + chargeStartupTime || hitWall)
				{
					charging = false;
					firstPersonController.SetCanTurn(true);
					firstPersonController.allowMoveBody = false;
					if (stuckHero != null)
						stuckHero.SetBeingCharged(false);
					stuckHero = null;
				}
			}
		}
		if(!charging)
			chargeHitbox.SetActive(false);

		if(shield.IsActive() || charging)
		{
			ToThirdPerson();
			hammerObject.SetActive(false);
			if (charging)
				firstPersonController.movementSettings.JumpForce = 0;
			else
				firstPersonController.movementSettings.JumpForce = unmodifiedJumpForce;
			firstPersonController.movementSettings.ForwardSpeed = charging ? 0 : shieldingSpeed;
			firstPersonController.movementSettings.BackwardSpeed = charging ? 0 : shieldingSpeed;
			firstPersonController.movementSettings.StrafeSpeed = charging ? 0 : shieldingSpeed;
			firstPersonController.moveCam = thirdPersonCam;
		}
		else
		{
			ToFirstPerson();
			hammerObject.SetActive(true);
			firstPersonController.movementSettings.JumpForce = unmodifiedJumpForce;
			firstPersonController.movementSettings.ForwardSpeed = regularSpeed;
			firstPersonController.movementSettings.BackwardSpeed = regularSpeed;
			firstPersonController.movementSettings.StrafeSpeed = regularSpeed;
			firstPersonController.moveCam = firstPersonCam;
		}

		if(ai)
		{
			firstPersonCam.enabled = false;
			thirdPersonCam.enabled = false;
		}

		if(shield.IsActive())
		{
			firstPersonController.mouseLook.MaximumX = 30;
			firstPersonController.mouseLook.MinimumX = -89;

			Vector3 rot = firstPersonCam.transform.rotation.eulerAngles;
			if (rot.x > 108)
				rot.x -= 360;
			rot.x = Mathf.Clamp(rot.x, -89, 0);
			firstPersonCam.transform.rotation = Quaternion.Euler(rot);
		}
		else
		{
			firstPersonController.mouseLook.MaximumX = 89;
			firstPersonController.mouseLook.MinimumX = -89;
		}
	}
		
	void LateUpdate () {
	}

	public new void FixedUpdate()
	{
		base.FixedUpdate();
		if(charging && chargeStartupTimer >= chargeStartupTime)
		{
			Vector3 newPos = transform.position;
			newPos += transform.forward * chargeSpeed * Time.deltaTime;
			body.MovePosition(newPos);

			if(stuckHero != null)
			{
				newPos = stuckHero.transform.position;
				newPos = transform.position + camera.transform.forward * 1f;
				stuckHero.GetComponent<Rigidbody>().MovePosition(newPos);
			}
		}
	}
}
