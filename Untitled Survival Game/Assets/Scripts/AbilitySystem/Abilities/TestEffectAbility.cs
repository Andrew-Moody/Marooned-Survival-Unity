using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestEffectAbility", menuName = "AbilitySystem/Ability/TestEffectAbility")]
	public class TestEffectAbility : Ability
	{
		[SerializeField]
		private Effect _effect;


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

			// Apply effect to self
			ApplyEffect(handle, _effect, handle.User);

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
