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

    public int equipSlot;

    public Sprite Sprite;

    public List<Options> InventoryOptions;

    public List<Options> EquipedOptions;

    // World Item Data
    public Mesh Mesh;

    public Material Material;

    public List<Options> WorldOptions;

    // Wieldable Data
    public AbilityItemSO AbilityItemSO;

}
