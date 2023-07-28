using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInViewCondition : BaseCondition
{
	[SerializeField]
	private LayerMask _viewMask;

	[SerializeField]
	private float _viewRange;


	public override bool Evaluate(Agent agent)
	{
		return Physics.CheckSphere(agent.transform.position, _viewRange, _viewMask.value);
	}


	public static BaseCondition Create()
	{
		return new TargetInViewCondition();
	}

	public override BaseCondition DeepCopy()
	{
		return new TargetInViewCondition(this);
	}


	public TargetInViewCondition() { }


	public TargetInViewCondition(TargetInViewCondition targetInViewCondition)
	{
		NextState = targetInViewCondition.NextState;

		_viewMask = targetInViewCondition._viewMask;

		_viewRange = targetInViewCondition._viewRange;
	}
}
