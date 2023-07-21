using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LegacyAbility;

public class AttackState : BaseState
{
	[SerializeField]
	private float _attackCoolDown;

	private float _coolDownLeft;

	private Combatant _combatant;

	private AbilityActor _attackTarget;

	public override void OnEnter(Agent agent)
	{
		Debug.Log("Entering AttackState");
		_coolDownLeft = _attackCoolDown;

		if (_combatant == null && agent.ActorObject != null)
		{
			_combatant = agent.ActorObject.GetComponent<Combatant>();
		}
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log("Exiting AttackState");

		agent.AttackTarget = null;

		agent.SetBlackboardValue("DistToTarget", 0f);
	}

	public override void OnTick(Agent agent)
	{
		if (_combatant == null)
		{
			return;
		}


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
			int abilityIndex = _combatant.ChooseAbility(_attackTarget);

			if (abilityIndex != -1)
			{
				Debug.Log("AttackState Used Ability: " + abilityIndex);

				_coolDownLeft = _attackCoolDown;

				_combatant.UseAbility(abilityIndex);
			}
		}
		else
		{
			_coolDownLeft -= Time.deltaTime;
		}
	}


	private void FindAttackTarget(Agent agent)
	{
		Collider[] hits = Physics.OverlapSphere(agent.transform.position, agent.ViewRange, _combatant.AttackMask);

		foreach (Collider hit in hits)
		{
			_attackTarget = hit.GetComponent<AbilityActor>();

			if (_attackTarget != null)
			{
				agent.AttackTarget = _attackTarget.gameObject;

				agent.Pathfinding.SetTarget(_attackTarget.transform);
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
