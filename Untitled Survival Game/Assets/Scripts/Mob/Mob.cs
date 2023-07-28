using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;
using FishNet.Component.Transforming;

public class Mob : NetworkBehaviour
{
	public event System.Action<Mob> MobDied;

	public int ID { get => _id; set => _id = value; }
	[SerializeField] private int _id;

	public string MobName => _mobName;
	[SerializeField] private string _mobName;

	public MobSO MobSO => _mobSO;
	[SerializeField] private MobSO _mobSO;

	[SerializeField] private GameObject _actorObject;

	private IActor _actor;

	private Vector3 _spawnPosition;

	private Quaternion _spawnRotation;


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		InitializeFromPrefab();

		Debug.Log("OnStartNetwork Position: " + transform.position);

		SetTransformOnSpawn();
	}


	private void SetTransformOnSpawn()
	{
		// Reset Mob root to origin and displace net transform to spawn position
		_spawnPosition = transform.position;
		_spawnRotation = transform.rotation;

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		if (IsServer && TryGetComponent(out NetworkTransform netTrans))
		{
			netTrans.transform.position = _spawnPosition;
			netTrans.transform.rotation = _spawnRotation;
		}
	}


	private void InitializeFromPrefab()
	{
		_actor = _actorObject.GetComponent<IActor>();

		_actor.DeathFinished += IActor_DeathFinished;
	}

	private void OnDestroy()
	{
		if (_actor != null)
		{
			_actor.DeathFinished -= IActor_DeathFinished;
		}
		
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
		if (_actorObject != null && _actorObject.GetComponent<IActor>() == null)
		{
			_actorObject = null;

			Debug.LogWarning("ActorObject must implement IActor");
		}
	}


	#region SpawnFromSO
	[Header("Legacy")]

	[SerializeField] private Transform _graphicParent;

	[SerializeField] private NetworkAnimator _networkAnimator;

	[SerializeField] private Agent _agent;

	[SerializeField] private Stats _stats;

	[SerializeField] private Transform _networkTransform;

	[SerializeField] private Transform _statDisplay;


	// This was almost all a consequence of changing the structure of the object at runtime.
	// While it sounded like a nice idea to not need a complete prefab for each mob variant,
	// it seems to go against the typical Unity workflow (and againsts Fishnets)
	// If I really need be that data driven it might be better to try to procedurally build prefabs at edit time
	// (could be a fun excerise but wont get this project done faster)
	private void InitializeFromSO(MobSO mobSO)
	{
		if (mobSO == null)
		{
			return;
		}

		_mobSO = mobSO;

		GameObject mobGraphic = mobSO.InstantiatePrefab(_graphicParent);

		Animator animator = mobGraphic.GetComponent<Animator>();

		ProjectileSource projSource = mobGraphic.GetComponentInChildren<ProjectileSource>();

		//_agent.Animator = animator;

		//_agent.RoamCenter = _networkTransform.position;

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
			InitializeFromSO(mobSO);
		}
	}

	#endregion
}
