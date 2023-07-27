using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestAbility", menuName = "AbilitySystem/Ability/TestAbility")]
	public class TestAbility : Ability
	{
		[SerializeField]
		private float _cooldown;

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
	}
}
