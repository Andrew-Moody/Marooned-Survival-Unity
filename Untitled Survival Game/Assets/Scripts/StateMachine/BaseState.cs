using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseState : IState
{
	public List<BaseCondition> Transitions = new List<BaseCondition>();


	/// <summary>
	/// Check if any transition conditions have been met
	/// </summary>
	/// <param name="agent"></param>
	/// <returns>True if any transition condition was met</returns>
	public bool CheckTransitions(Agent agent)
	{
		foreach (BaseCondition transition in Transitions)
		{
			//Debug.Log("Checking Transition to" + transition.State.GetType());

			if (transition.Evaluate(agent))
			{
				agent.ChangeState(transition.NextState);
				return true;
			}
		}

		return false;
	}

	

	public abstract void OnEnter(Agent agent);

	public abstract void OnExit(Agent agent);

	public abstract void OnTick(Agent agent);


	public BaseState()
	{
		Debug.LogError("Base State Constructor");
	}

	public BaseState(BaseState state)
	{
		// Need to copy the conditions as well but this is a test
		Transitions = state.Transitions;

		Transitions = new List<BaseCondition>();

		for (int i = 0; i < state.Transitions.Count; i++)
		{
			Transitions.Add(state.Transitions[i].DeepCopy());
		}


		Debug.LogError("Base State Copy Constructor");
	}


	public abstract BaseState DeepCopy();
}



public interface IState
{
	public bool CheckTransitions(Agent agent);

	public void OnEnter(Agent agent);

	public void OnExit(Agent agent);

	public void OnTick(Agent agent);

	public BaseState DeepCopy();
}


//public class AttackState : BaseState { }

//public class LauraState : BaseState { }

//public class MoveState : BaseState { }

//public class FightState : BaseState { }