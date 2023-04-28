using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : NetworkBehaviour
{
	
	public Pathfinding Pathfinding { get { return _pathfinding; } private set { _pathfinding = value; } }
	[SerializeField] private Pathfinding _pathfinding;

	public Animator Animator { get { return _animator; } set { _animator = value; } }
	[SerializeField] private Animator _animator;

	public Combatant Combatant { get { return _combatant; } private set { _combatant = value; } }
	[SerializeField] private Combatant _combatant;

	public LayerMask ViewMask { get { return _viewMask; } private set { _viewMask = value; } }
	[SerializeField] private LayerMask _viewMask;

	public float ViewRange {  get { return _viewRange; } private set { _viewRange = value; } }
	[SerializeField] private float _viewRange;

	public Vector3 RoamRange { get { return _roamRange; } private set { _roamRange = value; } }
	[SerializeField] private Vector3 _roamRange;

	public Vector3 RoamCenter { get { return _roamCenter; } private set { _roamCenter = value; } }
	[SerializeField] private Vector3 _roamCenter;


	[SerializeField]
	private WorldStatDisplay _worldStatDisplay;

	public AbilityActor AttackTarget;


	public float TimeToWait;


	private StateMachine _stateMachine;

	private bool _running = false;

	private float _textUpdateTime;
	private int _updateTicks;
	private int _flips;


	public override void OnStartServer()
	{
		base.OnStartServer();

		RoamState roamState = new RoamState();

		AttackState attackState = new AttackState();

		TargetInViewCond targetInViewCond = new TargetInViewCond();

		roamState.Transitions.Add(new Transition(targetInViewCond, attackState));

		_stateMachine = new StateMachine();

		_stateMachine.ChangeState(roamState, this);

		TimeToWait = 0f;

		_running = true;

		TimeManager.OnPostTick += TimeManager_OnPostTick;

		_pathfinding.SetIsServer(true);
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

			// Ahh well the collider kinda still needs to be enabled to do attack raycasts
			// I do have collision turned off between player and mobs though cause it just screws the prediction system
			// to have collision between predicted and non predicted (non static) objects
			//GetComponent<Collider>().enabled = false;

			// This is so the pathfinder gets ticked
			_running = true;

			TimeManager.OnPostTick += TimeManager_OnPostTick;

			_pathfinding.SetIsServer(false);
		}
	}


	public override void OnStopNetwork()
	{
		base.OnStopNetwork();

		TimeManager.OnPostTick -= TimeManager_OnPostTick;
	}


	public void StopAgent()
	{
		_pathfinding.EnableNMAgent(false);

		_running = false;
	}


	public void KnockBack(Vector3 direction, float strength)
	{
		_pathfinding.KnockBack(direction, strength, IsServer);
	}


	void Update()
	{
		if (_running)
		{
			_stateMachine.OnTick(this);

			if (TimeToWait > 0)
			{
				TimeToWait -= Time.deltaTime;
			}

			Vector3 velocity = _pathfinding.GetNormalisedVelocity();

			if (_animator != null)
			{
				_animator.SetFloat("ForwardSpeed", velocity.z);
				//_animator.SetFloat("ForwardSpeed", 1f);
				//_animator.SetFloat("RightSpeed", velocity.x);
			}
			else
			{
				Debug.Log("Animator is still null");
			}



			_textUpdateTime -= Time.deltaTime;
			_updateTicks++;

			if (_pathfinding.Flipped)
			{
				_flips++;
			}

			if (_textUpdateTime <= 0)
			{
				_textUpdateTime = 1f;

				//_worldStatDisplay.SetInfoText(_flips.ToString() + ", " +_updateTicks.ToString());



				_updateTicks = 0;
				_flips = 0;
			}

			_worldStatDisplay.SetInfoText(_pathfinding.GetDistanceToTarget().ToString());

		}


		

		
	}


	public void OnAnimatorStateExit(AnimatorStateInfo stateInfo)
	{

	}


	public void ChangeState(IState state)
	{
		_stateMachine.ChangeState(state, this);
	}


	private void TimeManager_OnPostTick()
	{
		if (_running)
		{
			_pathfinding.Tick((float)TimeManager.TickDelta);
		}
	}
}
