using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class RoamState : BaseState
{
	[SerializeField]
	private Vector3 _roamRange;

	private Vector3 _roamCenter;

	[SerializeField]
	private LayerMask _viewMask;

	[SerializeField]
	private float _viewRange;

	public override void OnEnter(Agent agent)
	{
		Debug.Log($"Entering {StateName}");

		_roamCenter = agent.Actor.NetTransform.position;

		agent.SetAttackTarget(null);

		agent.Pathfinding.SetDestination(ChooseNextDest(agent));

		agent.SetBlackboardValue("DistToTarget", float.MaxValue);
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log($"Exiting {StateName}");

		FindAttackTarget(agent);
	}

	public override void OnTick(Agent agent, float deltaTime)
	{
		if (CheckTransitions(agent))
		{
			return;
		}

		//Debug.Log("Still in RoamState. This should not appear after a state change");

		if (agent.Pathfinding.Arrived())
		{
			// Set a new random position
			Vector3 destination = ChooseNextDest(agent);

			agent.Pathfinding.SetDestination(destination);
		}
	}



	private Vector3 ChooseNextDest(Agent agent)
	{
		float x = Random.Range(-_roamRange.x, _roamRange.x);
		float y = Random.Range(-_roamRange.y, _roamRange.y);
		float z = Random.Range(-_roamRange.z, _roamRange.z);

		Vector3 destination = new Vector3(x, y, z) + _roamCenter;

		return destination;
	}


	private void FindAttackTarget(Agent agent)
	{
		Collider[] hits = Physics.OverlapSphere(agent.transform.position, _viewRange, _viewMask.value);

		foreach (Collider hit in hits)
		{
			if (hit.TryGetComponent(out ActorFinder finder))
			{
				agent.SetAttackTarget(finder.Actor);

				float distToTarget = (agent.AttackTarget.NetTransform.position - agent.transform.position).magnitude;

				agent.SetBlackboardValue("DistToTarget", distToTarget);
			}
		}
	}


	public static BaseState Create()
	{
		return new RoamState();
	}


	public override BaseState DeepCopy()
	{
		return new RoamState(this);
	}


	public RoamState() { }


	public RoamState(RoamState state)
		: base(state)
	{
		_roamRange = state._roamRange;

		_viewMask = state._viewMask;

		_viewRange = state._viewRange;
	}
}
