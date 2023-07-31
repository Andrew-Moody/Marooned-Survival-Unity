using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamState : BaseState
{
	[SerializeField]
	private Vector3 _roamRange;

	private Vector3 _roamCenter;

	public override void OnEnter(Agent agent)
	{
		Debug.Log("Entering RoamState");

		_roamCenter = agent.Actor.NetTransform.position;

		agent.Pathfinding.SetDestination(ChooseNextDest(agent));
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log("Exiting RoamState");
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
	}
}
