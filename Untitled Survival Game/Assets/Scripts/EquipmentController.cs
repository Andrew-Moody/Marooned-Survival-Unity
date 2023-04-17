using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EquipmentController : NetworkBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer _parentSMR;

	[SerializeField]
	private SkinnedMeshRenderer _bodySMR;

	[SerializeField]
	private SkinnedMeshRenderer _legSMR;

	[SerializeField]
	private SkinnedMeshRenderer _footSMR;

	[SerializeField]
	private SkinnedMeshRenderer _handSMR;

	private Dictionary<int, SkinnedMeshRenderer> _equipSlots;


	private void Start()
	{
		_equipSlots = new Dictionary<int, SkinnedMeshRenderer>();
		_equipSlots.Add(0, _bodySMR);
		_equipSlots.Add(1, _legSMR);
		_equipSlots.Add(2, _footSMR);
		_equipSlots.Add(3, _handSMR);

		_bodySMR.bones = _parentSMR.bones;
		_bodySMR.rootBone = _parentSMR.rootBone;

		_legSMR.bones = _parentSMR.bones;
		_legSMR.rootBone = _parentSMR.rootBone;

		_footSMR.bones = _parentSMR.bones;
		_footSMR.rootBone = _parentSMR.rootBone;

		_handSMR.bones = _parentSMR.bones;
		_handSMR.rootBone = _parentSMR.rootBone;
	}

	public void EquipItem(ItemNetData itemData)
	{
		ServerEquipItem(itemData);
	}


    [ServerRpc]
    private void ServerEquipItem(ItemNetData itemData)
	{
		ObserversEquipItem(itemData);
	}


	public void ServerUnequipItem(int slot)
	{
		ObserversUnequipItem(slot);
	}

	// Probably wont work as it will only show the latest change oof
	// Might have to make the slots seperate objects and have a collection of them here
	[ObserversRpc(BufferLast = true)]
	public void ObserversEquipItem(ItemNetData itemData)
	{
		ItemSO itemSO = ItemManager.Instance.GetItemSO(itemData.ItemID);

		if (itemSO != null)
		{
			if (itemSO.equipSlot != -1)
			{
				_equipSlots[itemSO.equipSlot].sharedMesh = itemSO.Mesh;
			}
			else if(IsOwner)
			{
				Debug.Log("Item cannot be equiped");
			}
		}
	}


	[ObserversRpc(BufferLast = true)]
	public void ObserversUnequipItem(int slot)
	{
		_equipSlots[slot].sharedMesh = null;
	}
}
