using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingUI : UIPanel, IPointerDownHandler
{
	[SerializeField]
	private CraftingSlotUI _slotPrefab;

	[SerializeField]
	private Transform _slotHolder;

	[SerializeField]
	private ToolTip _toolTip;


	private List<CraftingSlotUI> _slots = new List<CraftingSlotUI>();

	private CraftingRecipe[] _craftingRecipes;

	private Inventory _inventory;


	public override void Initialize()
	{
		
	}


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

		_toolTip.gameObject.SetActive(false);

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
				CraftingSlotUI slot = Instantiate(_slotPrefab, _slotHolder);

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


	public void OnPointerDown(PointerEventData eventData)
	{
		CraftingSlotUI slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<CraftingSlotUI>();

		if (slot != null)
		{
			int recipeID = _craftingRecipes[slot.index].RecipeID;

			CraftingManager.Instance.CraftItemSRPC(recipeID, _inventory);
		}
	}


	// OnPointerExit failed to fire when handled directly by CraftingUI when the mouse moved from one child to another
	// Moving the eventSystem callbacks to the slots themselves seems to work

	private void OnPointerEnterHandler(PointerEventData eventData)
	{
		//Debug.LogError($"OnPointerEnter on {eventData.pointerEnter.name}");

		if (eventData.pointerEnter.TryGetComponent(out CraftingSlotUI slot))
		{
			Ingredient[] ingredients = _craftingRecipes[slot.index].Ingredients;

			string[] entries = new string[ingredients.Length];

			for (int i = 0; i < ingredients.Length; i++)
			{
				entries[i] = $"{ItemManager.Instance.GetItemSO(ingredients[i].ItemID).ItemName} x{ingredients[i].Quantity}";
			}

			_toolTip.SetToolTip(slot.CurrentIcon, entries);
			_toolTip.gameObject.SetActive(true);
		}
	}

	private void OnPointerExitHandler(PointerEventData eventData)
	{
		//Debug.LogError($"OnPointerExit on {eventData.pointerEnter.name}");
		_toolTip.gameObject.SetActive(false);
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
