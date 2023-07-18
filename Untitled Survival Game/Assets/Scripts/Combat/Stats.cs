using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StatModifier = AbilitySystem.StatModifier;

public class Stats : NetworkBehaviour
{
	[SerializeField]
	private List<StatValue> _statInitialValues;

	/// <summary>
	/// Notify Listeners that a stat has changed. StatData contains the type, value, and max value. Bool is true if the change should be immediate
	/// </summary>
	public event Action<StatData, bool> OnStatChange;


	/// <summary>
	/// Notify Listeners when a stat reaches zero, typically health
	/// </summary>
	public event Action<StatType> OnStatEmpty;


	// The nice thing about syncvars is that they can collect changes and send periodically instead of every tick.
	// The downside is that this causes a delay that has been inconsistent in testing, and has a single global value
	// that can't be tailored to the desired update rate of a particular variable (and can't be queried at runtime)
	// Rpcs would be a better choice for things that dont change on tick and need to react fast to changes when they happen

	[SyncObject]
	private readonly SyncDictionary<StatType, float> _statValues = new SyncDictionary<StatType, float>();

	[SyncObject]
	private readonly SyncDictionary<StatType, float> _statMaxValues = new SyncDictionary<StatType, float>();

	#region NetworkCallbacks

	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_statValues.OnChange += Stats_OnChange;
	}


	public override void OnStartServer()
	{
		base.OnStartServer();

		SetupStats();
	}


	public override void OnStopNetwork()
	{
		base.OnStopNetwork();

		_statValues.OnChange -= Stats_OnChange;
	}


	public override void OnSpawnServer(NetworkConnection connection)
	{
		base.OnSpawnServer(connection);
		// OnStartClient is too early, StatsUI may not be initialized yet / may not be any listeners
		// Possibly not ideal but it does seem to occur after OnStartClient even with little latency
		InitializeUIListeners(connection);
	}

	#endregion

	public bool HasStat(StatType statType)
	{
		return _statValues.ContainsKey(statType);
	}


	public float GetStatValue(StatType statType)
	{
		_statValues.TryGetValue(statType, out float value);

		return value;
	}


	public float GetStatMax(StatType statType)
	{
		_statMaxValues.TryGetValue(statType, out float value);

		return value;
	}


	[Server]
	public void SetStat(StatType statType, float amount)
	{
		if (!_statValues.ContainsKey(statType))
		{
			Debug.LogWarning($"Failed to SetStat: {gameObject.GetInstanceID()} does not have stat of type: {statType}");
			return;
		}

		// Ensure the new value is between bounds
		amount = Mathf.Clamp(amount, 0f, _statMaxValues[statType]);

		// This should syncronize
		_statValues[statType] = amount;
	}


	// Used only by legacy ability system
	[Server]
	public void AddToStat(StatType statType, float amount)
	{
		if (_statValues.ContainsKey(statType))
		{
			float current = _statValues[statType];

			current += amount;

			current = Mathf.Clamp(current, 0f, _statMaxValues[statType]);

			// This should syncronize
			_statValues[statType] = current;
		}
		else
		{
			Debug.LogWarning($"Failed to SetStat: {gameObject.GetInstanceID()} does not have stat of type: {statType}");
		}
	}


	private void SetupStats()
	{
		//Debug.Log("SetupStats");
		for (int i = 0; i < _statInitialValues.Count; i++)
		{
			StatValue statValue = _statInitialValues[i];

			if (!_statValues.ContainsKey(statValue.StatType))
			{
				_statValues.Add(statValue.StatType, _statInitialValues[i].Value);
			}

			if (!_statMaxValues.ContainsKey(statValue.StatType))
			{
				_statMaxValues.Add(statValue.StatType, statValue.Value);
			}
		}
	}

	
	private void Stats_OnChange(SyncDictionaryOperation operation, StatType statType, float statValue, bool asServer)
	{
		if (!IsSpawned)
		{
			Debug.Log("Object is Despawned asServer: " + asServer + " op: " + operation);

			// Have to investigate more but it seems even though the syncvar is being changed first
			// This still gets called on the client after it has been marked despawned
			// when client host

			// Unsubscribing in OnStopNetwork does prevent this from being called when the object is despawned
			// Unfortunately host clients will not recieve the last OnStatChange event before the object is marked despawned

			// Welp seems syncvars are sent on an interval not immediatly
			// Best option seems to wait for at least one sync interval before actually calling despawn
			return;
		}


		if (operation == SyncDictionaryOperation.Set)
		{
			//Debug.LogError("Stats SyncDictionary Changed asServer: " + asServer + " For type: " + statType + " tick: " +TimeManager.Tick);

			// This callback will get called twice on client host, but we only want to Invoke OnStatChange once
			// (the client host will still get one Invoke when this is called with asServer = true)
			// asServer = true  -> Invoke
			// asClient and not IsServer -> Invoke
			// asClient and IsServer -> Don't Invoke
			if (IsServer && !asServer)
			{
				return;
			}

			// Changed to invoke on server and client (but only once if local host)
			if (statValue == 0f)
			{
				OnStatEmpty?.Invoke(statType);
			}

			if (!_statMaxValues.TryGetValue(statType, out float maxValue))
			{
				Debug.Log("Failed to get MaxValue: " + asServer);
			}

			StatData statData = new StatData(statType, statValue, maxValue);

			OnStatChange?.Invoke(statData, false);
			
		}
	}


	[TargetRpc]
	private void InitializeUIListeners(NetworkConnection networkConnection)
	{
		// Needed to update late joiners of the current values
		foreach (var item in _statValues)
		{
			StatData statData = new StatData(item.Key, item.Value, _statMaxValues[item.Key]);

			OnStatChange?.Invoke(statData, true);
		}
	} 
}


public enum StatType
{
	None,
	Health,
	MagicEnergy,
	DivineEnergy,
	Stamina,
	Hunger,
	
}


[System.Serializable]
public struct StatValue
{
	public StatType StatType;
	public float Value;
}


public struct StatData
{
	public StatType StatType;

	public float CurrentValue;

	public float MaxValue;

	public StatData(StatType statType, float current, float max)
	{
		StatType = statType;
		CurrentValue = current;
		MaxValue = max;
	}
}