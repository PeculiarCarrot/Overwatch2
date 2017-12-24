using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinhardtChargeHitbox : MonoBehaviour {

	public Reinhardt rein;

	// Use this for initialization
	void Start () {
		gameObject.layer = LayerMask.NameToLayer(rein.team ? "Team1" : "Team2");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void OnTriggerEnter(Collider other)
	{
		HeroBase hero = other.GetComponent<HeroBase>();
		if (hero != null && hero.team != rein.team)
		{
			rein.ChargeStick(hero);
		}
	}
}
