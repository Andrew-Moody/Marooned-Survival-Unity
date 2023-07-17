using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
	[SerializeReference]
	private Ability _ability = new BasicAbility();


	public Ability GetRuntimeAbility()
	{
		return AbilityFactory.CreateInstance(_ability.GetType(), _ability);
	}


	private void OnValidate()
	{
		AbilityFactory.ValidateAbility(ref _ability);
	}
}