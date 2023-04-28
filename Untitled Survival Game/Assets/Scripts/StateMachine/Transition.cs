using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Transition
{
	public ICondition Condition;
	public IState State;


	public Transition(ICondition condition, IState state)
	{
		Condition = condition;
		State = state;
	}
}
