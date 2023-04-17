using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance;

	[SerializeField]
	private InventoryUI _inventoryUI;

	[SerializeField]
	private StatsUI _statsUI;

	[SerializeField]
	private TextMeshProUGUI _fpsText;

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
			_inventoryUI.Initialize(player);


			_statsUI.Inititialize(player);
		}
	}
}
