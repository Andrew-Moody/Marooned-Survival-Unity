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
	private GameObject _objectToActivate;


	private void Awake()
	{
		for (int i = 0; i < _requirementKeys.Length; i++)
		{
			_requirementKeys[i].ActivatedEvent += OnActivated;
		}

		_objectToActivate.SetActive(false);
	}


	private void OnActivated(NetworkConnection activator, ItemRequirementKey activated)
	{
		if (_mode == RequirementMode.Any)
		{
			_objectToActivate.SetActive(true);
			return;
		}

		for (int i = 0; i < _requirementKeys.Length; i++)
		{
			if (!_requirementKeys[i].IsActivated)
			{
				return;
			}
		}

		_objectToActivate.SetActive(true);
	}


	private enum RequirementMode
	{
		All,
		Any
	}
}
