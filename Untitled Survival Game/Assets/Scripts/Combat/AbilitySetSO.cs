using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AbilitySetSO")]
public class AbilitySetSO : ScriptableObject
{
	[SerializeField]
	private AbilitySet _abilitySet;


	public Ability[] GetRuntimeAbilities()
	{
		return _abilitySet.GetRuntimeAbilities();
	}

	private void OnValidate()
	{
		_abilitySet.ValidateAbilities();
	}
}
