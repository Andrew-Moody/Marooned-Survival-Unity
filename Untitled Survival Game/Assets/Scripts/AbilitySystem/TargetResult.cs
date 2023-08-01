using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor = Actors.Actor;

namespace AbilitySystem
{
	// Used to return the result of a targeting action
	// Additional data may be added or may just have to derive custom result classes

	public class TargetResult { }


	public class GameObjectTargetResult : TargetResult
	{
		public GameObject GameObject { get; set; }
	}


	public class ActorTargetResult : TargetResult
	{
		public Actor Actor { get; set; }
	}


	public class InteractTargetResult : TargetResult
	{
		public Interactable Interactable { get; set; }
	}


	public class PointNormalTargetResult : TargetResult
	{
		public Vector3 Point { get; set; }
		public Vector3 Normal { get; set; }
	}
}
