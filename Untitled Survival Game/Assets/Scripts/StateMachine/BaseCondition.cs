using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCondition : ICondition
{
	public abstract bool Evaluate(Agent agent);
}


public interface ICondition
{
	public bool Evaluate(Agent agent);
}