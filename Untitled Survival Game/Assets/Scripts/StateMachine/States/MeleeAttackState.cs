using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Actors;

public class MeleeAttackState : BaseState
{
	[SerializeField]
	private AbilityInput _abilityInput;

	[SerializeField]
	private float _attackCoolDown;

	private float _coolDownLeft;

	private AbilityActor _abilityActor;

	public override void OnEnter(Agent agent)
	{
		Debug.Log($"Entering {StateName}");
		_coolDownLeft = _attackCoolDown;

		_abilityActor = agent.Actor.AbilityActor;

		if (agent.AttackTarget == null)
		{
			Debug.LogError($"Agent {agent.Actor.gameObject.name} does not have an attack target");
		}
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log($"Exiting {StateName}");
	}

	public override void OnTick(Agent agent, float deltaTime)
	{
		if (CheckTransitions(agent))
		{
			return;
		}


		float distToTarget = (agent.AttackTarget.NetTransform.position - agent.transform.position).magnitude;

		agent.SetBlackboardValue("DistToTarget", distToTarget);

		if (_coolDownLeft <= 0)
		{

			if (!_abilityActor.IsAbilityActive)
			{
				_abilityActor.ActivateAbility(_abilityInput);

				_coolDownLeft = _attackCoolDown;
			}
		}
		else
		{
			_coolDownLeft -= deltaTime;
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
		_abilityInput = state._abilityInput;

		_attackCoolDown = state._attackCoolDown;
	}
}
