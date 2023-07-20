using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem
{
	public class CueManager : MonoBehaviour
	{
		public static CueManager Instance => _instance;
		private static CueManager _instance;


		[SerializeField]
		private CueDatabase _cueDatabase;

		private Dictionary<int, Cue> _cueMap;


		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				Initialize();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Initialize()
		{
			_cueMap = _cueDatabase.BuildCueMap();
		}


		public static void HandleCue(AbilityTrait trait, CueEventType eventType, CueEventData data)
		{
			// Attempt to let the actor choose the response to a given tag
			if (!data.Target.TryHandleCue(trait, eventType, data))
			{
				// If not handled by the actor handle it here
				if (Instance._cueMap.TryGetValue(trait.GetTraitKey(), out Cue cue))
				{
					cue.HandleCue(eventType, data);
				}
			}
		}
	}
}

