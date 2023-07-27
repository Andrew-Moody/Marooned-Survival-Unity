using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestTargetingAbility", menuName = "AbilitySystem/Ability/TestTargetingAbility")]
	public class TestTargetingAbility : Ability
	{
		[SerializeField]
		private float _cooldown;

		[SerializeField]
		private Effect _effect;

		[SerializeField]
		private Targeter _targeter;


		public override bool CanActivate(AbilityHandle handle)
		{
			if (handle.AbilityData.CooldownRemaining <= 0f)
			{
				Debug.Log("TestAbility CanActivate returned true");
				return true;
			}
			else
			{
				Debug.Log($"TestAbility CanActivate returned false with {handle.AbilityData.CooldownRemaining} seconds remaining on cooldown");
				return false;
			}
		}


		public override void Activate(AbilityHandle handle)
		{
			if (handle.AbilityData.User.IsServer)
			{
				Debug.Log("TestAbility Activated AsServer");
			}
			else if (handle.AbilityData.User.IsOwner)
			{
				Debug.Log("TestAbility Activated AsOwner");
			}

			handle.AbilityData.CooldownRemaining = _cooldown;

			// Use the supplied targeter to find targets
			List<TargetResult> targetResults = _targeter.FindTargets(handle.AbilityData.User, new TargetingArgs());

			Debug.Log($"TestTargetAbility found {targetResults.Count} targets");

			// Apply effect to each target found
			for (int i = 0; i < targetResults.Count; i++)
			{
				ApplyEffect(handle, _effect, targetResults[i].Target);
			}

			End(handle);
		}


		public override void Cancel(AbilityHandle handle)
		{
			if (handle.AbilityData.User.IsServer)
			{
				Debug.Log("TestAbility Canceled AsServer");
			}
			else if (handle.AbilityData.User.IsOwner)
			{
				Debug.Log("TestAbility Canceled AsOwner");
			}

			End(handle);
		}


		protected override void End(AbilityHandle handle)
		{
			if (handle.AbilityData.User.IsServer)
			{
				Debug.Log("TestAbility End AsServer");
			}
			else if (handle.AbilityData.User.IsOwner)
			{
				Debug.Log("TestAbility End AsOwner");
			}

			handle.AbilityData.User.HandleAbilityEnd();
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

