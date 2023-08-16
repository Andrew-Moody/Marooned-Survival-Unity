using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	public class StatOperation : ScriptableObject
	{
		public virtual void Apply(Actor source, Actor target, OperationData data) { }
	}


	public class OperationData { };
}
