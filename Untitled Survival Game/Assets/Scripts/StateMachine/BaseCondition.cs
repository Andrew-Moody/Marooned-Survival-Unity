using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseCondition : ICondition
{
	private int _nextState;
	public int NextState { get => _nextState; set => _nextState = value; }

	public abstract bool Evaluate(Agent agent);

	public abstract BaseCondition DeepCopy();
}


public interface ICondition
{
	public bool Evaluate(Agent agent);

	public BaseCondition DeepCopy();
}