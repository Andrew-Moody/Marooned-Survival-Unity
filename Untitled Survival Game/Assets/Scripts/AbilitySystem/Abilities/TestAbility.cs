using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestAbility", menuName = "AbilitySystem/Ability/TestAbility")]
	public class TestAbility : Ability
	{


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
	}
}
