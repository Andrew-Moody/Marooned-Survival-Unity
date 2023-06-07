using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : NetworkBehaviour
{
	public static MobManager Instance;

	[SerializeField]
	private Mob _mobBase;


	[SerializeField]
	private MobSO[] _mobSOs;


	private Dictionary<string, int> _nameToID;

	private Dictionary<int, MobSO> _mobSODict;


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		Instance.LoadMobSOs();
	}


	[Server]
	public Mob SpawnMob(int mobID, Transform parent)
	{
		Mob mob = Instantiate(_mobBase, parent, false);
		Spawn(mob.gameObject);

		mob.ObserversInitializeMob(mobID);

		return mob;
	}


	[Server]
	public Mob SpawnMob(string mobName, Transform parent)
	{
		Mob mob = Instantiate(_mobBase, parent, false);

		Spawn(mob.gameObject);

		mob.ObserversInitializeMob(_nameToID[mobName]);

		return mob;
	}


	public MobSO GetMobSO(int mobID)
	{
		if (!_mobSODict.TryGetValue(mobID, out MobSO mobSO))
		{
			Debug.LogError($"MobManager failed to find MobSO for ID {mobID}");
		}

		return mobSO;
	}


	private void LoadMobSOs()
	{
		_mobSODict = new Dictionary<int, MobSO>();

		_nameToID = new Dictionary<string, int>();


		foreach (MobSO so in _mobSOs)
		{
			if (_mobSODict.ContainsKey(so.ID))
			{
				MobSO prev = _mobSODict[so.ID];
				Debug.LogWarning($"MobSO {so.name} with ID {so.ID} conflicts with MobSO {prev.name}");
			}

			if (_nameToID.ContainsKey(so.Name))
			{
				int id = _nameToID[so.Name];

				if (_mobSODict.TryGetValue(id, out MobSO prev))
				{
					Debug.LogWarning($"MobSO {so.name} with Name {so.Name} conflicts with MobSO {prev.name}");
				}
				else
				{
					Debug.LogWarning($"MobSO {so.name} with Name {so.Name} conflicts with an existing name");
				}
			}

			_mobSODict[so.ID] = so;
			_nameToID[so.Name] = so.ID;
		}
	}
}
