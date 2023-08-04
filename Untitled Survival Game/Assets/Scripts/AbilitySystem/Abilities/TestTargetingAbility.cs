using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestTargetingAbility", menuName = "AbilitySystem/Ability/TestTargetingAbility")]
	public class TestTargetingAbility : Ability
	{
		[SerializeField]
		private Effect _effect;

		[SerializeField]
		private Targeter _targeter;


		public override void Activate(AbilityHandle handle)
		{
			if (handle.User.IsServer)
			{
				Debug.Log("TestAbility Activated AsServer");
			}
			else if (handle.User.IsOwner)
			{
				Debug.Log("TestAbility Activated AsOwner");
			}

			// Use the supplied targeter to find targets
			List<TargetResult> targetResults = _targeter.FindTargets(handle.User, new TargetingArgs());

			Debug.Log($"TestTargetAbility found {targetResults.Count} targets");

			// Apply effect to each target found
			for (int i = 0; i < targetResults.Count; i++)
			{
				if (targetResults[i] is ActorTargetResult actorResult)
				{
					ApplyEffect(handle, _effect, actorResult.Actor.AbilityActor);
				}
			}

			End(handle);
		}


		public override void Cancel(AbilityHandle handle)
		{
			if (handle.User.IsServer)
			{
				Debug.Log("TestAbility Canceled AsServer");
			}
			else if (handle.User.IsOwner)
			{
				Debug.Log("TestAbility Canceled AsOwner");
			}

			End(handle);
		}


		protected override void End(AbilityHandle handle)
		{
			if (handle.User.IsServer)
			{
				Debug.Log("TestAbility End AsServer");
			}
			else if (handle.User.IsOwner)
			{
				Debug.Log("TestAbility End AsOwner");
			}

			handle.OnAbilityEnded(null);
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

