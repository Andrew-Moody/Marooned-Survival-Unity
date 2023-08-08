using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class PlacementUI : UIPanel
{
	[SerializeField]
	private MeshFilter _meshFilter;

	[SerializeField]
	private MeshRenderer _meshRenderer;

	[SerializeField]
	private float _range;

	[SerializeField]
	private LayerMask _placementMask;

	private Transform _cameraTransform;

	private Transform _placementTransform;

	private Inventory _inventory;


	public override void Initialize()
	{
		_cameraTransform = Camera.main.transform;

		_placementTransform = transform;
	}


	public override void SetPlayer(Actor player)
	{
		base.SetPlayer(player);

		if (_cameraTransform == null)
		{
			_cameraTransform = Camera.main.transform;
		}

		if (_player != null)
		{
			_inventory = _player.Inventory;

			if (_inventory != null)
			{
				_inventory.OnHotbarSelect += SelectionChangedHandler;
			}
		}
		
	}


	private void OnDestroy()
	{
		if (_inventory != null)
		{
			_inventory.OnHotbarSelect -= SelectionChangedHandler;
		}
	}


	private void SelectionChangedHandler(int itemID)
	{
		DestructibleSO destructibleSO = DestructibleManager.Instance.GetPlacedItemSO(itemID);

		if (destructibleSO != null)
		{
			_meshFilter.sharedMesh = destructibleSO.Mesh;
			gameObject.SetActive(true);
		}
		else
		{
			_meshFilter.sharedMesh = null;
			gameObject.SetActive(false);
		}
	}


	private void LateUpdate()
	{
		if (_cameraTransform == null)
		{
			return;
		}

		Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hitInfo, _range, _placementMask);

		if (hitInfo.collider != null)
		{
			Vector3 right = Vector3.Cross(_cameraTransform.forward, hitInfo.normal).normalized;
			Vector3 forward = Vector3.Cross(right, hitInfo.normal).normalized;

			Quaternion rotation = Quaternion.LookRotation(forward, hitInfo.normal);

			_placementTransform.rotation = rotation;

			_placementTransform.position = hitInfo.point;

			_meshRenderer.enabled = true;
		}
		else
		{
			_meshRenderer.enabled = false;
		}
	}
}
