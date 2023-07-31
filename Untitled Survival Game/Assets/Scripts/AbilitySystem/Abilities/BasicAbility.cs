using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewBasicAbility", menuName = "AbilitySystem/Ability/BasicAbility")]
	public class BasicAbility : Ability
	{
		public override void Activate(AbilityHandle handle)
		{
			Debug.Log("Ability Activate");

			End(handle);
		}
	}
}

