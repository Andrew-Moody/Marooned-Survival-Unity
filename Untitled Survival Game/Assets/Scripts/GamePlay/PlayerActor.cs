using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class PlayerActor : Actor
{
	public static Actor Player => _player;
	private static Actor _player;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		if (Owner.IsLocalClient)
		{
			Debug.Log("Initializing PlayerLocator");
			_player = this;
		}
	}


	public override void OnStartClient()
	{
		base.OnStartClient();

		if (IsOwner)
		{
			GameManager.Instance.OnLocalPlayerStartClient(_player);
		}
	}
}
