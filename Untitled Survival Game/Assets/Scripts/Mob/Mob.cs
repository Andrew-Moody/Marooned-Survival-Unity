using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;

public class Mob : NetworkBehaviour
{
	public event System.Action<Mob> MobDied;

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
	private GameObject _actorObject;

	private IActor _actor;

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

		if (_actor is Actor actor)
		{
			actor.Animator = animator;
		}
		
		_actor.DeathFinished += IActor_DeathFinished;

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
		_actor.DeathFinished -= IActor_DeathFinished;
	}


	private void LateUpdate()
	{
		_statDisplay.position = _graphicParent.position;
	}


	private void IActor_DeathFinished(IActor actor, ActorEventData data)
	{
		if (IsServer)
		{
			Vector3 spawnPos = _agent.transform.position + new Vector3(0f, 0.5f, 0f);
			ItemManager.Instance.SpawnWorldItem(_mobSO.ItemToDrop, spawnPos);

			OnMobDied(this);
		}
	}


	private void OnMobDied(Mob mob)
	{
		MobDied?.Invoke(mob);
	}


	protected override void OnValidate()
	{
		if (_actorObject != null)
		{
			_actor = _actorObject.GetComponent<IActor>();

			if (_actor == null)
			{
				_actorObject = null;

				Debug.LogWarning("ActorObject must implement IActor");
			}
		}
	}
}
