using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : NetworkBehaviour
{
	public static MobManager Instance => _instance;
	private static MobManager _instance;

	[SerializeField] private MobFactory _mobFactory;

	[SerializeField] private int _mobCap;

	public Transform MobHolder => _mobHolder;
	[SerializeField] private Transform _mobHolder;
	

	public static bool IsMobCapFull => Instance._mobCount >= Instance._mobCap;

	public static int MobCount => Instance._mobCount;

	private int _mobCount;


	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	[Server]
	public static Mob SpawnMob(int mobID, Vector3 position, Quaternion rotation)
	{
		if (IsMobCapFull)
		{
			return null;
		}

		Mob mobPrefab = _instance._mobFactory.GetMobPrefab(mobID);

		if (mobPrefab != null)
		{
			Mob mob = Instantiate(mobPrefab, position, rotation, _instance._mobHolder);
			_instance.Spawn(mob.gameObject);

			_instance._mobCount++;
			mob.DeathFinished += (actor, data) => Instance._mobCount--;

			return mob;
		}

		return null;
	}


	[Server]
	public static Mob SpawnMob(string mobName, Vector3 position, Quaternion rotation)
	{
		int mobID = _instance._mobFactory.GetMobID(mobName);

		return SpawnMob(mobID, position, rotation);
	}
}
