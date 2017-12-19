using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class HeroInput : MonoBehaviour {

	public bool mousePosIsFinal;
	protected float mouseX, mouseY;
	protected Dictionary<KeyCode, HeroKey> keys = new Dictionary<KeyCode, HeroKey>();

	void Awake () {
		GetComponent<NavMeshAgent>().enabled = false;
		keys.Add(KeyCode.E, new HeroKey());
		keys.Add(KeyCode.Q, new HeroKey());
		keys.Add(KeyCode.Mouse0, new HeroKey());
		keys.Add(KeyCode.Mouse1, new HeroKey());
		keys.Add(KeyCode.LeftShift, new HeroKey());
		keys.Add(KeyCode.W, new HeroKey());
		keys.Add(KeyCode.A, new HeroKey());
		keys.Add(KeyCode.S, new HeroKey());
		keys.Add(KeyCode.D, new HeroKey());
		keys.Add(KeyCode.R, new HeroKey());
		keys.Add(KeyCode.Space, new HeroKey());
	}

	public bool GetKey(KeyCode k)
	{
		HeroKey hk = null;
		keys.TryGetValue(k, out hk);
		return hk.down;
	}

	public bool GetKeyUp(KeyCode k)
	{
		HeroKey hk = null;
		keys.TryGetValue(k, out hk);
		return hk.justReleased;
	}

	public bool GetKeyDown(KeyCode k)
	{
		HeroKey hk = null;
		if(!keys.TryGetValue(k, out hk))
		{
			Debug.LogError("No keycode found: " + k);
			return false;
		}
		return hk.justPressed;
	}

	public float GetMouseX()
	{
		return mouseX;
	}

	public float GetMouseY()
	{
		return mouseY;
	}
	
	void Update () {
		UpdateInput();
	}

	protected abstract void UpdateInput();
}
