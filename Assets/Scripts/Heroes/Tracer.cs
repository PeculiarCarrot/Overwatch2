﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(TracerGuns))]
public class Tracer : HeroBase {

	public RigidbodyFirstPersonController firstPersonController;
	private float spread = .06f;
	private float blinkDistance = 8f;
	private int blinkCharges, maxBlinkCharges = 3;
	private float blinkTimer, blinkTime = 3f;
	private float rewindTimer, rewindTime = 12f;
	private float ultTimer, ultTime = 5f;
	private float rewindInterpolateTimer, rewindInterpolateTime = .02f;
	private int rewindInterpolateIndex;

	private float shootTimer, shootTime = 1 / 20f;
	private TracerGuns guns;
	public Texture crosshair;
	public GameObject pulseBombPrefab;

	private bool rewinding;

	public GameObject bulletDustPrefab;

	private Vector3[] positions = new Vector3[30];
	private Quaternion[] rotations = new Quaternion[30];
	private float savePositionTimer, savePositionTime = .1f;

	private Vector3 smoothVelocity = Vector3.zero;
	private float smoothTime = .2f;
	private float rotSmooth = 5;
	
	void Start () {
		blinkCharges = maxBlinkCharges;
		rewindTimer = rewindTime;
		blinkTimer = blinkTime;
		ultTimer = ultTime;
		SetMaxAmmo(40);
		guns = GetComponent<TracerGuns>();
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
		Instantiate(pulseBombPrefab, camera.transform.position + camera.transform.forward * .5f, camera.transform.rotation);
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
		else
		{
			firstPersonController.SetCanTurn(false);
			if (rewindInterpolateTimer < rewindInterpolateTime)
				rewindInterpolateTimer += Time.deltaTime;
			if(rewindInterpolateTimer >= rewindInterpolateTime)
			{
				transform.position = Vector3.SmoothDamp(transform.position, positions[rewindInterpolateIndex], ref smoothVelocity, smoothTime);//Vector3.Slerp(transform.position, positions[rewindInterpolateIndex], rewindInterpolateTime / rewindInterpolateTimer);
				Vector3 charRot = transform.localRotation.eulerAngles;
				Vector3 camRot = camera.transform.localRotation.eulerAngles;
				Vector3 charLerp = Quaternion.Slerp(transform.rotation, rotations[rewindInterpolateIndex], .5f).eulerAngles;//Quaternion.Slerp(transform.localRotation, rotations[rewindInterpolateIndex], rewindInterpolateTime / rewindInterpolateTimer).eulerAngles;
				Vector3 camLerp = Quaternion.Slerp(camera.transform.localRotation, rotations[rewindInterpolateIndex],  .5f).eulerAngles;
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
				}
			}
		}
	}

	void Rewind()
	{
		rewinding = true;
		rewindInterpolateTimer = 0;
		rewindInterpolateIndex = positions.Length - 1;
	}

	void OnGUI()
	{
		float xMin = (Screen.width / 2) - (crosshair.width / 2);
		float yMin = (Screen.height / 2) - (crosshair.height / 2);
		GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
	}

	public void Blink()
	{
		RaycastHit hit;
		Vector3 dir = camera.transform.forward;
		int side = 0;
		int front = 0;

		if (Input.GetKey(KeyCode.S))
			front -= 1;
		if (Input.GetKey(KeyCode.A))
			side -= 1;
		if (Input.GetKey(KeyCode.D))
			side += 1;
		if (Input.GetKey(KeyCode.W))
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
		transform.position = newPos;
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
			if (Physics.Raycast(origin, direction, out hit))
			{
				Debug.DrawLine(origin, hit.point, i == 0 ? Color.red : Color.blue);
				point = hit.point;
				Instantiate(bulletDustPrefab, hit.point, Quaternion.identity);
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("PhysicsObject"))
				{
					hit.collider.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(direction * 5, point);
				}
			}
			LineRenderer r = i == 0 ? guns.leftGunLine : guns.rightGunLine;
			r.enabled = true;
			r.SetPosition(0, origin);
			r.SetPosition(1, point);
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