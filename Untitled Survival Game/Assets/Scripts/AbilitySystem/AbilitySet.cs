using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class AbilitySet
	{
		private AbilityActor _user;


		// Instantiated ability instances
		private Dictionary<AbilityInput, AbilityHandle> _abilityDefaults;

		// Overide ability input binding
		private Dictionary<AbilityInput, AbilityHandle> _abilityOverrides;


		private Dictionary<EquipSlot, ItemHandle> _equippedItems;


		public AbilitySet(AbilityActor user, List<AbilityInputBinding> defaultAbilities, Inventory inventory)
		{
			_user = user;			

			SetDefaultAbilities(defaultAbilities);

			if (inventory != null)
			{
				_equippedItems = new Dictionary<EquipSlot, ItemHandle>();

				inventory.ItemEquipped += Inventory_ItemEquipped;
			}
		}


		public void SetDefaultAbilities(List<AbilityInputBinding> abilities)
		{
			_abilityDefaults = new Dictionary<AbilityInput, AbilityHandle>();

			foreach (AbilityInputBinding binding in abilities)
			{
				AbilityHandle handle = new AbilityHandle(binding.Ability, _user, binding.Input);

				_abilityDefaults.Add(binding.Input, handle);
			}

			_abilityOverrides = new Dictionary<AbilityInput, AbilityHandle>();
		}


		public bool TryFindAbility(AbilityInput abilityInput, out AbilityHandle abilityHandle)
		{
			if (_abilityOverrides.TryGetValue(abilityInput, out abilityHandle))
			{
				return true;
			}

			return _abilityDefaults.TryGetValue(abilityInput, out abilityHandle);
		}


		public void SetAbilityOverride(AbilityHandle handle)
		{
			_abilityOverrides[handle.InputBinding] = handle;
		}


		public void RemoveAbilityOverride(AbilityHandle handle)
		{
			_abilityOverrides.Remove(handle.InputBinding);
		}


		public void SetAbilityOverrides(List<AbilityHandle> handles)
		{
			foreach (AbilityHandle handle in handles)
			{
				_abilityOverrides[handle.InputBinding] = handle;
			}
		}


		public void RemoveAbilityOverrides(List<AbilityHandle> handles)
		{
			foreach (AbilityHandle handle in handles)
			{
				_abilityOverrides.Remove(handle.InputBinding);
			}
		}


		private void Inventory_ItemEquipped(object sender, ItemEquippedArgs args)
		{
			// Remove the abilities from the previous item
			if (_equippedItems.TryGetValue(args.EquipSlot, out ItemHandle prevItem))
			{
				RemoveAbilityOverrides(prevItem.AbilityHandles);
			}

			// Add the new item and set the appropriate overrides (currently assumes a dummy item exist for case when no item is equipped)
			_equippedItems[args.EquipSlot] = args.Item;

			SetItemActivationData(args.Item);

			SetAbilityOverrides(args.Item.AbilityHandles);
		}


		private void SetItemActivationData(ItemHandle item)
		{
			// Setup abilityHandles for the item if not already done
			if (item.AbilityHandles == null)
			{
				item.CreateAbilityHandles(_user);
			}

			// Eventually the abilityHandles may be shared between multiple items
			// Whenever an item is equipped the abilityHandle must have the activation data updated to point
			// the currently equipped item
			foreach (AbilityHandle handle in item.AbilityHandles)
			{
				handle.ActivationData = new ItemActivateEventData() { Item = item };
			}
		}
	}
}
