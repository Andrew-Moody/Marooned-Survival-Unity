using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AbilitySystem
{
	[CreateAssetMenu(menuName = "AbilitySystem/CueDatabase")]
	public class CueDatabase : ScriptableObject
	{
		[SerializeField]
		private List<Cue> _cues;


		public Dictionary<AbilityTag, Cue> BuildCueMap()
		{
			Dictionary<AbilityTag, Cue> cueMap = new Dictionary<AbilityTag, Cue>();

			for (int i = 0; i < _cues.Count; i++)
			{
				if (cueMap.ContainsKey(_cues[i].Tag))
				{
					Debug.LogError($"Failed to add cue ({_cues[i].name}) to global set: tag ({_cues[i].Tag}) already exists");
				}
				else
				{
					cueMap[_cues[i].Tag] = _cues[i];
				}
			}

			return cueMap;
		}
	}
}
