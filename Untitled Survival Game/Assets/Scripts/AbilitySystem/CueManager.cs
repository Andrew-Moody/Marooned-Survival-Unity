using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(menuName = "AbilitySystem/CueManager")]
public class CueManager : ScriptableObject
{

	private static CueManager _instance;

	public static CueManager Instance
	{
		get
		{
			if (_instance == null)
			{
				CueManager[] managers = Resources.FindObjectsOfTypeAll<CueManager>();

				if (managers.Length == 0)
				{
					Debug.LogError("Failed to find CueManager Asset");
				}
				else if (managers.Length > 1)
				{
					Debug.LogError("Found multiple instances of CueManager Asset");
				}
				else
				{
					_instance = managers[0];
					_instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
					_instance.OnNewInstance();
				}
			}

			return _instance;
		}
	}


	private Cue[] _cues;

	private Dictionary<AbilityTag, Cue> _cueMap;


	private void OnNewInstance()
	{
		_cues = Resources.FindObjectsOfTypeAll<Cue>();

		_cueMap = new Dictionary<AbilityTag, Cue>();

		for (int i = 0; i < _cues.Length; i++)
		{
			_cueMap[_cues[i].Tag] = _cues[i];
		}
	}


	public static void NotifyCue(AbilityTag tag, CueEventType eventType, CueEventData data)
	{
		if (Instance._cueMap.TryGetValue(tag, out Cue cue))
		{
			switch(eventType)
			{
				case CueEventType.OnExecute:
					cue.OnExecute(data);
					break;
				case CueEventType.OnActivate:
					cue.OnActivate(data);
					break;
				case CueEventType.OnRemove:
					cue.OnRemove();
					break;
				case CueEventType.OnVisible:
					cue.OnVisible(data);
					break;
			}
		}
	}
}
