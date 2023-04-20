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

	private Dictionary<int, SkinnedMeshRenderer> _equipSlotsDict;


	[SerializeField]
	private List<EquipmentSlot> _equipmentSlots;


	private void Start()
	{
		_equipSlotsDict = new Dictionary<int, SkinnedMeshRenderer>();
		_equipSlotsDict.Add(0, _bodySMR);
		_equipSlotsDict.Add(1, _legSMR);
		_equipSlotsDict.Add(2, _footSMR);
		_equipSlotsDict.Add(3, _handSMR);

		_bodySMR.bones = _parentSMR.bones;
		_bodySMR.rootBone = _parentSMR.rootBone;

		_legSMR.bones = _parentSMR.bones;
		_legSMR.rootBone = _parentSMR.rootBone;

		_footSMR.bones = _parentSMR.bones;
		_footSMR.rootBone = _parentSMR.rootBone;

		_handSMR.bones = _parentSMR.bones;
		_handSMR.rootBone = _parentSMR.rootBone;


		for (int i = 0; i < _equipmentSlots.Count; i++)
		{
			//_equipSlotsDict.Add(i, _equipmentSlots[i]);

			_equipmentSlots[i].Initialize(_parentSMR);
		}
	}


    [Server]
    public void ServerEquipItem(ItemNetData itemNetData)
	{
		ItemSO itemSO = ItemManager.Instance.GetItemSO(itemNetData.ItemID);

		if (itemSO.equipSlot != -1)
		{
			_equipmentSlots[itemSO.equipSlot].ObserversEquipItem(itemNetData);
		}
	}


	[Server]
	public void ServerUnequipItem(int slot)
	{
		_equipmentSlots[slot].ObserversClearSlot();
	}
}
