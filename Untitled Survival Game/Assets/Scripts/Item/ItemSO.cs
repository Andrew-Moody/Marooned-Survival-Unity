using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityInputBinding = AbilitySystem.AbilityInputBinding;


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

	public List<AbilityInputBinding> Abilities => new List<AbilityInputBinding>(_abilities);
	[SerializeField] private List<AbilityInputBinding> _abilities;

	public float Armor => _armor;
	[SerializeField] private float _armor;
}
