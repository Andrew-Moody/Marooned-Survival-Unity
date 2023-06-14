using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequirementListener : MonoBehaviour
{
	[SerializeField]
	private ItemRequirementKey _requirementKey;

	[SerializeField]
	private MeshRenderer _objectToActivate;


	private void Awake()
	{
		_requirementKey.ActivatedEvent += OnActivated;

		_objectToActivate.material.color = Color.red;
	}


	private void OnActivated(NetworkConnection activator, ItemRequirementKey activated)
	{
		_objectToActivate.material.color = Color.green;
	}
}
