using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CraftingStationSO")]
public class CraftingStationSO : ScriptableObject
{
    public CraftingRecipe[] Recipes;
}
