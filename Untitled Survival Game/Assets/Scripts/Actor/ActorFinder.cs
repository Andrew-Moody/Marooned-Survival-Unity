using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Actors
{
	public class ActorFinder : MonoBehaviour
	{
		public Actor Actor { get; private set; }

		void Start()
		{
			Actor = Actor.FindActor(gameObject);
		}
	}
}
