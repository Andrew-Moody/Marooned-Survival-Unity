using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "ProjectileHitAbility", menuName = "AbilitySystem/Ability/ProjectileHitAbility")]
	public class ProjectileHitAbility : Ability
	{
		[SerializeField]
		private Effect _effect;

		
		public override void Activate(AbilityHandle handle)
		{
			ProjectileActivateEventData data = handle.ActivationData as ProjectileActivateEventData;

			if (data != null && data.Target != null)
			{
				Actor targetActor = data.Target.Actor;

				if (targetActor.Agent != null && handle.Actor.IsServer)
				{
					Vector3 direction = (targetActor.NetTransform.position - handle.Actor.NetTransform.position).normalized;

					direction.y += Mathf.Atan(Mathf.Deg2Rad * 30f); // add an upward component

					targetActor.Agent.KnockBack(direction, 5f);

					targetActor.Animator.SetTrigger("HIT");
				}

				ApplyEffect(handle, _effect, data.Target);
			}
		}


		private void ApplyEffect(AbilityHandle handle, Effect effect, AbilityActor target)
		{
			EffectEventData effectData = new EffectEventData()
			{
				Source = handle.User,
				Target = target
			};

			EffectHandle effectHandle = new EffectHandle(effect, effectData);

			target.ApplyEffect(effectHandle);
		}
	}
}

