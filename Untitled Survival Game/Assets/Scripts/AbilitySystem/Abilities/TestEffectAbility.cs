using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestEffectAbility", menuName = "AbilitySystem/Ability/TestEffectAbility")]
	public class TestEffectAbility : Ability
	{
		[SerializeField]
		private float _cooldown;

		[SerializeField]
		private Effect _effect;

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
			if (handle.AbilityData.User.AsServer)
			{
				Debug.Log("TestAbility Activated AsServer");
			}
			else if (handle.AbilityData.User.AsOwner)
			{
				Debug.Log("TestAbility Activated AsOwner");
			}

			handle.AbilityData.CooldownRemaining = _cooldown;

			// Apply effect to self
			ApplyEffect(handle, _effect, handle.AbilityData.User);

			End(handle);
		}


		public override void Cancel(AbilityHandle handle)
		{
			if (handle.AbilityData.User.AsServer)
			{
				Debug.Log("TestAbility Canceled AsServer");
			}
			else if (handle.AbilityData.User.AsOwner)
			{
				Debug.Log("TestAbility Canceled AsOwner");
			}

			End(handle);
		}


		protected override void End(AbilityHandle handle)
		{
			if (handle.AbilityData.User.AsServer)
			{
				Debug.Log("TestAbility End AsServer");
			}
			else if (handle.AbilityData.User.AsOwner)
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
