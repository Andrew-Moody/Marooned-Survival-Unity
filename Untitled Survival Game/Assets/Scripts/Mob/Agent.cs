using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;

public class Agent : NetworkBehaviour
{
	[SerializeField] private MobAISO _mobAISO;

	public Pathfinding Pathfinding { get { return _pathfinding; } private set { _pathfinding = value; } }
	[SerializeField] private Pathfinding _pathfinding;

	public Animator Animator { get { return _animator; } set { _animator = value; } }
	[SerializeField] private Animator _animator;

	public GameObject ActorObject => _actorObject;
	[SerializeField] private GameObject _actorObject;
	private IActor _actor;

	public LayerMask ViewMask { get { return _viewMask; } private set { _viewMask = value; } }
	[SerializeField] private LayerMask _viewMask;

	public float ViewRange {  get { return _viewRange; } private set { _viewRange = value; } }
	[SerializeField] private float _viewRange;

	public Vector3 RoamRange { get { return _roamRange; } private set { _roamRange = value; } }
	[SerializeField] private Vector3 _roamRange;

	public Vector3 RoamCenter { get { return _roamCenter; } set { _roamCenter = value; } }
	[SerializeField] private Vector3 _roamCenter;


	[SerializeField]
	private WorldStatDisplay _worldStatDisplay;


	[SerializeField]
	private Transform _viewTransform;


	public GameObject AttackTarget { get; set; }


	public float TimeToWait;


	private StateMachine _stateMachine;

	private bool _running = false;

	private float _textUpdateTime;
	private int _updateTicks;
	private int _flips;

	private Dictionary<string, float> _blackboard = new Dictionary<string, float>();


	public override void OnStartServer()
	{
		base.OnStartServer();

		// Have to check timing if NetTransform gets set on Spawn before start server
		_roamCenter = ActorObject.transform.position;

		TimeToWait = 0f;

		_stateMachine = _mobAISO.GetRuntimeFSM();

		_running = true;

		TimeManager.OnPostTick += TimeManager_OnPostTick;

		_actor = _actorObject.GetComponent<IActor>();
	}


	public override void OnStartClient()
	{
		base.OnStartClient();
		if (!IsServer)
		{
			// Most likely this only needs to be updated on the server
			this.enabled = false;

			// Must disable Colliders / NavMeshAgent or anything that would cause movement on the client and rely on server syncing
			// unless agents to be handled by the CSP system

			_pathfinding.EnableNMAgent(false);

			// Ahh well the collider still needs to be enabled to do attack raycasts
			// I do have collision turned off between player and mobs because the prediction system has
			// trouble handling collisions between predicted and non predicted (non static) objects
			//GetComponent<Collider>().enabled = false;

			// Why tick on client?

			// This is so the pathfinder gets ticked
			//_running = true;

			// TimeManager.OnPostTick += TimeManager_OnPostTick;
		}
	}


	public override void OnStopNetwork()
	{
		base.OnStopNetwork();

		TimeManager.OnPostTick -= TimeManager_OnPostTick;
	}


	[Server]
	public void StopAgent()
	{
		_pathfinding.EnableNMAgent(false);

		_running = false;
	}


	[Server]
	public void KnockBack(Vector3 direction, float strength)
	{
		_pathfinding.KnockBack(direction, strength);
	}


	public void SetBlackboardValue(string name, float value)
	{
		_blackboard[name] = value;
	}


	public float GetBlackboardValue(string name)
	{
		if (!_blackboard.TryGetValue(name, out float value))
		{
			Debug.LogError($"Blackboard does not contain entry with name: " + name);
		}

		return value;
	}


	public void OnAnimatorStateExit(AnimatorStateInfo stateInfo)
	{

	}


	public void ChangeState(int state)
	{
		_stateMachine.ChangeState(state, this);
	}


	public void SetStateMachine(StateMachine stateMachine)
	{
		_stateMachine = stateMachine;
	}


	private void TimeManager_OnPostTick()
	{
		if (_running)
		{
			if (_stateMachine != null)
			{
				_stateMachine.OnTick(this);
			}

			_pathfinding.Tick((float)TimeManager.TickDelta);

			if (TimeToWait > 0)
			{
				TimeToWait -= Time.deltaTime;
			}

			Vector3 velocity = _pathfinding.GetNormalisedVelocity();

			_animator.SetFloat("ForwardSpeed", velocity.z);
			//_animator.SetFloat("ForwardSpeed", 1f);
			//_animator.SetFloat("RightSpeed", velocity.x);


			if (_viewTransform != null && AttackTarget != null)
			{
				Vector3 targetPos = AttackTarget.transform.position;
				targetPos.y += 1.4f;

				_viewTransform.LookAt(targetPos, Vector3.up);
			}
		}
	}


	protected override void OnValidate()
	{
		if (_actorObject != null && _actorObject.GetComponent<IActor>() == null)
		{
			_actorObject = null;

			Debug.LogWarning("ActorObject must implement IActor");
		}
	}
}
