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
    private MobFactory _mobFactory;


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}


	[Server]
	public Mob SpawnMob(int mobID, Transform parent)
	{
		Mob mob = Instantiate(_mobBase, parent, false);
		Spawn(mob.gameObject);

		mob.ObserversInitializeMob(mobID);

		return mob;
	}


	public MobSO GetMobSO(int mobID)
	{
		return _mobFactory.GetMobSO(mobID);
	}
}
