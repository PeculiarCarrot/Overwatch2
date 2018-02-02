using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payload : MonoBehaviour {

	private static float contestRadius = 7;

	private float timeSinceFriendlyContested;
	private Rigidbody body;
	public Stage stage;
	public bool team;
	
	void Start () {
		body = GetComponent<Rigidbody>();
	}

	void Update () {
		int contestingFriendlies = 0;
		int contestingEnemies = 0;
		foreach(HeroBase hero in stage.heroObjects)
		{
			if(Vector3.Distance(hero.transform.position, transform.position) <= contestRadius)
			{
				if (hero.team == team)
					contestingFriendlies++;
				else
					contestingEnemies++;
			}
		}

		timeSinceFriendlyContested += Time.deltaTime;

		float speed = 1;

		if (contestingFriendlies > 0)
			timeSinceFriendlyContested = 0;

		if (contestingEnemies == 0)
		{
			if (contestingFriendlies == 0)
			{
				if (timeSinceFriendlyContested >= 10)
					speed *= -.5f;
				else
					speed = 0;
			}
			else if (contestingFriendlies == 1)
				speed *= 1;
			else if (contestingFriendlies == 2)
				speed *= 1.165f;
			if (contestingFriendlies >= 3)
				speed *= 1.33f;
		}
		else
			speed = 0;

		Move(speed);
	}

	void Move(float speed)
	{
		body.MovePosition(transform.position + new Vector3(0, 0, -speed * Time.deltaTime));
	}
}
