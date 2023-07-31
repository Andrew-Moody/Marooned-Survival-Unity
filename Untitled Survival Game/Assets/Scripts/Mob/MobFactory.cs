using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/MobFactory")]
public class MobFactory : ScriptableObject
{
	[SerializeField]
	private GameObject[] _mobPrefabs;

	private Dictionary<int, Mob> _mobDict;

	private Dictionary<string, int> _nameToID;


	public Mob GetMobPrefab(int mobID)
	{
		if (_mobDict == null)
		{
			LoadMobPrefabs();
		}

		if (_mobDict.TryGetValue(mobID, out Mob mob))
		{
			return mob;
		}
		
		Debug.LogWarning($"Failed to create mob with ID: {mobID}");

		return null;
	}


	public int GetMobID(string name)
	{
		if (_nameToID == null)
		{
			LoadMobPrefabs();
		}

		if (_nameToID.TryGetValue(name, out int mobID))
		{
			return mobID;
		}

		Debug.LogWarning($"Failed to create mob with Name: {name}");

		return -1;
	}


	private void LoadMobPrefabs()
	{
		_mobDict = new Dictionary<int, Mob>();

		_nameToID = new Dictionary<string, int>();

		foreach (GameObject pf in _mobPrefabs)
		{
			if (pf.TryGetComponent(out Mob mob))
			{
				_mobDict.Add(mob.ID, mob);

				_nameToID.Add(mob.MobName, mob.ID);
			}
		}
	}


	private void OnValidate()
	{
		for (int i = 0; i < _mobPrefabs.Length; i++)
		{
			if (_mobPrefabs[i] != null && _mobPrefabs[i].GetComponent<Mob>() == null)
			{
				_mobPrefabs[i] = null;

				Debug.LogWarning("Mob prefab must have a Mob component");
			}
		}
	}
}
