using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.AI;

[RequireComponent(typeof(HeroBase))]
public class TracerAIInput : HeroInput {
	
	public GameObject target;
	Vector3 lookingAt;
	HeroBase hero;
	NavMeshAgent agent;
	NavMeshPath path;
	int cornerIndex;

	void Start()
	{
		mousePosIsFinal = true;
		GetComponentInChildren<RigidbodyFirstPersonController>().mouseLook.XSensitivity = 1;
		GetComponentInChildren<RigidbodyFirstPersonController>().mouseLook.YSensitivity = 1;
		hero = GetComponent<HeroBase>();

		agent = GetComponent<NavMeshAgent>();
		path = new NavMeshPath();
		Path(target.transform.position);
	}

	private void Path(Vector3 pos)
	{
		agent.CalculatePath(target.transform.position, path);
		cornerIndex = 0;
	}

	private void MoveTowardPoint(Vector3 goal)
	{
		HeroKey hk = null;
		keys.TryGetValue(KeyCode.W, out hk);
		hk.down = true;
		Debug.DrawLine(transform.position, goal, Color.red);
		Debug.DrawLine(target.transform.position, goal, Color.green);
	}
	
	protected override void UpdateInput ()
	{
		Vector3 goal = transform.position;
		if(cornerIndex >= path.corners.Length)
			Path(target.transform.position);
		if(cornerIndex < path.corners.Length)
			goal = path.corners[cornerIndex];

		if(Vector3.Distance(transform.position, goal) < 1.6f && cornerIndex < path.corners.Length)
			cornerIndex++;
		lookingAt = goal;

		MoveTowardPoint(goal);
		for (int i = 0; i < path.corners.Length-1; i++)
			Debug.DrawLine(path.corners[i], path.corners[i+1], Color.magenta);		

		GameObject cam = hero.camera;
		//lookingAt = target.transform.position;
		Vector3 relativePos = lookingAt - cam.transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		Quaternion newRot = Quaternion.Slerp(cam.transform.rotation, rotation, .07f);
		mouseX = -newRot.eulerAngles.x;
		mouseY = newRot.eulerAngles.y;

		/*foreach(KeyCode k in keys.Keys)
		{
			HeroKey hk = null;
			keys.TryGetValue(k, out hk);
			if (k == KeyCode.R)
				continue;
			bool press = Random.Range(0, 100) < 5;
			bool release = Random.Range(0, 100) < 5;
			hk.justPressed = false;
			hk.justReleased = false;
			if(!hk.down)
				hk.justPressed = press;
			hk.down = press || (hk.down && !release);
			hk.justReleased = release;
			if (release)
				hk.down = false;
		}*/
	}
}
