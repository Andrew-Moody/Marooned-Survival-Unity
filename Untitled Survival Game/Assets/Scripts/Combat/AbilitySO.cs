using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
	[SerializeReference]
	private Ability _ability;
	public Ability Ability { get { return _ability; } }


	private void OnValidate()
	{
		if (_ability == null)
		{
			_ability = new Ability();
		}
		else
		{
			// This is begging for a factory
			if (_ability.AbilityType == AbilityType.Melee && !(_ability.GetType() == typeof(MeleeAbility)))
			{
				_ability = new MeleeAbility(_ability);
			}
			else if (_ability.AbilityType == AbilityType.None && !(_ability.GetType() == typeof(Ability)))
			{
				_ability = new Ability(_ability);
			}
		}

		_ability.OnValidate();
	}
}