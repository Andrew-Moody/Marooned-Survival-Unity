using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityActor = AbilitySystem.AbilityActor;

public class EquipmentSlot : NetworkBehaviour
{
	[SerializeField]
	private AbilityActor _abilityActor;

	[SerializeField]
	private EquipSlot _equipSlot;
	public EquipSlot EquipSlot { get { return _equipSlot; } }

	private AbilityItem _equippedItem;
	public AbilityItem EquipedItem { get { return _equippedItem; } }



	[SerializeField]
	private bool _isSkinnedMesh;

	private IRenderable _renderable;


	public void Initialize(SkinnedMeshRenderer parentRig)
	{
		if (_isSkinnedMesh)
		{
			_renderable = new SkinnedMeshRenderable(parentRig, gameObject);
		}
		else
		{
			_renderable = new MeshFilterRenderable(gameObject);
		}

		
	}


	[ObserversRpc(BufferLast = true, RunLocally = true)]
	public void ObserversEquipItem(ItemNetData itemNetData)
	{
		//Debug.LogError($"ObserversEquipItem: {itemNetData.ItemID}");

		if (itemNetData.ItemID != 0)
		{
			InventoryItem item = new InventoryItem(itemNetData);

			_equippedItem = item.AbilityItem;

			if (_abilityActor != null)
			{
				//_abilityActor.SetAbilityItem(item.AbilityItem);
			}

			_renderable.SetMesh(item.ItemSO.Mesh);
			_renderable.SetMaterial(item.ItemSO.Material);
		}
		else
		{
			if (_abilityActor != null)
			{
				//_abilityActor.SetAbilityItem(null);
			}

			_equippedItem = null;
			_renderable.SetMesh(null);
		}
		
	}


	[ObserversRpc(BufferLast = true)]
	public void ObserversClearSlot()
	{
		_equippedItem = null;

		_renderable.SetMesh(null);
	}
}


public enum EquipSlot
{
	None,
	Body,
	Legs,
	Hands,
	Feet,
	MainHand,
	OffHand,
	Accessory
}
