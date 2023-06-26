using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAbility : Ability
{
	[SerializeField]
	private int _polyTest;

	public BasicAbility() { _abilityType = AbilityType.Basic; }

	public BasicAbility(Ability ability) : base(ability) { }

	public BasicAbility(BasicAbility basicAbility) : base(basicAbility) { _polyTest = basicAbility._polyTest; }


	public static Ability Create()
	{
		return new BasicAbility();
	}


	public static Ability CreateFrom(Ability ability)
	{
		return new BasicAbility(ability);
	}


	public override Ability CreateCopy()
	{
		return new BasicAbility(this);
	}
}
