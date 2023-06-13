using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/MobFactory")]
public class MobFactory : ScriptableObject
{
	[SerializeField]
	private List<MobSO> _mobs;


	private Dictionary<int, MobSO> _mobDict;


	public MobSO GetMobSO(int mobID)
	{
		if (_mobDict == null)
		{
			InitializeDict();
		}

		if (_mobDict.TryGetValue(mobID, out MobSO mobSO))
		{
			return mobSO;
		}
		else
		{
			Debug.LogWarning($"Failed to create mob with ID: {mobID}");
		}

		return null;
	}


	private void OnValidate()
	{
		InitializeDict();
	}


	private void InitializeDict()
	{
		_mobDict = new Dictionary<int, MobSO>();

		foreach (MobSO mob in _mobs)
		{
			if (mob != null)
			{
				_mobDict.Add(mob.ID, mob);
			}
		}
	}
}
