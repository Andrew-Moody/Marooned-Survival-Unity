using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "ProjectileHitAbility", menuName = "AbilitySystem/Ability/ProjectileHitAbility")]
	public class ProjectileHitAbility : Ability
	{
		[SerializeField]
		private Effect _effect;

		
		public override void Activate(AbilityHandle handle)
		{
			AbilityEventData data = handle.AbilityData.AbilityEventData;

			if (data != null && data.Target != null)
			{
				ApplyEffect(handle, _effect, data.Target);
			}
		}


		private void ApplyEffect(AbilityHandle handle, Effect effect, AbilityActor target)
		{
			EffectEventData effectData = new EffectEventData()
			{
				Source = handle.AbilityData.User,
				Target = target
			};

			EffectHandle effectHandle = new EffectHandle(effect, effectData);

			target.ApplyEffect(effectHandle);
		}
	}
}
