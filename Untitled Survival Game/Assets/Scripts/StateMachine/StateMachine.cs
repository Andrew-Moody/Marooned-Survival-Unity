using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
	private int _currentState = -1;

	private IState[] _states;


	public void ChangeState(int state, Agent agent)
	{
		if (_currentState != -1)
		{
			_states[_currentState].OnExit(agent);
		}
		
		_currentState = state;

		_states[_currentState].OnEnter(agent);
	}


	public void OnTick(Agent agent, float deltaTime)
	{
		if (_currentState == -1 && _states.Length > 0)
		{
			ChangeState(0, agent);
		}

		_states[_currentState].OnTick(agent, deltaTime);
	}


	public StateMachine DeepCopy()
	{
		IState[] states = new IState[_states.Length];

		for (int i = 0; i < _states.Length; i++)
		{
			states[i] = _states[i].DeepCopy();
		}

		return new StateMachine(states);
	}


	//public StateMachine() { }

	public StateMachine(IState[] states)
	{
		_states = states;
	}
}
