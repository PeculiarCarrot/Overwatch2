using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInput : HeroInput {
	
	protected override void UpdateInput ()
	{
		mouseX = CrossPlatformInputManager.GetAxis("Mouse X");
		mouseY = CrossPlatformInputManager.GetAxis("Mouse Y");

		foreach(KeyCode k in keys.Keys)
		{
			HeroKey hk = null;
			keys.TryGetValue(k, out hk);
			hk.justPressed = Input.GetKeyDown(k);
			hk.justReleased = Input.GetKeyUp(k);
			hk.down = Input.GetKey(k);
		}
	}
}
