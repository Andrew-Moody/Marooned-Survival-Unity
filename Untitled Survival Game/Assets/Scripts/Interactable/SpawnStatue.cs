using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStatue : Interactable
{
	[SerializeField]
	private string _mobToSpawn;


	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		MobManager.SpawnMob(_mobToSpawn, transform.position, transform.rotation);
	}
}
