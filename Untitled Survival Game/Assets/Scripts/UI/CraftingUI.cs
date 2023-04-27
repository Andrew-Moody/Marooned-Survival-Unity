using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingUI : MonoBehaviour, IPointerDownHandler
{
	[SerializeField]
	private CraftingSlotUI _slotPrefab;


	private List<CraftingSlotUI> _slots = new List<CraftingSlotUI>();

	private CraftingRecipe[] _craftingRecipes;

	private Inventory _inventory;


	// Called by UIManager in OnStartClient
	public void Initialize(GameObject player)
	{
		_inventory = player.GetComponent<Inventory>();

		CraftingManager.Instance.OnCraftItem += PopulateRecipes;
	}


	private void OnDestroy()
	{
		if (CraftingManager.Instance != null)
		{
			CraftingManager.Instance.OnCraftItem -= PopulateRecipes;
		}
	}


	public void Show(CraftingRecipe[] craftingRecipes)
	{
		_craftingRecipes = craftingRecipes;

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
