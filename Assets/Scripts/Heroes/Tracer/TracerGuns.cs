using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerGuns : MonoBehaviour {

	private float spinTime = 0.5f;
	private float dropTime = .15f;
	private float reloadTimer, reloadTime = 1.25f;
	private bool reloading;
	public GameObject leftGun, rightGun;
	public GameObject leftGunTip, rightGunTip;
	[HideInInspector]
	public LineRenderer leftGunLine, rightGunLine;
	private Quaternion leftGunStartRot, rightGunStartRot;
	
	void Start () {
		leftGunStartRot = leftGun.transform.localRotation;
		rightGunStartRot = rightGun.transform.localRotation;
		leftGunLine = leftGun.GetComponent<LineRenderer>();
		rightGunLine = rightGun.GetComponent<LineRenderer>();
	}

	public Vector3 GetLeftGunTip()
	{
		return leftGunTip.transform.position;
	}

	public Vector3 GetRightGunTip()
	{
		return rightGunTip.transform.position;
	}

	public void Reload()
	{
		reloadTimer = 0;
		reloading = true;
	}

	public bool IsReloading()
	{
		return reloading;
	}
	
	void Update () {
		if (reloadTimer < reloadTime)
			reloadTimer += Time.deltaTime;
		else
			reloading = false;
		
		if(IsReloading())
		{
			if (reloadTimer <= spinTime)
			{
				float f = (-90 - 360) / spinTime;
				leftGun.transform.Rotate(0, 0, f * Time.deltaTime);
				rightGun.transform.Rotate(0, 0, f * Time.deltaTime);
			}
			else if(reloadTimer > reloadTime - dropTime)
			{
				float f = 90 / (dropTime);
				leftGun.transform.Rotate(0, 0, f * Time.deltaTime);
				rightGun.transform.Rotate(0, 0, f * Time.deltaTime);
			}
		}
		else
		{
			leftGun.transform.localRotation = leftGunStartRot;
			rightGun.transform.localRotation = rightGunStartRot;
		}
	}
}
