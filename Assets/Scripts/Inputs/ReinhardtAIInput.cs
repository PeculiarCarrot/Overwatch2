using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinhardtAIInput : AIInput {
	
	protected override void UpdateInput ()
	{
		HeroKey hk = null;
		keys.TryGetValue(KeyCode.Mouse1, out hk);
		hk.down = true;
	}
}
