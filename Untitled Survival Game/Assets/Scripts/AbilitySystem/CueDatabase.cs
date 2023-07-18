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


		public Dictionary<AbilityTrait, Cue> BuildCueMap()
		{
			Dictionary<AbilityTrait, Cue> cueMap = new Dictionary<AbilityTrait, Cue>();

			for (int i = 0; i < _cues.Count; i++)
			{
				if (cueMap.ContainsKey(_cues[i].Trait))
				{
					Debug.LogError($"Failed to add cue ({_cues[i].name}) to global set: tag ({_cues[i].Trait}) already exists");
				}
				else
				{
					cueMap[_cues[i].Trait] = _cues[i];
				}
			}

			return cueMap;
		}
	}
}
