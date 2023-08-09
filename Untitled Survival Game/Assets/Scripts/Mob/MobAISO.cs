using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MobAISO")]
public class MobAISO : ScriptableObject
{
	public AIState[] States;

	private Dictionary<string, int> _stateTable;

	private StateMachine _prototypeFSM;

    public StateMachine GetRuntimeFSM()
	{
		if (_prototypeFSM == null)
		{
			BuildRuntimeFSM();
		}

		return _prototypeFSM.DeepCopy();
	}


	private void BuildRuntimeFSM()
	{
		_stateTable = new Dictionary<string, int>();

		for (int i = 0; i < States.Length; i++)
		{
			if (States[i].StateType != StateType.None)
			{
				_stateTable[States[i].StateName] = i;
			}
		}


		List<IState> states = new List<IState>();

		foreach (AIState state in States)
		{
			if (state.StateType == StateType.None)
			{
				continue;
			}

			foreach (AITransition transition in state.Transitions)
			{
				if (!_stateTable.TryGetValue(transition.NextState, out int nextState))
				{
					Debug.LogWarning($"NextState {transition.NextState} did not correspond to a state name");
					continue;
				}

				transition.Condition.NextState = nextState;

				state.State.Transitions.Add(transition.Condition);
			}

			state.State.StateName = state.StateName;

			states.Add(state.State);
		}

		_prototypeFSM = new StateMachine(states.ToArray());
	}


	private void OnValidate()
	{
		for (int i = 0; i < States.Length; i++)
		{
			if (!States[i].CheckTypeMatch())
			{
				States[i].State = FSMFactorySO.CreateState(States[i].StateType);
			}

			AITransition[] transitions = States[i].Transitions;

			for (int j = 0; j < transitions.Length; j++)
			{
				if (!transitions[j].CheckTypeMatch())
				{
					transitions[j].Condition = FSMFactorySO.CreateCondition(transitions[j].ConditionType);
				}
			}
		}
	}
}


[System.Serializable]
public struct AIState
{
	public string StateName;
	public StateType StateType;
	[SerializeReference]
	public BaseState State;

	public AITransition[] Transitions;

	public bool CheckTypeMatch()
	{
		if (State == null || StateType == StateType.None)
		{
			return State == null && StateType == StateType.None;
		}

		return (StateType.ToString() + "State") == State.GetType().ToString();
	}
}


[System.Serializable]
public struct AITransition
{
	public ConditionType ConditionType;
	[SerializeReference]
	public BaseCondition Condition;

	public string NextState;

	public bool CheckTypeMatch()
	{
		if (Condition == null || ConditionType == ConditionType.None)
		{
			return Condition == null && ConditionType == ConditionType.None;
		}

		return (ConditionType.ToString() + "Condition" == Condition.GetType().ToString());
	}
}
