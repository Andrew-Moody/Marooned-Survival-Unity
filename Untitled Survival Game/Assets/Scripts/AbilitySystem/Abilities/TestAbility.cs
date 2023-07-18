using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(menuName = "AbilitySystem/TestAbility")]
	public class TestAbility : Ability
	{
		[SerializeField]
		private float _cooldown;

		public override bool CanActivate(AbilityInstanceData data)
		{
			if (data.CooldownRemaining <= 0f)
			{
				Debug.Log("TestAbility CanActivate returned true)");
				return true;
			}
			else
			{
				Debug.Log($"TestAbility CanActivate returned false with {data.CooldownRemaining} seconds remaining on cooldown");
				return false;
			}
		}


		public override void Activate(AbilityInstanceData data)
		{
			if (data.User.AsServer)
			{
				Debug.Log("TestAbility Activated AsServer");
			}
			else if (data.User.AsOwner)
			{
				Debug.Log("TestAbility Activated AsOwner");
			}

			data.CooldownRemaining = _cooldown;

			End(data);
		}


		public override void Cancel(AbilityInstanceData data)
		{
			if (data.User.AsServer)
			{
				Debug.Log("TestAbility Canceled AsServer");
			}
			else if (data.User.AsOwner)
			{
				Debug.Log("TestAbility Canceled AsOwner");
			}

			End(data);
		}


		protected override void End(AbilityInstanceData data)
		{
			if (data.User.AsServer)
			{
				Debug.Log("TestAbility End AsServer");
			}
			else if (data.User.AsOwner)
			{
				Debug.Log("TestAbility End AsOwner");
			}
		}
	}
}
