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


		public Dictionary<int, Cue> BuildCueMap()
		{
			Dictionary<int, Cue> cueMap = new Dictionary<int, Cue>();

			for (int i = 0; i < _cues.Count; i++)
			{
				int traitHash = _cues[i].Trait.GetTraitKey();

				if (cueMap.ContainsKey(traitHash))
				{
					Debug.LogError($"Failed to add cue ({_cues[i].name}) to global set: tag ({_cues[i].Trait}) already exists");
				}
				else
				{
					cueMap[traitHash] = _cues[i];
				}
			}

			return cueMap;
		}
	}
}
