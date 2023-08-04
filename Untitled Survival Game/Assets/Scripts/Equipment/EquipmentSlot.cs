using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : NetworkBehaviour
{
	public EquipSlot EquipSlot => _equipSlot;
	[SerializeField] private EquipSlot _equipSlot;

	[SerializeField] private bool _isSkinnedMesh;

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
	public void ObserversEquipItem(int itemID)
	{
		ItemSO itemSO = ItemManager.Instance.GetItemSO(itemID);

		if (itemSO != null)
		{
			_renderable.SetMesh(itemSO.Mesh);
			_renderable.SetMaterial(itemSO.Material);
		}
		else
		{
			_renderable.SetMesh(null);
		}
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
