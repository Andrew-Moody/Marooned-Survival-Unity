using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualAltar : Interactable
{
	[SerializeField]
	private GameObject[] _objectsToActivate;

	[SerializeField]
	private GameObject[] _objectsToDeactivate;


	private void Awake()
	{
		_interactPrompt = "Begin the ritual";
	}


	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		MobManager.SpawnMob("Demon", transform.position, Quaternion.identity);

		for (int i = 0; i < _objectsToActivate.Length; i++)
		{
			_objectsToActivate[i].SetActive(true);
		}

		for (int i = 0; i < _objectsToDeactivate.Length; i++)
		{
			_objectsToDeactivate[i].SetActive(false);
		}
	}
}
