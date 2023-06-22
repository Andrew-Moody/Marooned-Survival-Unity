using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingUI : UIPanel
{
	[SerializeField]
	private CraftingSlotUI _slotPrefab;

	[SerializeField]
	private Transform _slotHolder;


	private List<CraftingSlotUI> _slots = new List<CraftingSlotUI>();

	private CraftingRecipe[] _craftingRecipes;

	private Inventory _inventory;


	public override void SetPlayer(GameObject player)
	{
		base.SetPlayer(player);

		if (_player != null)
		{
			_inventory = _player.GetComponent<Inventory>();

			CraftingManager.Instance.OnCraftItem += PopulateRecipes;
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

		MouseUI.HideTooltip();

		CraftingUIPanelData data = craftingRecipes as CraftingUIPanelData;

		if (data != null)
		{
			_craftingRecipes = data.Recipes;
		}

		PopulateRecipes();
	}


	public override void Hide()
	{
		base.Hide();

		MouseUI.HideTooltip();
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
				CraftingSlotUI slot = Instantiate(_slotPrefab, _slotHolder);

				slot.OnPointerClickEvent += OnPointerClickHandler;
				slot.OnPointerEnterEvent += OnPointerEnterHandler;
				slot.OnPointerExitEvent += OnPointerExitHandler;

				_slots.Add(slot);
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


	// OnPointerExit failed to fire when handled directly by CraftingUI and the mouse moved from one child to another
	// Seems to work better with the slots recieving pointer events and raising events to be handled here
	private void OnPointerClickHandler(PointerEventData eventData)
	{
		if (eventData.pointerClick.TryGetComponent(out CraftingSlotUI slot))
		{
			int recipeID = _craftingRecipes[slot.index].RecipeID;

			CraftingManager.Instance.CraftItemSRPC(recipeID, _inventory);
		}
	}

	
	private void OnPointerEnterHandler(PointerEventData eventData)
	{
		if (eventData.pointerEnter.TryGetComponent(out CraftingSlotUI slot))
		{
			Ingredient[] ingredients = _craftingRecipes[slot.index].Ingredients;

			string[] entries = new string[ingredients.Length];

			for (int i = 0; i < ingredients.Length; i++)
			{
				entries[i] = $"{ItemManager.Instance.GetItemSO(ingredients[i].ItemID).ItemName} x{ingredients[i].Quantity}";
			}

			MouseUI.ShowTooltip(slot.CurrentIcon, entries);
		}
	}

	private void OnPointerExitHandler(PointerEventData eventData)
	{
		MouseUI.HideTooltip();
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
