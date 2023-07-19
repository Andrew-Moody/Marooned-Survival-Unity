using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LegacyAbility;

public class StatsUI : UIPanel
{
	[SerializeField]
	private List<StatBar> _statBars;

	private Stats _stats;

	private readonly Dictionary<StatType, StatBar> _statBarDict = new Dictionary<StatType, StatBar>();


	public override void Initialize()
	{
		foreach(StatBar statBar in _statBars)
		{
			_statBarDict.Add(statBar.StatType, statBar);
		}
	}


	public override void SetPlayer(GameObject player)
	{
		base.SetPlayer(player);

		if (_player != null)
		{
			_stats = player.GetComponent<Stats>();

			if (_stats != null)
			{
				_stats.OnStatChange += StatChangeHandler;
			}
			else
			{
				Debug.LogWarning("StatsUI Failed to locate target with stats component");
			}
		}
	}


	// This is not safe
	private void OnDestroy()
	{
		if (_stats != null)
		{
			_stats.OnStatChange -= StatChangeHandler;
		}
	}


	private void StatChangeHandler(StatData statData, bool immediate)
	{
		if (_statBarDict.ContainsKey(statData.StatType))
		{
			_statBarDict[statData.StatType].StatChangeHandler(statData, immediate);
		}
	}
}
