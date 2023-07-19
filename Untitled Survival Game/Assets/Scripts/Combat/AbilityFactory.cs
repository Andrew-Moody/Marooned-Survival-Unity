using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyAbility
{
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

				ability = CreateInstance(type, ability);
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
}
