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

	[SerializeField]
	private GameObject[] _mobPrefabs;


	public static bool IsMobCapFull { get => Instance._mobCount >= Instance._mobCap; }

	private Dictionary<string, int> _nameToID;

	private Dictionary<int, MobSO> _mobSODict;

	private Dictionary<int, Mob> _mobPrefabDict;

	private int _mobCount;

	public static int MobCount => Instance._mobCount;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		Instance.LoadMobSOs();

		Instance.LoadMobPrefabs();
	}


	[Server]
	public static Mob SpawnMob(int mobID, Vector3 position, Quaternion rotation)
	{
		if (IsMobCapFull)
		{
			return null;
		}

		Mob mob = Instance.SpawnMobFromPrefab(mobID, position, rotation);

		Instance._mobCount++;

		mob.MobDied += (Mob mob) => Instance._mobCount--;

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


	private Mob SpawnMobFromSO(int mobID, Vector3 position, Quaternion rotation)
	{
		MobSO mobSO = GetMobSO(mobID);

		Mob mob = Instantiate(mobSO.BaseMobPrefab, position, rotation, _mobHolder);

		Spawn(mob.gameObject);

		mob.ObserversInitializeMob(mobID);

		return null;
	}


	private Mob SpawnMobFromPrefab(int mobID, Vector3 position, Quaternion rotation)
	{
		if (_mobPrefabDict.TryGetValue(mobID, out Mob mobPrefab))
		{
			Mob mob = Instantiate(mobPrefab, position, rotation, _mobHolder);
			Spawn(mob.gameObject);

			return mob;
		}

		return null;
	}


	private void LoadMobPrefabs()
	{
		_mobPrefabDict = new Dictionary<int, Mob>();

		_nameToID = new Dictionary<string, int>();

		foreach (GameObject pf in _mobPrefabs)
		{
			if (pf.TryGetComponent(out Mob mob))
			{
				_mobPrefabDict.Add(mob.ID, mob);

				_nameToID.Add(mob.MobName, mob.ID);
			}
		}
	}


	protected override void OnValidate()
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
