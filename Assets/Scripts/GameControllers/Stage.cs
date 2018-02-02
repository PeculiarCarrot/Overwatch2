using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stage : MonoBehaviour {

	public List<HeroBase> heroObjects;
	private Hero[] heroes;
	
	void Start () {
		heroes = new Hero[]{
			new Hero("Reinhardt", Resources.Load<GameObject>("Heroes/ReinhardtChar")),
			new Hero("Tracer", Resources.Load<GameObject>("Heroes/TracerChar")),
		};

		heroObjects = new List<HeroBase>();
			
		//SpawnHero("Tracer", new Vector3(-23, 5, -5), true, true);

		SpawnHero("Tracer", new Vector3(-15, 6, 4), true, false);
		SpawnHero("Tracer", new Vector3(-23, 5, 3), false, false);
		SpawnHero("Tracer", new Vector3(-23, 5, 3), false, true);
		SpawnHero("Tracer", new Vector3(-23, 5, 3), false, true);
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

		if (player)
			ClientGraphics.ClientTeam = team;

		heroObjects.Add(o.GetComponent<HeroBase>());

		return o;
	}
	
	void Update () {
		
	}
}


class Hero {
	public string name;
	public GameObject prefab;

	public Hero(string name, GameObject prefab)
	{
		this.name = name;
		this.prefab = prefab;
	}
}