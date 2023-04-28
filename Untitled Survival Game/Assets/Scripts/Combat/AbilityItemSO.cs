using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AbilityItemSO")]
public class AbilityItemSO : ScriptableObject
{
	public string ItemName;

	public int ItemID;

	[SerializeReference]
	public Ability[] Abilities;


	public Ability[] GetAbilities()
	{
		Ability[] abilities = new Ability[Abilities.Length];

		for (int i = 0; i < Abilities.Length; i++)
		{
			if (Abilities[i] == null)
			{
				Debug.LogError($"Ability {i} was null");
			}
			else if (Abilities[i].AbilityType == AbilityType.Melee)
			{
				Debug.LogError($"Range = {(Abilities[i] as MeleeAbility).Range}");
			}

			abilities[i] = Abilities[i].CreateCopy();
		}

		return abilities;
	}

	private void OnValidate()
	{
		for (int i = 0; i < Abilities.Length; i++)
		{
			if (Abilities[i] == null)
			{
				//Debug.LogWarning($"Ability {i} was null");
				Abilities[i] = new Ability();
			}
			else
			{
				if (Abilities[i].AbilityType == AbilityType.Melee && !(Abilities[i].GetType() == typeof(MeleeAbility)))
				{
					//Debug.LogWarning($"MeleeAbility(Ability) called");
					Abilities[i] = new MeleeAbility(Abilities[i]);
				}
				else if (Abilities[i].AbilityType == AbilityType.None && !(Abilities[i].GetType() == typeof(Ability)))
				{
					//Debug.LogWarning($"Ability(Ability) called ");
					Abilities[i] = new Ability(Abilities[i]);
				}
			}


			//Debug.LogWarning($"Ability.OnValidate");

			Abilities[i].OnValidate();
		}
	}
}
