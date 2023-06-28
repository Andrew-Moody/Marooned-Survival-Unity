using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MeleeAbility : Ability
{
	[SerializeField]
	private float _range;
	public float Range => _range;


	public MeleeAbility()
	{
		// Derived default constructor (no parameters) implicitly calls base default constructor
		// unless a non default constructor is defined without also defining a default constructor

		// if default constructor is missing for the derived type it will first call the base default constructor
		// (which I expected) but then it still sets values back to their default values because a default constructor was missing.

		// When this constructor is commented out the base constructor gets called but non serialized values still get set to default
		//Debug.LogWarning($"MeleeAbility Constructor Called for {this}");
	}


	public MeleeAbility(Ability ability)
		: base (ability)
	{
		//Debug.LogError("MeleeAbility copied from Ability");
	}


	public MeleeAbility(MeleeAbility ability)
		: base (ability)
	{
		_range = ability._range;
	}


	public static Ability Create()
	{
		return new MeleeAbility();
	}


	public static Ability CreateFrom(Ability ability)
	{
		return new MeleeAbility(ability);
	}


	public override Ability CreateCopy()
	{
		//Debug.LogWarning($"MeleeAbility.CreateCopy called on {AbilityName}");
		return new MeleeAbility(this);
	}


	public override bool Useable(bool asServer, AbilityActor user, AbilityActor target)
	{
		return base.Useable(asServer, user, target);
	}


	public override AbilityActor[] FindTargets(Vector3 userPosition, LayerMask targetMask)
	{
		Debug.LogError($"FindTargets with range {_range}");

		Collider[] hits = Physics.OverlapSphere(userPosition, _range, targetMask);

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
