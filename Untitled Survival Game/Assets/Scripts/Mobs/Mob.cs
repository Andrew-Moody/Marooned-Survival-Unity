using FishNet.Component.Animating;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : NetworkBehaviour
{
	[SerializeField]
	private Transform _graphicParent;

	[SerializeField]
	private NetworkAnimator _networkAnimator;

	[SerializeField]
	private Agent _agent;

	[SerializeField]
	private AbilityActor _abilityActor;

	[SerializeField]
	private Combatant _combatant;

	[SerializeField]
	private Stats _stats;

	[SerializeField]
	private Transform _statDisplay;

	public void Initialize(MobSO mobSO)
	{
		if (mobSO == null)
		{
			return;
		}

		GameObject mobGraphic = mobSO.InstantiatePrefab(_graphicParent);

		Animator animator = mobGraphic.GetComponent<Animator>();

		_agent.Animator = animator;

		_agent.RoamCenter = transform.position;

		_agent.SetStateMachine(mobSO.MobAISO.GetRuntimeFSM());

		_abilityActor.Initialize(animator);

		_combatant.Initialize(mobSO._abilities);

		_networkAnimator.SetAnimator(animator);

		if (IsServer)
		{
			foreach (StatValue stat in mobSO._stats)
			{
				_stats.SetStat(stat.StatType, stat.Value);
			}
		}
	}


	[ObserversRpc(BufferLast = true, RunLocally = true)]
	public void ObserversInitializeMob(int mobID)
	{
		MobSO mobSO = MobManager.Instance.GetMobSO(mobID);

		Initialize(mobSO);
	}


	private void LateUpdate()
	{
		_statDisplay.position = _graphicParent.position;
	}
}
