using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace LegacyAbility
{
	[MovedFrom(false, "", null, null)]
	public class BasicAbility : Ability
	{
		[SerializeField]
		private int _polyTest;

		public BasicAbility() { _abilityType = AbilityType.Basic; }

		public BasicAbility(Ability ability) : base(ability) { }

		public BasicAbility(BasicAbility basicAbility) : base(basicAbility) { _polyTest = basicAbility._polyTest; }
	}
}
