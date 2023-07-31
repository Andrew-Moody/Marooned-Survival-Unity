using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Actors;

public class MeleeAttackState : BaseState
{
	[SerializeField]
	private float _attackCoolDown;

	[SerializeField]
	private LayerMask _viewMask;

	[SerializeField]
	private float _viewRange;

	private float _coolDownLeft;

	private AbilityActor _abilityActor;

	private AbilityActor _attackTarget;

	public override void OnEnter(Agent agent)
	{
		Debug.Log("Entering MeleeAttackState");
		_coolDownLeft = _attackCoolDown;

		_abilityActor = agent.Actor.AbilityActor;

		agent.SetBlackboardValue("DistToTarget", 0f);
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log("Exiting MeleeAttackState");

		agent.SetAttackTarget(null);

		agent.SetBlackboardValue("DistToTarget", 0f);
	}

	public override void OnTick(Agent agent, float deltaTime)
	{
		if (CheckTransitions(agent))
		{
			return;
		}


		if (agent.AttackTarget == null)
		{
			FindAttackTarget(agent);
			return;
		}

		float distToTarget = (agent.AttackTarget.transform.position - agent.transform.position).magnitude;

		agent.SetBlackboardValue("DistToTarget", distToTarget);

		if (_coolDownLeft <= 0)
		{

			if (!_abilityActor.IsAbilityActive)
			{
				_abilityActor.ActivateAbility(AbilityInput.Primary);

				_coolDownLeft = _attackCoolDown;
			}
		}
		else
		{
			_coolDownLeft -= deltaTime;
		}
	}


	private void FindAttackTarget(Agent agent)
	{
		Collider[] hits = Physics.OverlapSphere(agent.transform.position, _viewRange, _viewMask.value);

		foreach (Collider hit in hits)
		{
			if (hit.TryGetComponent(out ActorFinder finder))
			{
				_attackTarget = finder.Actor.AbilityActor;

				if (_attackTarget != null)
				{
					agent.SetAttackTarget(_attackTarget.gameObject);
				}
			}
		}
	}


	public static BaseState Create()
	{
		return new MeleeAttackState();
	}


	public override BaseState DeepCopy()
	{
		return new MeleeAttackState(this);
	}

	public MeleeAttackState()
	{
		
	}


	public MeleeAttackState(MeleeAttackState state)
		: base(state)
	{
		_attackCoolDown = state._attackCoolDown;

		_viewRange = state._viewRange;

		_viewMask = state._viewMask;
	}
}
