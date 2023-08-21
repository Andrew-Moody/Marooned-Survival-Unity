using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class StatsUI : UIPanel
{
	[SerializeField]
	private List<StatIndicator> _statIndicators;

	private IUIEventPublisher _target;

	private readonly Dictionary<string, StatIndicator> _statIndicatorDict = new Dictionary<string, StatIndicator>();


	public override void Initialize()
	{
		foreach (StatIndicator statIndicator in _statIndicators)
		{
			_statIndicatorDict.Add(statIndicator.StatName, statIndicator);
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
		Debug.Log($"StatChangeHandler: {data.TagString}");

		if (_statIndicatorDict.TryGetValue(data.TagString, out StatIndicator statIndicator))
		{
			statIndicator.StatChangeHandler(data);
		}
	}
}
