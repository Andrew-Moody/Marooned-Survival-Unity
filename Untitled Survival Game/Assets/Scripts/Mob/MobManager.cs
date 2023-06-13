using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : NetworkBehaviour
{
	public static MobManager Instance;

	[SerializeField]
	private int _mobCap;

	[SerializeField]
	private Transform _mobHolder;
	public Transform MobHolder => _mobHolder;

	[SerializeField]
	private MobSO[] _mobSOs;


	public static bool IsMobCapFull { get => Instance._mobCount >= Instance._mobCap; }

	private Dictionary<string, int> _nameToID;

	private Dictionary<int, MobSO> _mobSODict;

	private int _mobCount;

	public static int MobCount => Instance._mobCount;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		Instance.LoadMobSOs();
	}


	[Server]
	public static Mob SpawnMob(int mobID, Vector3 position, Quaternion rotation)
	{
		if (IsMobCapFull)
		{
			return null;
		}

		MobSO mobSO = GetMobSO(mobID);

		Mob mob = Instantiate(mobSO.BaseMobPrefab, Instance._mobHolder, false);
		mob.NetworkTransform.position = position;
		mob.NetworkTransform.rotation = rotation;

		Instance.Spawn(mob.gameObject);

		mob.ObserversInitializeMob(mobID);

		Instance._mobCount++;

		mob.Combatant.OnDeathEndEvent += () => Instance._mobCount--;

		return mob;
	}


	[Server]
	public static Mob SpawnMob(string mobName, Vector3 position, Quaternion rotation)
	{
		if (Instance._nameToID.TryGetValue(mobName, out int id))
		{
			return SpawnMob(id, position, rotation);
		}

		return null;
	}


	public static MobSO GetMobSO(int mobID)
	{
		if (!Instance._mobSODict.TryGetValue(mobID, out MobSO mobSO))
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
