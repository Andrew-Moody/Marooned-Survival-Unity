using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	public class AnimEventForwarder : MonoBehaviour
	{
		public event System.Action<AnimEventData> AnimEventRecieved;

		private void AbilityCue(int param)
		{
			AnimEventData data = new AnimEventData()
			{
				EventKind = AnimEventKind.AbilityCue,
				Param = param
			};

			AnimEventRecieved?.Invoke(data);
		}


		private void AbilityEnd(int param)
		{
			AnimEventData data = new AnimEventData()
			{
				EventKind = AnimEventKind.AbilityEnd,
				Param = param
			};

			AnimEventRecieved?.Invoke(data);
		}
	}

	public enum AnimEventKind
	{
		None,
		AbilityCue,
		AbilityEnd,
	}


	public class AnimEventData
	{
		public AnimEventKind EventKind { get; set; }
		public int Param { get; set; } 
	}
}

