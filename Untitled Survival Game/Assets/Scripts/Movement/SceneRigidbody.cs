using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SceneRigidbody : NetworkBehaviour
{
	private Rigidbody _rigidbody;

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_rigidbody = GetComponent<Rigidbody>();

		if(!base.IsServer)
		{
			_rigidbody.isKinematic = true;
		}
	}
}
