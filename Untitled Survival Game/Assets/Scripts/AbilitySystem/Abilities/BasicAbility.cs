using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewBasicAbility", menuName = "AbilitySystem/Ability/BasicAbility")]
	public class BasicAbility : Ability
	{
		[SerializeField]
		private float _cooldown;

		public override bool CanActivate(AbilityHandle handle)
		{
			return handle.AbilityData.CooldownRemaining <= 0f;
		}


		public override void Activate(AbilityHandle handle)
		{
			Debug.Log("Ability Activate");

			handle.AbilityData.CooldownRemaining = _cooldown;

			End(handle);
		}
	}
}

