using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
	public static UIManager Instance;

	[SerializeField]
	private InventoryUI _inventoryUI;

	[SerializeField]
	private StatsUI _statsUI;

	[SerializeField]
	private CraftingUI _craftingUI;

	[SerializeField]
	private HotbarUI _hotbarUI;

	[SerializeField]
	private PlacementUI _placementUI;

	[SerializeField]
	private TextMeshProUGUI _fpsText;

	[SerializeField]
	private Image _cursor;

	[SerializeField]
	private Canvas _worldCanvas;
	public Canvas WorldCanvas { get { return _worldCanvas; } }

	private float _timesSinceFPS;

	private int _frames;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}


	public override void OnStartClient()
	{
		base.OnStartClient();

		Debug.LogError("UIManager OnStartClient");
		InitializeUI();

	}


	private void Update()
	{
		_timesSinceFPS += Time.deltaTime;
		_frames++;
		if (_timesSinceFPS >= 1f)
		{
			float fps = _frames / _timesSinceFPS;
			_fpsText.text = "FPS: " + fps.ToString("#.##");
			_timesSinceFPS = 0f;
			_frames = 0;
		}
	}


	private void InitializeUI()
	{
		GameObject player = PlayerLocator.Player;

		if (player != null)
		{
			Debug.LogError("InitializeUI");

			_inventoryUI.Initialize(player);

			_statsUI.Inititialize(player);

			_craftingUI.Initialize(player);

			_hotbarUI.Initialize(player);

			_placementUI.Initialize(player);
		}
	}


	public void ShowCursor(bool show)
	{
		_cursor.enabled = show;
	}


	public void ShowCraftingUI(CraftingRecipe[] recipes)
	{
		_craftingUI.gameObject.SetActive(true);
		_craftingUI.Show(recipes);
	}


	public void HideCraftingUI()
	{
		_craftingUI.gameObject.SetActive(false);
	}
}
