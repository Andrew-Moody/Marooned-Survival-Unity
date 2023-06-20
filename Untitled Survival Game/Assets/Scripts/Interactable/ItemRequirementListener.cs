using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequirementListener : NetworkBehaviour
{
	[SerializeField]
	private RequirementMode _mode;

	[SerializeField]
	private ItemRequirementKey[] _requirementKeys;

	[SerializeField]
	private GameObject _objectToActivate;


	private void Awake()
	{
		_objectToActivate.SetActive(false);
	}


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		if (IsServer)
		{
			for (int i = 0; i < _requirementKeys.Length; i++)
			{
				_requirementKeys[i].ActivatedEvent += OnActivated;
			}
		}
	}


	private void OnActivated(NetworkConnection activator, ItemRequirementKey activated)
	{
		if (_mode == RequirementMode.Any)
		{
			ActivateORPC();
			return;
		}

		for (int i = 0; i < _requirementKeys.Length; i++)
		{
			if (!_requirementKeys[i].IsActivated)
			{
				return;
			}
		}

		ActivateORPC();
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void ActivateORPC()
	{
		_objectToActivate.SetActive(true);
	}


	private enum RequirementMode
	{
		All,
		Any
	}
}
