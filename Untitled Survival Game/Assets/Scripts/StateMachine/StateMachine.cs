using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState _currentState;


    public void ChangeState(IState state, Agent agent)
	{
		if (_currentState != null)
		{
			_currentState.OnExit(agent);
		}
		
		_currentState = state;

		_currentState.OnEnter(agent);
	}


	public void OnTick(Agent agent)
	{
		_currentState.OnTick(agent);
	}
}
