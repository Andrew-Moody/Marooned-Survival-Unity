using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFactory
{
	private static AbilityFactory _instance;

	delegate Ability AbilityFactoryMethod();

	delegate Ability AbilityCopyMethod(Ability ability);

	private Dictionary<AbilityType, Type> _abilityTypes;

	private Dictionary<AbilityType, AbilityFactoryMethod> _factoryMethods;

	private Dictionary<AbilityType, AbilityCopyMethod> _copyMethods;

	public AbilityFactory()
	{
		_abilityTypes = new Dictionary<AbilityType, Type>
		{
			{ AbilityType.Basic, typeof(BasicAbility) },
			{ AbilityType.Melee, typeof(MeleeAbility) },
			// { AbilityType.Range, typeof(RangeAbility) },
			// { AbilityType.Magic, typeof(MagicAbility) },
			// { AbilityType.Combo, typeof(ComboAbility) }
		};


		_factoryMethods = new Dictionary<AbilityType, AbilityFactoryMethod>
		{
			{ AbilityType.Basic, BasicAbility.Create },
			{ AbilityType.Melee, MeleeAbility.Create },
			// { AbilityType.Range, RangeAbility.Create },
			// { AbilityType.Magic, MagicAbility.Create },
			// { AbilityType.Combo, ComboAbility.Create },
		};


		_copyMethods = new Dictionary<AbilityType, AbilityCopyMethod>
		{
			{ AbilityType.Basic, BasicAbility.CreateFrom },
			{ AbilityType.Melee, MeleeAbility.CreateFrom },
			// { AbilityType.Range, RangeAbility.CreateFrom },
			// { AbilityType.Magic, MagicAbility.CreateFrom },
			// { AbilityType.Combo, ComboAbility.CreateFrom },
		};
	}


	public static Ability CreateAbility(AbilityType abilityType)
	{
		if (_instance == null)
		{
			_instance = new AbilityFactory();
		}

		if (_instance._factoryMethods.TryGetValue(abilityType, out AbilityFactoryMethod factoryMethod))
		{
			return factoryMethod.Invoke();
		}

		Debug.LogError($"AbilityFactory is a missing a factory method for AbilityType.{abilityType}");
		return null;
	}


	// It may be that simple
	public static Ability CreateInstance(Type type, Ability ability = null)
	{
		if (ability == null)
		{
			return Activator.CreateInstance(type) as Ability;
		}

		return Activator.CreateInstance(type, ability) as Ability;
	}


	
	public static Ability CreateAbilityFrom(AbilityType abilityType, Ability ability)
	{
		if (_instance == null)
		{
			_instance = new AbilityFactory();
		}

		if (_instance._copyMethods.TryGetValue(abilityType, out AbilityCopyMethod copyMethod))
		{
			return copyMethod.Invoke(ability);
		}

		Debug.LogError($"AbilityFactory is a missing a copy method for AbilityType.{abilityType}");
		return ability;
	}


	public static void ValidateAbility(ref Ability ability)
	{
		if (_instance == null)
		{
			_instance = new AbilityFactory();
		}

		if (ability == null)
		{
			ability = new BasicAbility();
			return;
		}

		if (!_instance._abilityTypes.TryGetValue(ability.AbilityType, out Type type))
		{
			Debug.LogError($"AbilityFactory is a missing a type entry for AbilityType.{ability.AbilityType} on {ability.AbilityName}");
			return;
		}

		if (ability.GetType() != type)
		{
			Debug.LogError($"Ability {ability.AbilityName} mismatched type: current {ability.GetType()}, desired {type}, enum {ability.AbilityType}");
			ability = CreateAbilityFrom(ability.AbilityType, ability);
			
		}

		ability.ValidateEffects();
	}
}


public enum AbilityType
{
	None,
	Basic,
	Melee,
	Range,
	Magic,
	Combo
}


public enum ToolType
{
	None,
	Mining,
	Logging
}
