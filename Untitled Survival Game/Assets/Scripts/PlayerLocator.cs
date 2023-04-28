using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocator : NetworkBehaviour
{
	public static GameObject Player;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		if (Owner.IsLocalClient)
		{
			Debug.Log("Initializing PlayerLocator");
			Player = gameObject;
		}
	}
}
