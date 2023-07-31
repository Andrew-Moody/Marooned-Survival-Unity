using FishNet.Component.Animating;
using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;
using AbilitySystem;

public class Mob : Actor
{
	public int ID => _mobSO.ID;

	public string MobName => _mobSO.Name;

	public MobSO MobSO => _mobSO;
	[SerializeField] private MobSO _mobSO;

	
	protected override void DoDeathFinish()
	{
		Vector3 spawnPos = NetTransform.position + new Vector3(0f, 0.5f, 0f);
		ItemManager.Instance.SpawnWorldItem(_mobSO.ItemToDrop, spawnPos);
	}
}
