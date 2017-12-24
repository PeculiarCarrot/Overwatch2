using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(TracerGuns))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Tracer : HeroBase {

	public RigidbodyFirstPersonController firstPersonController;
	private float spread = .06f;
	private float blinkDistance = 8f;
	private int blinkCharges, maxBlinkCharges = 3;
	private float blinkTimer, blinkTime = 3f;
	private float rewindTimer, rewindTime = 12f;
	private float ultTimer, ultTime = 1f;
	private float rewindInterpolateTimer, rewindInterpolateTime = .005f;
	private int rewindInterpolateIndex;

	private float shootTimer, shootTime = 1 / 20f;
	private TracerGuns guns;
	public Texture crosshair;
	public GameObject pulseBombPrefab;

	private bool rewinding;

	public GameObject bulletDustPrefab;
	public GameObject recallPrefab;
	public GameObject bloodPrefab;

	private Vector3[] positions = new Vector3[30 * 3];
	private Quaternion[] rotations = new Quaternion[30 * 3];
	private float savePositionTimer, savePositionTime = .04f;

	private Vector3 smoothVelocity = Vector3.zero;
	private float smoothTime = .2f;
	private float rotSmooth = 5;

	private Quaternion startRewindCamPos, startRewindCharPos;

	public Material allyBullet, enemyBullet;

	private Collider collider;
	
	void Start () {
		blinkCharges = maxBlinkCharges;
		rewindTimer = rewindTime;
		blinkTimer = blinkTime;
		ultTimer = ultTime;
		SetMaxAmmo(40);
		guns = GetComponent<TracerGuns>();
		collider = GetComponent<Collider>();
		for (int i = 0; i < positions.Length; i++)
			positions[i] = transform.position;
		for (int i = 0; i < rotations.Length; i++)
			rotations[i] = transform.rotation;
	}

	private bool CanBlink()
	{
		return blinkCharges > 0;
	}

	private bool CanRewind()
	{
		return rewindTimer >= rewindTime;
	}

	private bool CanUlt()
	{
		return ultTimer >= ultTime;
	}

	public void Ult()
	{
		Instantiate(pulseBombPrefab, camera.transform.position + camera.transform.forward * .75f, camera.transform.rotation).layer = LayerMask.NameToLayer(team ? "Team1" : "Team2");
		ultTimer = 0;
	}
	
	void Update () {

		if(blinkTimer < blinkTime && blinkCharges < maxBlinkCharges)
		{
			blinkTimer += Time.deltaTime;
		}
		if(blinkTimer >= blinkTime)
		{
			blinkTimer = 0;
			blinkCharges++;
		}
		if (rewindTimer < rewindTime)
			rewindTimer += Time.deltaTime;
		
		if (ultTimer < ultTime)
			ultTimer += Time.deltaTime;

		if (shootTimer < shootTime)
			shootTimer += Time.deltaTime;
		
		guns.leftGunLine.enabled = false;
		guns.rightGunLine.enabled = false;
		
		if(!rewinding)
		{
			firstPersonController.SetCanTurn(true);
			body.isKinematic = false;
			collider.enabled = true;
			if (savePositionTimer < savePositionTime)
				savePositionTimer += Time.deltaTime;
			else
			{
				System.Array.Copy(positions, 1, positions, 0, positions.Length - 1);
				positions[positions.Length - 1] = transform.position;
				System.Array.Copy(rotations, 1, rotations, 0, rotations.Length - 1);
				Vector3 r = transform.localRotation.eulerAngles;
				r.x = camera.transform.localRotation.eulerAngles.x;
				rotations[rotations.Length - 1] = Quaternion.Euler(r);
				savePositionTimer = 0;
			}
			if(!IsStunned())
			{
				if(((GetInput().GetKeyDown(KeyCode.R)  && GetAmmo() < GetMaxAmmo()) || GetAmmo() == 0) && !guns.IsReloading())
				{
					Reload();
					SetAmmo(GetMaxAmmo());
				}

				if (GetInput().GetKeyDown(KeyCode.E) && CanRewind())
					Rewind();

				if((GetInput().GetKeyDown(KeyCode.Mouse1) || GetInput().GetKeyDown(KeyCode.LeftShift)) && CanBlink())
				{
					Blink();
				}

				if(GetInput().GetKey(KeyCode.Mouse0) && CanShoot())
				{
					Shoot();
				}

				if(GetInput().GetKeyDown(KeyCode.Q) && CanUlt())
				{
					Ult();
				}
			}
		}
		else
		{
			firstPersonController.SetCanTurn(false);
			body.isKinematic = true;
			collider.enabled = false;
			GameObject o = Instantiate(recallPrefab, transform.position, transform.rotation);
			o.GetComponent<ParticleSystemRenderer>().material = team ? allyBullet : enemyBullet;

			if (rewindInterpolateTimer < rewindInterpolateTime)
				rewindInterpolateTimer += Time.deltaTime;
			if(rewindInterpolateTimer >= rewindInterpolateTime)
			{
				transform.position = Vector3.SmoothDamp(transform.position, positions[rewindInterpolateIndex], ref smoothVelocity, smoothTime);//Vector3.Slerp(transform.position, positions[rewindInterpolateIndex], rewindInterpolateTime / rewindInterpolateTimer);
				Vector3 charRot = transform.localRotation.eulerAngles;
				Vector3 camRot = camera.transform.localRotation.eulerAngles;
				Vector3 charLerp = Quaternion.Lerp(startRewindCharPos, rotations[0],  1 - ((float)rewindInterpolateIndex / rotations.Length)).eulerAngles;//Quaternion.Slerp(transform.localRotation, rotations[rewindInterpolateIndex], rewindInterpolateTime / rewindInterpolateTimer).eulerAngles;
				Vector3 camLerp = Quaternion.Lerp(startRewindCamPos, rotations[0],  1 - ((float)rewindInterpolateIndex / rotations.Length)).eulerAngles;
				charRot.y = charLerp.y;
				camRot.x = camLerp.x;
				transform.localRotation = Quaternion.Euler(charRot);
				camera.transform.localRotation = Quaternion.Euler(camRot);
				//transform.rotation = rotations[rewindInterpolateIndex];
				rewindInterpolateTimer = 0;
				rewindInterpolateIndex -= 1;
				if (rewindInterpolateIndex < 0)
				{
					rewinding = false;
					rewindTimer = 0;
					foreach (Renderer r in GetComponentsInChildren<Renderer>())
						r.enabled = true;
				}
			}
		}
	}

	void Rewind()
	{
		startRewindCamPos = camera.transform.localRotation;
		startRewindCharPos = transform.localRotation;
		rewinding = true;
		rewindInterpolateTimer = 0;
		rewindInterpolateIndex = positions.Length - 1;
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
			r.enabled = false;
	}

	void OnGUI()
	{
		if(GetInput() is PlayerInput)
		{
			float xMin = (Screen.width / 2) - (crosshair.width / 2);
			float yMin = (Screen.height / 2) - (crosshair.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
		}
	}

	public void Blink()
	{
		RaycastHit hit;
		Vector3 dir = camera.transform.forward;
		int side = 0;
		int front = 0;

		if (GetInput().GetKey(KeyCode.S))
			front -= 1;
		if (GetInput().GetKey(KeyCode.A))
			side -= 1;
		if (GetInput().GetKey(KeyCode.D))
			side += 1;
		if (GetInput().GetKey(KeyCode.W))
			front += 1;

		if(front == 0 && side != 0)
		{
			dir = camera.transform.right * side;
		}
		else if(front != 0 && side == 0)
		{
			dir = camera.transform.forward * front;
		}
		else if(front != 0 && side != 0)
		{
			dir = camera.transform.forward * front;
			dir += camera.transform.right * side;
			dir *= .5f;
		}
		

		dir.y = 0;
		Vector3 newPos = transform.position;
		//Vector3 tpPos = 
		if (Physics.Raycast(transform.position, dir, out hit, blinkDistance, LayerMask.GetMask("Ground")))
		{
			newPos = hit.point;
			newPos -= dir * .35f;
		}
		else
		{
			newPos = transform.position + dir * blinkDistance;
		}
		Vector3 lastPos = transform.position;
		transform.position = newPos;
		Vector3 speed = Vector3.Normalize(transform.position - lastPos);
		body.velocity += speed * 4f;
		blinkCharges--;
	}

	public void Shoot()
	{
		SetAmmo(GetAmmo() - 2);
		RaycastHit hit;

		for(int i = 0; i < 2; i++)
		{
			Vector3 offset = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
			Vector3 origin = i == 0 ? guns.GetLeftGunTip() : guns.GetRightGunTip();
			Vector3 direction = camera.transform.forward + offset;
			Vector3 point = origin + direction * 100;
			if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, LayerMask.GetMask(team ? "Team2" : "Team1", "Ground", "PhysicsObject")))
			{
				Debug.DrawLine(origin, hit.point, i == 0 ? Color.red : Color.blue);
				point = hit.point;
				Instantiate(hit.collider.GetComponent<HeroBase>() != null && hit.collider.GetComponent<HeroBase>().team != team ? bloodPrefab : bulletDustPrefab, hit.point, Quaternion.identity);
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("PhysicsObject"))
				{
					hit.collider.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(direction * 5, point);
				}
			}
			LineRenderer r = i == 0 ? guns.leftGunLine : guns.rightGunLine;
			r.enabled = true;
			r.SetPosition(0, origin);
			r.SetPosition(1, point);
			r.material = team ? allyBullet : enemyBullet;
		}
		shootTimer = 0;
	}

	public bool CanShoot()
	{
		return !guns.IsReloading() && shootTimer >= shootTime;
	}

	void Reload()
	{
		guns.Reload();
	}
}
