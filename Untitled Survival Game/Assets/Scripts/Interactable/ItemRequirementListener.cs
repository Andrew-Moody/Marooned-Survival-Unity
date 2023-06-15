using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequirementListener : MonoBehaviour
{
	[SerializeField]
	private RequirementMode _mode;

	[SerializeField]
	private ItemRequirementKey[] _requirementKeys;

	[SerializeField]
	private MeshRenderer _objectToActivate;


	private void Awake()
	{
		for (int i = 0; i < _requirementKeys.Length; i++)
		{
			_requirementKeys[i].ActivatedEvent += OnActivated;
		}
		
		_objectToActivate.material.color = Color.red;
	}


	private void OnActivated(NetworkConnection activator, ItemRequirementKey activated)
	{
		if (_mode == RequirementMode.Any)
		{
			_objectToActivate.material.color = Color.green;
			return;
		}

		for (int i = 0; i < _requirementKeys.Length; i++)
		{
			if (!_requirementKeys[i].IsActivated)
			{
				return;
			}
		}

		_objectToActivate.material.color = Color.green;
	}


	private enum RequirementMode
	{
		All,
		Any
	}
}
