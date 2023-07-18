using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(menuName = "AbilitySystem/BasicAbility")]
	public class BasicAbility : Ability
	{
		[SerializeField]
		private float _cooldown;

		public override bool CanActivate(AbilityInstanceData data)
		{
			return data.CooldownRemaining <= 0f;
		}


		public override void Activate(AbilityInstanceData data)
		{
			Debug.Log("Ability Activate");

			data.CooldownRemaining = _cooldown;

			End(data);
		}
	}
}

