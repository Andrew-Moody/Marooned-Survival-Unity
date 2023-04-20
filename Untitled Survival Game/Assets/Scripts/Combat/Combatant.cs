using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : NetworkBehaviour
{
	public Transform FollowTarget;

	[SerializeField]
	private LayerMask _attackMask;
	public LayerMask AttackMask { get { return _attackMask; } }

	[SerializeField]
	private List<AbilitySO> _abilitySOList;

	[SerializeField]
	private WorldStatDisplay _statDisplay;

	private List<Ability> _abilities;

	private AbilityActor _abilityActor;

	private AbilityActor _equipedItem;

	private AbilityActor _currentTarget;

	private Ability _abilityInUse;

	private Stats _stats;


	private AbilityItemSO _abilityItem;

	private Inventory _inventory;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		_abilities = new List<Ability>();

		foreach (AbilitySO abilitySO in _abilitySOList)
		{
			//// This is begging for a factory
			//if (abilitySO.Ability.GetAbilityType() == AbilityType.Melee)
			//{
			//	_abilities.Add(new MeleeAbility(abilitySO.Ability));
			//}
			//else
			//{
			//	_abilities.Add(abilitySO.Ability);
			//}

			_abilities.Add(abilitySO.Ability);
		}

		_abilityActor = GetComponent<AbilityActor>();

		_stats = GetComponent<Stats>();

		_stats.OnStatEmpty += OnStatEmpty;

		_inventory = GetComponent<Inventory>();
	}


	// This is called right after buffered RPC's are called
	// Very usefull for syncing the current state to a late joining client
	// You could use a server rpc to pull the latest state in OnStartClient but that occurs before buffered rpc's
	//public override void OnSpawnServer(NetworkConnection connection)
	//{
	//	base.OnSpawnServer(connection);
	//	//Debug.LogWarning("OnSpawnServer: Combatant " + gameObject);

	//	TargetOnSpawnServer(connection, _health.CurrentValue, _energy.CurrentValue);
	//}

	public override void OnStopNetwork()
	{
		base.OnStopNetwork();

		AnimEventHandler animEventHandler = transform.parent.GetComponentInChildren<AnimEventHandler>();
		if (animEventHandler != null)
		{
			animEventHandler.OnAbilityAnimEvent -= AbilityAnimEventHandler;
			animEventHandler.OnDeathEndAnimEvent -= DeathEndAnimEventHandler;
		}
	}


	public override void OnStartClient()
	{
		base.OnStartClient();
		//Debug.LogWarning("OnStartClient: Combatant " + gameObject);
	}


	// This handles initialization that has to happen after the graphic object is instantiated
	public void Initialize()
	{
		AnimEventHandler animEventHandler = transform.parent.GetComponentInChildren<AnimEventHandler>();
		if (animEventHandler != null)
		{
			animEventHandler.OnAbilityAnimEvent += AbilityAnimEventHandler;
			animEventHandler.OnDeathEndAnimEvent += DeathEndAnimEventHandler;
		}
	}



	public int ChooseAbility(AbilityActor target = null)
	{
		if (_inventory != null)
		{
			ItemSO itemSO = _inventory.GetItemSO(8);
			_abilityItem = itemSO.AbilityItemSO;

			int ability = ChooseItemAbility(target);

			Debug.Log($"Choose ItemAbility {ability}");

			if (ability != -1)
			{
				return ability + _abilities.Count;
			}
		}
		else
		{
			// For now get the first ability that is useable

			for (int i = 0; i < _abilities.Count; i++)
			{
				if (_abilities[i].Useable(_abilityActor, target))
				{
					return i;
				}
			}
		}

		

		return -1;
	}


	private int ChooseItemAbility(AbilityActor target = null)
	{
		Debug.Log($"Choosing ItemAbility {_abilityItem}");

		if (_abilityItem != null)
		{
			for (int i = 0; i < _abilityItem.Abilities.Length; i++)
			{
				Ability ability = _abilityItem.Abilities[i];

				Debug.Log($"Checking {ability.AbilityName} of type {ability.GetType()}");


				if (_abilityItem.Abilities[i].Useable(_abilityActor, target))
				{
					return i;
				}
			}
		}

		return -1;
	}

	public Ability GetAbility(int abilityIndex)
	{
		if (abilityIndex == -1)
		{
			//Debug.Log("Failed to get ability with index: -1");
			return null;
		}

		if (abilityIndex < _abilities.Count)
		{
			return _abilities[abilityIndex];
		}

		abilityIndex -= _abilities.Count;

		if (_abilityItem != null && abilityIndex < _abilityItem.Abilities.Length)
		{
			return _abilityItem.Abilities[abilityIndex];
		}

		return null;
	}


	#region Ability User
	public void UseAbility(int abilityIndex, AbilityActor target = null)
	{
		Debug.Log("UseAbility");
		_abilityInUse = GetAbility(abilityIndex);
		_currentTarget = target;

		if (_abilityInUse != null)
		{
			if (IsServer)
			{
				UseAbilityAsServer(abilityIndex, target);
			}
			else
			{
				// Request the server to use the ability (note that using a server rpc still incurs a delay when local host
				ServerUseAbility(abilityIndex, target);

				// Client Goes ahead and plays FX immediately (if client is also host no need to do anything else)
				// Hence why the observersRpc Excludes the owner
				_abilityInUse.ApplyEffects(_abilityActor, _equipedItem, target, EffectTiming.OnStart, false);

				// Start Cooldown on client if not host
				// (isnt authoritative / can be hacked client side but nothing will happen server side
				// this just helps the cool down be consistent for the user)

				// What to do if client sends rpc to use ability twice and the second one takes less time to reach the server
				// than the first? will the delay in starting mean the cooldown has not finished on the server resulting in the input being ignored?
				// need some form of compensation / forgiveness
				_abilityInUse.StartCoolDown();

			}
		}
	}


	private void AbilityAnimEventHandler()
	{
		Debug.Log("AbilityAnimEvent asServer: " + IsServer + " On: " + transform.parent.gameObject.name);

		if (_abilityInUse == null)
		{
			return;
		}

		if (_currentTarget != null)
		{
			_abilityInUse.ApplyEffects(_abilityActor, _equipedItem, _currentTarget, EffectTiming.OnAnimEvent, IsServer);
		}
		else
		{
			AbilityActor[] targets = _abilityInUse.FindTargets(_abilityActor.transform.position, _attackMask);

			if (targets.Length > 1)
			{
				Debug.Log("Multiple Targets found");
			}

			foreach (AbilityActor target in targets)
			{
				if (target.IsAlive)
				{
					_abilityInUse.ApplyEffects(_abilityActor, _equipedItem, target, EffectTiming.OnAnimEvent, IsServer);
				}
			}
		}
	}


	private void DeathEndAnimEventHandler()
	{
		OnDeathEnd();
	}


	[Server]
	private void UseAbilityAsServer(int abilityIndex, AbilityActor target)
	{
		Debug.Log("ServerUseAbility");

		//_abilityInUse.Cancel() Cancel the previous ability?

		_abilityInUse = GetAbility(abilityIndex);

		if (_abilityInUse.Useable(_abilityActor, target))
		{
			_currentTarget = target;

			_abilityInUse.ApplyEffects(_abilityActor, _equipedItem, target, EffectTiming.OnStart, true);

			_abilityInUse.StartCoolDown();

			ObserversUseAbility(abilityIndex, target);
		}
	}


	[ServerRpc]
	private void ServerUseAbility(int abilityIndex, AbilityActor target)
	{
		Debug.Log("ServerRpcUseAbility");

		UseAbilityAsServer(abilityIndex, target);
	}


	[ObserversRpc(ExcludeOwner = true)]
	private void ObserversUseAbility(int abilityIndex, AbilityActor target)
	{
		// Local host only needs effects to be applied once
		if (!IsServer)
		{
			Debug.Log("ObserversUseAbility");

			_abilityInUse = GetAbility(abilityIndex);

			_currentTarget = target;

			_abilityInUse.ApplyEffects(_abilityActor, _equipedItem, target, EffectTiming.OnStart, false);

			// Dont need observers (non owning) to track cooldown
			//_abilityInUse.StartCoolDown(); // Does the cool down need to tick on observers?
		}
	}

	#endregion


	// Handle death of combatant
	#region OnDeath


	private void OnStatEmpty(StatType statType)
	{
		if (statType == StatType.Health)
		{
			OnDeathStart();
		}
	}


	private void OnDeathStart()
	{
		Debug.Log("OnDeathStart");

		_abilityActor.IsAlive = false;

		_abilityActor.SetAnimTrigger("DEATH");

		_statDisplay.Show(false);

		if (IsServer)
		{
			Agent agent = GetComponent<Agent>();

			if (agent != null)
			{
				agent.StopAgent();
			}

			Vector3 spawnPos = transform.position;
			spawnPos.y += 1.4f;
			ItemManager.Instance.SpawnWorldItem(99, spawnPos);
		}
	}


	private void OnDeathEnd()
	{
		if (IsServer)
		{
			Despawn(transform.parent.gameObject);
		}
	}


	[ObserversRpc]
	private void ObserversOnDeath()
	{
		Debug.Log($"Oh dear, {gameObject.name} {OwnerId} has died!");
	}


	[TargetRpc]
	private void TargetOnDeath(NetworkConnection connection)
	{
		Debug.Log("Oh dear, you are dead!");
	}

	#endregion

	
	private void Update()
	{
		if (_abilities != null)
		{
			foreach (Ability ability in _abilities)
			{
				ability.TickAbility(Time.deltaTime);
			}

			if (_abilityItem != null)
			{
				foreach (Ability ability in _abilityItem.Abilities)
				{
					ability.TickAbility(Time.deltaTime);
				}
			}
		}
	}
}