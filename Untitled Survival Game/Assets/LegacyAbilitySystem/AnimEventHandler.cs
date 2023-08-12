using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{

	public event Action OnAbilityAnimEvent;

	public event Action OnAbilityEndAnimEvent;

	public event Action OnDeathEndAnimEvent;


	private void AbilityAnimEvent(string stringParam)
	{
		Debug.Log($"AbilityAnimEvent {stringParam}");

		OnAbilityAnimEvent?.Invoke();
	}


	private void AbilityEndAnimEvent(string stringParam)
	{
		Debug.Log($"AbilityEndAnimEvent {stringParam}");

		OnAbilityEndAnimEvent?.Invoke();
	}


	private void DeathEndAnimEvent()
	{
		OnDeathEndAnimEvent?.Invoke();
	}
}
