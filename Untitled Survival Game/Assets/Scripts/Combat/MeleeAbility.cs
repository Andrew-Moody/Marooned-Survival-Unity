using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MeleeAbility : Ability
{
	private const string RANGE = "RANGE";
	/// <summary>
	/// Check if the target is within melee range
	/// </summary>
	/// <param name="user"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	/// 

	public MeleeAbility(Ability ability)
		: base (ability)
	{
		// Creates a MeleeAbility that copies the ability passed to it
		// This is the best way I can think of at the moment to get
		// a derived instance from the base instance created by the inspector

		// Im hoping I can right a custom editor that will create the correct type based on an enum dropdown
	}


	public override bool Useable(AbilityActor user, AbilityActor target)
	{
		return base.Useable(user, target);
	}


	public override AbilityActor[] FindTargets(Vector3 userPosition, LayerMask targetMask)
	{
		Collider[] hits = Physics.OverlapSphere(userPosition, GetValue("RANGE"), targetMask);

		AbilityActor[] targets = new AbilityActor[hits.Length];

		for (int i = 0; i < hits.Length; i++)
		{
			Debug.Log(hits[i].gameObject.name);

			targets[i] = hits[i].GetComponent<AbilityActor>();

			if (targets[i] == null)
			{
				Debug.LogWarning($"FindTargets found an invalid target. Check targetMask");
			}
		}

		return targets;
	}
}
