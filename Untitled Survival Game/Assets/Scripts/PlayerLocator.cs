using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocator : NetworkBehaviour
{
	private static GameObject _player;

	public static GameObject Player => _player;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		if (Owner.IsLocalClient)
		{
			Debug.Log("Initializing PlayerLocator");
			_player = gameObject;
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
