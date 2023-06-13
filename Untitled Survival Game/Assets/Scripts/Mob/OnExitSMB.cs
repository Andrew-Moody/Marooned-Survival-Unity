using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnExitSMB : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Agent agent = animator.gameObject.GetComponent<Agent>();
		if (agent != null)
		{
			agent.OnAnimatorStateExit(stateInfo);
		}

	}
}
