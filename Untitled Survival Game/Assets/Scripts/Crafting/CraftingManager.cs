using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : NetworkBehaviour
{
	public static CraftingManager Instance;

	[SerializeField]
	private CraftingStationSO[] _craftingStationSOs;

	private Dictionary<int, CraftingRecipe> _recipes;

	public event Action OnCraftItem;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;

			_recipes = new Dictionary<int, CraftingRecipe>();

			foreach (CraftingStationSO stationSO in _craftingStationSOs)
			{
				foreach (CraftingRecipe recipe in stationSO.Recipes)
				{
					if (!_recipes.ContainsKey(recipe.RecipeID))
					{
						_recipes.Add(recipe.RecipeID, recipe);
					}
				}
			}
		}
	}


	
	public CraftingRecipe GetCraftingRecipe(int recipeID)
	{
		_recipes.TryGetValue(recipeID, out CraftingRecipe recipe);

		return recipe;
	}


	public bool CheckRecipe(CraftingRecipe recipe, Inventory inventory)
	{
		for (int i = 0; i < recipe.Ingredients.Length; i++)
		{
			if (!inventory.HasItem(out int slot, recipe.Ingredients[i].ItemID, recipe.Ingredients[i].Quantity))
			{
				return false;
			}
		}

		return true;
	}


	[ServerRpc(RequireOwnership = false)]
	public void CraftItemSRPC(int recipeID, Inventory inventory)
	{
		CraftingRecipe recipe = GetCraftingRecipe(recipeID);

		if (!CheckRecipe(recipe, inventory))
		{
			return;
		}

		for (int i = 0; i < recipe.Ingredients.Length; i++)
		{
			if (inventory.HasItem(out int slot, recipe.Ingredients[i].ItemID, recipe.Ingredients[i].Quantity))
			{
				if (!inventory.RemoveItemsAtSlot(slot, recipe.Ingredients[i].ItemID, recipe.Ingredients[i].Quantity))
				{
					Debug.LogError("Failed to Craft Item");
				}
			}
		}

		ItemNetData itemNetData = new ItemNetData(recipe.OutputID, recipe.Quantity);

		if (!inventory.TryTakeItem(ref itemNetData))
		{
			Debug.LogError("Failed to Take Item");
		}

		TargetOnCraftItem(inventory.Owner);
	}


	[TargetRpc]
	public void TargetOnCraftItem(NetworkConnection connection)
	{
		OnCraftItem?.Invoke();
	}
}
