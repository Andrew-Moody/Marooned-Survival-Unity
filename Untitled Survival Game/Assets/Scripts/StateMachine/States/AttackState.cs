using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityActor = LegacyAbility.AbilityActor;

public class AttackState : BaseState
{
	[SerializeField]
	private float _attackCoolDown;

	private float _coolDownLeft;

	public override void OnEnter(Agent agent)
	{
		Debug.Log("Entering AttackState");
		_coolDownLeft = _attackCoolDown;
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log("Exiting AttackState");

		agent.AttackTarget = null;

		agent.SetBlackboardValue("DistToTarget", 0f);
	}

	public override void OnTick(Agent agent)
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
			int abilityIndex = agent.Combatant.ChooseAbility(agent.AttackTarget);

			if (abilityIndex != -1)
			{
				Debug.Log("AttackState Used Ability: " + abilityIndex);

				_coolDownLeft = _attackCoolDown;

				agent.Combatant.UseAbility(abilityIndex);
			}
		}
		else
		{
			_coolDownLeft -= Time.deltaTime;
		}
	}


	private void FindAttackTarget(Agent agent)
	{
		Collider[] hits = Physics.OverlapSphere(agent.transform.position, agent.ViewRange, agent.Combatant.AttackMask);

		foreach (Collider hit in hits)
		{
			AbilityActor target = hit.GetComponent<AbilityActor>();

			if (target != null)
			{
				agent.AttackTarget = target;

				agent.Pathfinding.SetTarget(target.transform);
			}
		}
	}


	public static BaseState Create()
	{
		return new AttackState();
	}


	public override BaseState DeepCopy()
	{
		return new AttackState(this);
	}

	public AttackState()
	{
		//Debug.LogError("Attack State Constructor");
	}


	public AttackState(AttackState state)
		: base(state)
	{
		_attackCoolDown = state._attackCoolDown;

		//Debug.LogError("Attack State Copy Constructor (AttackState)");
	}
}
