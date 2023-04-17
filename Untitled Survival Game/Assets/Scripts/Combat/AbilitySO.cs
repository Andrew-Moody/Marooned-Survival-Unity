using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
	public Ability Ability;


	private void OnValidate()
	{
		if (Ability == null)
		{
			Ability = new Ability();
		}

		Ability.OnValidate();
	}
}