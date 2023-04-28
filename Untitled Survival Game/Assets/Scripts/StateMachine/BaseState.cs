using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseState : IState
{
	public List<Transition> Transitions = new List<Transition>();


	/// <summary>
	/// Check if any transition conditions have been met
	/// </summary>
	/// <param name="agent"></param>
	/// <returns>True if any transition condition was met</returns>
	public bool CheckTransitions(Agent agent)
	{
		foreach (Transition transition in Transitions)
		{
			//Debug.Log("Checking Transition to" + transition.State.GetType());

			if (transition.Condition.Evaluate(agent))
			{
				agent.ChangeState(transition.State);
				return true;
			}
		}

		return false;
	}

	public abstract void OnEnter(Agent agent);

	public abstract void OnExit(Agent agent);

	public abstract void OnTick(Agent agent);
}



public interface IState
{
	public void OnEnter(Agent agent);

	public void OnExit(Agent agent);

	public void OnTick(Agent agent);
}


//public class AttackState : BaseState { }

//public class LauraState : BaseState { }

//public class MoveState : BaseState { }

//public class FightState : BaseState { }