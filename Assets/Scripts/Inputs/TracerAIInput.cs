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
	float pathfindTimer;
	RigidbodyFirstPersonController fpsController;

	private float pathfindTime = .5f;
	int cornerIndex;
	Vector3 lastGoal;

	void Start()
	{
		mousePosIsFinal = true;
		fpsController = GetComponentInChildren<RigidbodyFirstPersonController>();
		fpsController.mouseLook.XSensitivity = 1;
		fpsController.mouseLook.YSensitivity = 1;
		hero = GetComponent<HeroBase>();

		agent = GetComponent<NavMeshAgent>();
		path = new NavMeshPath();
		lastGoal = transform.position;
		agent.enabled = false;
	}

	private void Path(Vector3 pos)
	{
		agent.enabled = true;
		agent.CalculatePath(target.transform.position, path);
		agent.enabled = false;
		cornerIndex = 0;
	}

	private float distToGround = .94f;

	private void MoveTowardPoint(Vector3 goal)
	{
		HeroKey hk = null;
		keys.TryGetValue(KeyCode.W, out hk);
		hk.down = true;
		keys.TryGetValue(KeyCode.Space, out hk);
		if (goal.y - transform.position.y + distToGround > .7f && Vector3.Distance(transform.position, goal) < 4f && target.transform.position != goal)
		{
			hk.justPressed = true;
		}
		else
			hk.justPressed = false;
		Debug.DrawLine(transform.position, goal, Color.red);
	}
		
	public GameObject GetNearestEnemy()
	{
		HeroBase[] heroes = FindObjectsOfType<HeroBase>();
		float shortest = 0;
		HeroBase closest = null;
		foreach(HeroBase b in heroes)
		{
			if (b.team == hero.team)
				continue;
			float dist = Vector3.Distance(b.transform.position, hero.transform.position);
			if(dist < shortest || closest == null)
			{
				shortest = dist;
				closest = b;
			}
		}

		return closest.gameObject;
	}
	
	protected override void UpdateInput ()
	{
		target = GetNearestEnemy();
		Vector3 goal = transform.position;
		pathfindTimer += Time.deltaTime;
		if((cornerIndex >= path.corners.Length || pathfindTimer > pathfindTime) && fpsController.Grounded)
			Path(target.transform.position);
		if (pathfindTimer > pathfindTime)
			pathfindTimer = 0;
		if(cornerIndex < path.corners.Length)
			goal = path.corners[cornerIndex];
		Debug.DrawLine(target.transform.position, transform.position, Color.green);

		if (Vector3.Distance(transform.position, goal) < 1.6f && cornerIndex < path.corners.Length)
		{
			cornerIndex++;
		}
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

		foreach(KeyCode k in keys.Keys)
		{
			HeroKey hk = null;
			keys.TryGetValue(k, out hk);
			if (k == KeyCode.R || k == KeyCode.W || k == KeyCode.Space || k == KeyCode.S)
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
		}
	}
}
