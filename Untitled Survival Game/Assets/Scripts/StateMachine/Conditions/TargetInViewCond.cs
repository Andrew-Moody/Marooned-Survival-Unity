using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInViewCond : BaseCondition
{
	public override bool Evaluate(Agent agent)
	{
		return Physics.CheckSphere(agent.transform.position, agent.ViewRange, agent.ViewMask);
	}
}
