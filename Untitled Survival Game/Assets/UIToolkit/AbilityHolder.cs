using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AbilityHolder")]
public class AbilityHolder : ScriptableObject
{
	//[SerializeReference]
	//private IAbility _ability = new DerivedAbility();

	[SerializeReference]
	private List<IAbility> _abilities;

	
	public void AddAbility(Type type)
	{
		if (_abilities == null)
		{
			_abilities = new List<IAbility>();
		}

		_abilities.Add(Activator.CreateInstance(type) as IAbility);
	}
}
