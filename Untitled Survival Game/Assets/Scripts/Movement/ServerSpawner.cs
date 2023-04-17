using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSpawner : NetworkBehaviour
{
	[SerializeField]
	private CSPObject _prefab;

	private CSPObject _obj;

	public override void OnStartServer()
	{
		base.OnStartServer();

		_obj = Instantiate(_prefab, transform, false);

		Spawn(_obj.gameObject);

		Debug.Log("Spawned Scene RB at: " + _obj.transform.position);
	}


	public override void OnStartClient()
	{
		base.OnStartClient();

		Debug.Log("Scene RB location: " + _obj.transform.position);
	}
}
