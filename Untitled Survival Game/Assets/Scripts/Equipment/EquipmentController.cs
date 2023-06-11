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

	private Dictionary<EquipSlot, EquipmentSlot> _equipSlotsDict;

	//private Dictionary<EquipSlot, AbilityItem> _equipedItems;

	[SerializeField]
	private List<EquipmentSlot> _equipmentSlots;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_equipSlotsDict = new Dictionary<EquipSlot, EquipmentSlot>();

		for (int i = 0; i < _equipmentSlots.Count; i++)
		{
			_equipSlotsDict.Add(_equipmentSlots[i].EquipSlot, _equipmentSlots[i]);

			_equipmentSlots[i].Initialize(_parentSMR);
		}
	}


	public AbilityItem GetItemAtSlot(EquipSlot slot)
	{
		return _equipSlotsDict[slot].EquipedItem;
	}


	public void EquipItem(InventoryItem item, EquipSlot slot)
	{
		if (slot == EquipSlot.None)
		{
			Debug.LogError("Cant equip item with EquipSlot = None");
		}

		if (IsServer)
		{
			//Debug.LogError($"EquipItem: {item.ItemID}");
			_equipSlotsDict[slot].ObserversEquipItem(item.GetNetData());
		}
	}


	public void UnequipItem(EquipSlot slot)
	{
		if (IsServer)
		{
			_equipSlotsDict[slot].ObserversClearSlot();
		}
	}


	private void Update()
	{
		foreach (KeyValuePair<EquipSlot, EquipmentSlot> pair in _equipSlotsDict)
		{
			if (pair.Value.EquipedItem != null)
			{
				foreach (Ability ability in pair.Value.EquipedItem.Abilities)
				{
					ability.TickAbility(Time.deltaTime);
				}
			}
		}
	}
}
