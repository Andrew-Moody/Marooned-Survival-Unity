using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
	public int ItemID;

	public string ItemName;

	public string ExamineText;

	// Inventory Data
	public int StackLimit;

	public EquipSlot equipSlot;

	public Sprite Sprite;

	public List<Options> InventoryOptions;

	public List<Options> EquipedOptions;

	// World Item Data
	public Mesh Mesh;

	public Material Material;

	public List<Options> WorldOptions;

	// Wieldable Data

	public Vector3 ProjectileSource;
	public AbilityItemSO AbilityItemSO;

	[SerializeReference]
	public Ability[] Abilities;


	// Placement Data
	public int PlacedItemID;

	public AbilityItem GetAbilityItem()
	{
		return new AbilityItem(this);
	}


	public Ability[] GetRuntimeAbilities()
	{
		// Get abilities from SO if one exists
		if (AbilityItemSO != null)
		{
			return AbilityItemSO.GetAbilities();
		}

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
		if (Abilities == null)
		{
			return;
		}

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
