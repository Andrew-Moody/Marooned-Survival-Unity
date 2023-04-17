using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AbilityItemSO")]
public class AbilityItemSO : ScriptableObject
{
	public string ItemName;

	public int itemID;

	public Ability[] Abilities;
}
