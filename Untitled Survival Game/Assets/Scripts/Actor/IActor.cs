using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	public interface IActor
	{
		public event ActorEventHandler DeathStarted;
		public event ActorEventHandler DeathFinished;
	}
}
