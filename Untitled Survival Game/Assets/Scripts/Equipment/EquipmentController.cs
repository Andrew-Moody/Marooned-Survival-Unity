using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

using Actors;


public class EquipmentController : NetworkBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer _parentSMR;

	[SerializeField]
	private List<EquipmentSlot> _equipmentSlots;

	private Dictionary<EquipSlot, EquipmentSlot> _equipSlotsDict;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_equipSlotsDict = new Dictionary<EquipSlot, EquipmentSlot>();

		for (int i = 0; i < _equipmentSlots.Count; i++)
		{
			_equipSlotsDict.Add(_equipmentSlots[i].EquipSlot, _equipmentSlots[i]);

			_equipmentSlots[i].Initialize(_parentSMR);
		}

		Actor.FindActor(gameObject).Inventory.ItemEquipped += Inventory_ItemEquipped;
	}
	

	private void Inventory_ItemEquipped(object sender, ItemEquippedArgs args)
	{
		if (args.EquipSlot == EquipSlot.None)
		{
			Debug.LogError("Cant equip item with EquipSlot = None");
			return;
		}

		if (IsServer)
		{
			_equipSlotsDict[args.EquipSlot].ObserversEquipItem(args.Item.ItemID);
		}
	}
}
