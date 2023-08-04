using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "ConsumeItemAbility", menuName = "AbilitySystem/Ability/ConsumeItemAbility")]
	public class ConsumeItemAbility : Ability
	{
		[SerializeField] private Effect _consumeEffect;

		public override void Activate(AbilityHandle handle)
		{
			ConsumeItem(handle);

			End(handle);
		}


		private void ConsumeItem(AbilityHandle handle)
		{
			ItemHandle item = null;

			if (handle.AbilityData.AbilityEventData is ItemActivateEventData data)
			{
				item = data.Item;
			}
			else
			{
				Debug.LogError("Attempting to use a item based ability but item data was missing");
				return;
			}

			if (item == null)
			{
				Debug.LogError("Attempting to use a item based ability but item data was missing an itemhandle");
				return;
			}


			EffectEventData effectData = new EffectEventData()
			{
				Source = handle.User,
				Target = handle.User
			};

			EffectHandle effect = new EffectHandle(_consumeEffect, effectData);

			handle.User.ApplyEffect(effect);


			// Finally need to deduct quantity from the item in the inventory
			item.TryRemoveItem(1);
		}
	}
}

