using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{

	public event Action OnAbilityAnimEvent;

	public event Action OnAbilityEndAnimEvent;

	public event Action OnDeathEndAnimEvent;


	private void AbilityAnimEvent()
	{
		//Debug.Log("AbilityAnimEvent");

		OnAbilityAnimEvent?.Invoke();
	}


	private void AbilityEndAnimEvent()
	{
		OnAbilityEndAnimEvent?.Invoke();
	}


	private void DeathEndAnimEvent()
	{
		OnDeathEndAnimEvent?.Invoke();
	}
}
