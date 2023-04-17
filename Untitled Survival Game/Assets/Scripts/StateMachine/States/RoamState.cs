using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamState : BaseState
{
	public override void OnEnter(Agent agent)
	{
		Debug.Log("Entering RoamState");
		agent.Pathfinding.SetDestination(ChooseNextDest(agent));
	}

	public override void OnExit(Agent agent)
	{
		Debug.Log("Exiting RoamState");
	}

	public override void OnTick(Agent agent)
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
		Vector3 roamRange = agent.RoamRange;

		float x = Random.Range(-roamRange.x, roamRange.x);
		float y = Random.Range(-roamRange.y, roamRange.y);
		float z = Random.Range(-roamRange.z, roamRange.z);

		Vector3 destination = new Vector3(x, y, z) + agent.RoamCenter;

		return destination;
	}
}
