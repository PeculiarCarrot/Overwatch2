using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stage : MonoBehaviour {

	[System.Serializable]
	public struct Hero{
		public string name;
		public GameObject prefab;
	}
	
	public Hero[] heroes;
	
	void Start () {
		SpawnHero("Tracer", new Vector3(-23, 5, -5), true, true);

		//SpawnHero("Tracer", new Vector3(-13, 6, 0), false, true);
		//SpawnHero("Reinhardt", new Vector3(-15, 6, 4), false, true);
		//SpawnHero("Tracer", new Vector3(-13, 6, 2), false, false);
		//SpawnHero("Reinhardt", new Vector3(-18, 6, 10), false, false);
		//SpawnHero("Reinhardt", new Vector3(-23, 5, 3), false, false);
	}

	public GameObject SpawnHero(string name, Vector3 pos, bool player, bool team)
	{
		GameObject prefab = null;
		foreach (Hero h in heroes)
			if (h.name == name)
				prefab = h.prefab;
		GameObject o = Instantiate(prefab, pos, Quaternion.identity);
		o.GetComponent<HeroBase>().ai = !player;
		o.GetComponent<HeroBase>().team = team;
		o.GetComponent<HeroBase>().Reset();
		return o;
	}
	
	void Update () {
		
	}
}
