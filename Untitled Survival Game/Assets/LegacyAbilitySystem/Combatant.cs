using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace LegacyAbility
{
	public class Combatant : NetworkBehaviour, IActor
	{
		public event ActorEventHandler DeathStarted;

		public event ActorEventHandler DeathFinished;

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

		[SerializeField]
		private AbilityActor _itemActor;

		private AbilityActor _currentTarget;

		private Ability _abilityInUse;

		private IUIEventPublisher _stats;

		private EquipmentController _equipment;

		private bool _IsAbilityActive;


		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			_abilityActor = GetComponent<AbilityActor>();

			_stats = GetComponent<IUIEventPublisher>();

			if (_stats != null)
			{
				_stats.UIEvent += Stats_StatChanged;
			}

			_equipment = GetComponent<EquipmentController>();
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
				animEventHandler.OnAbilityEndAnimEvent -= AbilityEndAnimEventHandler;
				animEventHandler.OnDeathEndAnimEvent -= DeathEndAnimEventHandler;
			}
		}


		public override void OnStartClient()
		{
			base.OnStartClient();
			//Debug.LogWarning("OnStartClient: Combatant " + gameObject);
		}


		// This handles initialization that has to happen after the graphic object is instantiated
		public void Initialize(List<AbilitySO> abilities = null)
		{
			_abilities = new List<Ability>();

			foreach (AbilitySO abilitySO in _abilitySOList)
			{
				// Each instance of Combatant needs its own instance of each ability for cooldowns, buffs etc.

				_abilities.Add(abilitySO.GetRuntimeAbility());
			}

			if (abilities != null)
			{
				foreach (AbilitySO abilitySO in abilities)
				{
					// Each instance of Combatant needs its own instance of each ability for cooldowns, buffs etc.

					_abilities.Add(abilitySO.GetRuntimeAbility());
				}
			}


			AnimEventHandler animEventHandler = transform.parent.GetComponentInChildren<AnimEventHandler>();
			if (animEventHandler != null)
			{
				animEventHandler.OnAbilityAnimEvent += AbilityAnimEventHandler;
				animEventHandler.OnAbilityEndAnimEvent += AbilityEndAnimEventHandler;
				animEventHandler.OnDeathEndAnimEvent += DeathEndAnimEventHandler;
			}
		}


		/// <summary>
		/// Get the first useable ability. if the combatant has a weapon equipped only check weapon abilities
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public int ChooseAbility(AbilityActor target = null)
		{
			if (_IsAbilityActive)
			{
				//Debug.Log("AbilityIsActive");
				return -1;
			}

			if (_itemActor != null)
			{
				AbilityItem abilityItem = _itemActor.AbilityItem;

				if (abilityItem != null && abilityItem.ItemID != 0)
				{
					int ability = ChooseItemAbility(abilityItem, target);

					if (ability != -1)
					{
						return ability + _abilities.Count;
					}
					else
					{
						return -1;
					}
				}
				else
				{
					Debug.Log("Failed to get ability item from itemActor");
				}
			}


			// For now get the first ability that is useable

			for (int i = 0; i < _abilities.Count; i++)
			{
				if (_abilities[i].Useable(IsServer, _abilityActor, target))
				{
					return i;
				}
			}



			return -1;
		}


		private int ChooseItemAbility(AbilityItem item, AbilityActor target = null)
		{
			if (item == null)
			{
				return -1;
			}

			for (int i = 0; i < item.Abilities.Length; i++)
			{
				if (item.Abilities[i].Useable(IsServer, _abilityActor, target))
				{
					return i;
				}
			}

			return -1;
		}

		public Ability GetAbility(int abilityIndex)
		{
			if (abilityIndex == -1)
			{
				//Debug.LogError("Failed to get ability with index: -1");
				return null;
			}

			if (abilityIndex < _abilities.Count)
			{
				return _abilities[abilityIndex];
			}

			if (_itemActor != null)
			{
				abilityIndex -= _abilities.Count;

				AbilityItem abilityItem = _itemActor.AbilityItem;

				if (abilityItem != null && abilityIndex < abilityItem.Abilities.Length)
				{
					return abilityItem.Abilities[abilityIndex];
				}
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
					_abilityInUse.UseAbility(_abilityActor, _itemActor, target, EffectTiming.OnStart, false);

					// Start Cooldown on client if not host
					// (isnt authoritative / can be hacked client side but nothing will happen server side
					// this just helps the cool down be consistent for the user)

					// What to do if client sends rpc to use ability twice and the second one takes less time to reach the server
					// than the first? will the delay in starting mean the cooldown has not finished on the server resulting in the input being ignored?
					// need some form of compensation / forgiveness
					_abilityInUse.StartCoolDown();

					_IsAbilityActive = true;
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
				_abilityInUse.UseAbility(_abilityActor, _itemActor, _currentTarget, EffectTiming.OnAnimEvent, IsServer);
			}
			else
			{
				AbilityActor[] targets = _abilityInUse.FindTargets(_abilityActor.transform.position, _attackMask);

				if (targets == null)
				{
					// Ability may still perform some effects on the user and itemActor
					_abilityInUse.UseAbility(_abilityActor, _itemActor, null, EffectTiming.OnAnimEvent, IsServer);
					return;
				}

				foreach (AbilityActor target in targets)
				{
					if (target.IsAlive)
					{
						_abilityInUse.UseAbility(_abilityActor, _itemActor, target, EffectTiming.OnAnimEvent, IsServer);
					}
				}
			}
		}


		private void AbilityEndAnimEventHandler()
		{
			Debug.Log("Ability End");
			_IsAbilityActive = false;
		}


		private void DeathEndAnimEventHandler()
		{
			Debug.LogError("DeathEndAnimEventHandler");

			OnDeathEnd();
		}


		[Server]
		private void UseAbilityAsServer(int abilityIndex, AbilityActor target)
		{
			Debug.LogError("UseAbilityAsServer");

			//_abilityInUse.Cancel() Cancel the previous ability?

			_abilityInUse = GetAbility(abilityIndex);

			if (_abilityInUse != null && _abilityInUse.Useable(IsServer, _abilityActor, target))
			{
				_currentTarget = target;

				_abilityInUse.UseAbility(_abilityActor, _itemActor, target, EffectTiming.OnStart, true);

				_abilityInUse.StartCoolDown();

				_IsAbilityActive = true;

				ObserversUseAbility(abilityIndex, target);
			}
			else
			{
				if (_abilityInUse == null)
				{
					Debug.LogError($"UseAbilityAsServer Failed to GetAbility for abilityIndex: {abilityIndex}");
				}
				else
				{
					Debug.LogError($"UseAbilityAsServer Ability not Useable");
				}
			}
		}


		[ServerRpc]
		private void ServerUseAbility(int abilityIndex, AbilityActor target)
		{
			Debug.LogError($"ServerRpcUseAbility {abilityIndex}");

			UseAbilityAsServer(abilityIndex, target);
		}


		[ObserversRpc(ExcludeOwner = true)]
		private void ObserversUseAbility(int abilityIndex, AbilityActor target)
		{
			// Local host only needs effects to be applied once
			if (!IsServer)
			{
				Debug.LogError("ObserversUseAbility");

				_abilityInUse = GetAbility(abilityIndex);

				_currentTarget = target;

				if (_abilityInUse != null)
				{
					_abilityInUse.UseAbility(_abilityActor, _itemActor, target, EffectTiming.OnStart, false);
				}
				else
				{
					Debug.LogError($"ObserversUseAbility Failed to GetAbility for abilityIndex: {abilityIndex}");
				}


				// Dont need observers (non owning) to track cooldown
				//_abilityInUse.StartCoolDown(); // Does the cool down need to tick on observers?
			}
		}

		#endregion


		// Handle death of combatant
		#region OnDeath


		private void Stats_StatChanged(UIEventData data)
		{
			if (data.TagString == "Health" && data is UIFloatChangeEventData statData)
			{
				if (statData.Value == statData.MinValue)
				{
					OnDeathStart();
				}
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
			}

			DeathStarted?.Invoke(this, new ActorEventData());
		}


		private void OnDeathEnd()
		{
			Debug.LogError("OnDeathEnd");

			DeathFinished?.Invoke(this, new ActorEventData());

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
			}
		}
	}
}
