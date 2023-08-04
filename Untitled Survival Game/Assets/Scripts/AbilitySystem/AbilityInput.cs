using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public enum AbilityInput
	{
		None,
		Primary,
		Secondary,
		AbilitySlot1,
		AbilitySlot2,
		AbilitySlot3,
		AbilitySlot4,
		AbilitySlot5,
		AbilitySlot6,
		AbilitySlot7,
		AbilitySlot8,
		AbilitySlot9,
		AbilitySlot10,
		UseImmediate,
		UseOnDeath,
		Interact,
		UseItem
	}


	[System.Serializable]
	public struct AbilityInputBinding
	{
		public AbilityInput Input => _input;
		[SerializeField] AbilityInput _input;

		public Ability Ability => _ability;
		[SerializeField] Ability _ability;
	}
}
