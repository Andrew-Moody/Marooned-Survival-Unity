using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class HeadsupUI : UIPanel
{
	private static HeadsupUI _instance;

	[SerializeField]
	private HotbarUI _hotbarUI;

	[SerializeField]
	private StatsUI _statsUI;

	[SerializeField]
	private ChatUI _chatUI;


	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	public override void Initialize()
	{
		base.Initialize();

		_hotbarUI.Initialize();

		_statsUI.Initialize();

		_chatUI.Initialize();
	}


	public override void SetPlayer(Actor player)
	{
		base.SetPlayer(player);

		_hotbarUI.SetPlayer(player);

		_statsUI.SetPlayer(player);

		_chatUI.SetPlayer(player);
	}


	public override void Show(UIPanelData data)
	{
		base.Show(data);

		_hotbarUI.Show(data);

		_statsUI.Show(data);

		_chatUI.Show(data);
	}


	public override void Hide()
	{
		base.Hide();

		_hotbarUI.Hide();

		_statsUI.Hide();

		_chatUI.Hide();
	}
}
