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

	public event System.Action RitualStartedEvent;


	private void Awake()
	{
		_interactPrompt = "Begin the ritual";
	}


	public override void Interact(NetworkConnection user)
	{
		base.Interact(user);

		RitualStartedEvent?.Invoke();

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
