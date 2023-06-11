using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingUI : UIPanel, IPointerDownHandler
{
	[SerializeField]
	private CraftingSlotUI _slotPrefab;


	private List<CraftingSlotUI> _slots = new List<CraftingSlotUI>();

	private CraftingRecipe[] _craftingRecipes;

	private Inventory _inventory;


	// Called by UIManager in OnStartClient
	public override void Initialize()
	{
		CraftingManager.Instance.OnCraftItem += PopulateRecipes;
	}


	public override void SetPlayer(GameObject player)
	{
		base.SetPlayer(player);

		if (_player != null)
		{
			_inventory = _player.GetComponent<Inventory>();
		}
	}


	private void OnDestroy()
	{
		if (CraftingManager.Instance != null)
		{
			CraftingManager.Instance.OnCraftItem -= PopulateRecipes;
		}
	}


	public override void Show(UIPanelData craftingRecipes)
	{
		gameObject.SetActive(true);

		CraftingUIPanelData data = craftingRecipes as CraftingUIPanelData;

		if (data != null)
		{
			_craftingRecipes = data.Recipes;
		}

		PopulateRecipes();
	}


	private void PopulateRecipes()
	{
		if (_craftingRecipes == null)
		{
			return;
		}

		if (_craftingRecipes.Length > _slots.Count)
		{
			for (int i = _slots.Count; i < _craftingRecipes.Length; i++)
			{
				_slots.Add(Instantiate(_slotPrefab, transform));
				_slots[i].index = i;
			}
		}

		for (int i = 0; i < _craftingRecipes.Length; i++)
		{
			_slots[i].SetRecipe(_craftingRecipes[i]);
			_slots[i].SetGreyout(!CraftingManager.Instance.CheckRecipe(_craftingRecipes[i], _inventory));
			_slots[i].gameObject.SetActive(true);
		}

		for (int i = _craftingRecipes.Length; i < _slots.Count; i++)
		{
			_slots[i].gameObject.SetActive(false);
		}
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		CraftingSlotUI slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<CraftingSlotUI>();

		if (slot != null)
		{
			int recipeID = _craftingRecipes[slot.index].RecipeID;

			CraftingManager.Instance.CraftItemSRPC(recipeID, _inventory);
		}
	}
}


public class CraftingUIPanelData : UIPanelData
{
	private CraftingRecipe[] _recipes;
	public CraftingRecipe[] Recipes => _recipes;

	public int test;

	public CraftingUIPanelData(CraftingRecipe[] recipes)
	{
		_recipes = recipes;
	}
}
