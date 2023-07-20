using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityActor = AbilitySystem.AbilityActor;
using Combatant = LegacyAbility.Combatant;

using Actor;

public class Mob : NetworkBehaviour
{
	[SerializeField]
	private int _id;
	public int ID { get => _id; set => _id = value; }

	[SerializeField]
	private Transform _networkTransform;
	public Transform NetworkTransform => _networkTransform;

	[SerializeField]
	private Transform _graphicParent;

	[SerializeField]
	private NetworkAnimator _networkAnimator;

	[SerializeField]
	private Agent _agent;

	[SerializeField]
	private AbilityActor _abilityActor;

	[SerializeField]
	private Combatant _combatant;
	public Combatant Combatant => _combatant;

	[SerializeField]
	private Stats _stats;

	[SerializeField]
	private Transform _statDisplay;

	private MobSO _mobSO;


	public void Initialize(MobSO mobSO)
	{
		if (mobSO == null)
		{
			return;
		}

		_mobSO = mobSO;

		GameObject mobGraphic = mobSO.InstantiatePrefab(_graphicParent);

		Animator animator = mobGraphic.GetComponent<Animator>();

		ProjectileSource projSource = mobGraphic.GetComponentInChildren<ProjectileSource>();

		_agent.Animator = animator;

		_agent.RoamCenter = NetworkTransform.position;

		_agent.SetStateMachine(mobSO.MobAISO.GetRuntimeFSM());

		_abilityActor.Initialize(animator);

		_abilityActor.SetProjectileSource(projSource);

		//_combatant.Initialize(mobSO.Abilities);

		//_combatant.OnDeathEndEvent += OnDeathEndEventHandler;

		_networkAnimator.SetAnimator(animator);

		_stats.SetInitialValues(_mobSO.InitialStats);
	}


	[ObserversRpc(BufferLast = true, RunLocally = true)]
	public void ObserversInitializeMob(int mobID)
	{
		MobSO mobSO = MobManager.GetMobSO(mobID);

		if (mobSO != null)
		{
			Initialize(mobSO);
		}
	}

	private void OnDestroy()
	{
		//Debug.LogError("Mob OnDestroy");
		_combatant.OnDeathEndEvent -= OnDeathEndEventHandler;
	}


	private void LateUpdate()
	{
		_statDisplay.position = _graphicParent.position;
	}


	private void OnDeathEndEventHandler()
	{
		Debug.LogError("OnDeathEndEventHandler");
		if (IsServer)
		{
			Vector3 spawnPos = _agent.transform.position;
			spawnPos.y += 0.5f;
			ItemManager.Instance.SpawnWorldItem(_mobSO.ItemToDrop, spawnPos);

			Debug.LogError("Spawning at " + spawnPos);
		}
	}
}
