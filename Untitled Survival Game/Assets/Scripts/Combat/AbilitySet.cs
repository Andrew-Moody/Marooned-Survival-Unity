using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilitySet
{
	[SerializeField]
	private AbilitySetSO _abilitySetSO;

	[SerializeField]
	private AbilitySO[] _abilitySOList;

	[SerializeReference]
	private Ability[] _abilities;


	public Ability[] GetRuntimeAbilities()
	{
		// Get abilities from SO if one exists
		if (_abilitySetSO != null)
		{
			return _abilitySetSO.GetRuntimeAbilities();
		}

		Ability[] abilities = new Ability[_abilitySOList.Length + _abilities.Length];

		for (int i = 0; i < _abilitySOList.Length; i++)
		{
			if (_abilitySOList[i] != null)
			{
				abilities[i] = _abilitySOList[i].GetRuntimeAbility();
			}
		}

		for (int i = 0; i < _abilities.Length; i++)
		{
			if (_abilities[i] != null)
			{
				abilities[i] = _abilities[i].CreateCopy();
			}
		}

		return abilities;
	}


	public void ValidateAbilities()
	{
		if (_abilities == null)
		{
			return;
		}

		for (int i = 0; i < _abilities.Length; i++)
		{
			AbilityFactory.ValidateAbility(ref _abilities[i]);
		}
	}
}
