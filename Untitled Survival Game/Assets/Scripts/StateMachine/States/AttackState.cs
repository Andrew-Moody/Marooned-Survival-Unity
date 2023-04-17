using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{

	public override void OnEnter(Agent agent)
	{
		Debug.Log("Entering AttackState");
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log("Exiting AttackState");
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

		int abilityIndex = agent.Combatant.ChooseAbility(agent.AttackTarget);

		if (abilityIndex != -1)
		{
			Debug.Log("AttackState Used Ability: " + abilityIndex);

			agent.Combatant.UseAbility(abilityIndex);
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
}
