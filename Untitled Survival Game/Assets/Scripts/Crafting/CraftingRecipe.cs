using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CraftingRecipe
{
	public int RecipeID;
	public int OutputID;
	public int Quantity;

	public Ingredient[] Ingredients;
}



[System.Serializable]
public struct Ingredient
{
	public int ItemID;

	[Min(1)]
	public int Quantity;
}