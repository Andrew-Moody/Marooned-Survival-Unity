using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;

public class Agent : NetworkBehaviour
{
	[SerializeField] private MobAISO _mobAISO;

	public Pathfinding Pathfinding => _pathfinding;
	[SerializeField] private Pathfinding _pathfinding;

	public Actor Actor => _actor;
	private Actor _actor;

	public GameObject AttackTarget => _attackTarget;
	private GameObject _attackTarget;

	private ViewTransform _viewTransform;

	private StateMachine _stateMachine;

	private bool _running = false;

	private Dictionary<string, float> _blackboard = new Dictionary<string, float>();


	public override void OnStartServer()
	{
		base.OnStartServer();

		_stateMachine = _mobAISO.GetRuntimeFSM();

		_running = true;

		TimeManager.OnPostTick += TimeManager_OnPostTick;

		_actor = Actor.FindActor(gameObject);

		_viewTransform = _actor.ViewTransform;
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


	public void SetAttackTarget(GameObject attackTarget)
	{
		_attackTarget = attackTarget;

		if (_attackTarget != null)
		{
			_pathfinding.SetTarget(attackTarget.transform);
		}
		else
		{
			_pathfinding.SetTarget(null);
		}
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
				_stateMachine.OnTick(this, (float)TimeManager.TickDelta);
			}

			_pathfinding.Tick((float)TimeManager.TickDelta);

			Vector3 velocity = _pathfinding.GetNormalisedVelocity();

			_actor.Animator.SetFloat("ForwardSpeed", velocity.z);
			//_animator.SetFloat("ForwardSpeed", 1f);
			//_animator.SetFloat("RightSpeed", velocity.x);


			if (_viewTransform != null && _attackTarget != null)
			{
				Vector3 targetPos = _attackTarget.transform.position;
				targetPos.y += 1.4f;

				_viewTransform.transform.LookAt(targetPos, Vector3.up);
			}
		}
	}
}
