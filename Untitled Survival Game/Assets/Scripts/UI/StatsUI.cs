using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class StatsUI : UIPanel
{
	[SerializeField]
	private List<StatBar> _statBars;

	private IUIEventPublisher _target;

	private readonly Dictionary<string, StatBar> _statBarDict = new Dictionary<string, StatBar>();


	public override void Initialize()
	{
		foreach(StatBar statBar in _statBars)
		{
			_statBarDict.Add(statBar.StatName, statBar);
		}
	}


	public override void SetPlayer(Actor player)
	{
		base.SetPlayer(player);

		if (_player != null)
		{
			_target = player.Stats;

			if (_target != null)
			{
				_target.UIEvent += StatChangeHandler;
			}
			else
			{
				Debug.LogWarning("StatsUI Failed to locate component that implements IUIEventPublisher");
			}
		}
	}


	private void OnDestroy()
	{
		if (_target != null)
		{
			_target.UIEvent -= StatChangeHandler;
		}
	}


	private void StatChangeHandler(UIEventData data)
	{
		if (_statBarDict.TryGetValue(data.TagString, out StatBar statBar))
		{
			statBar.StatChangeHandler(data);
		}
	}
}
