using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalTicker : NetworkBehaviour
{

	[SerializeField]
	private TickMode _tickMode;

	[SerializeField]
	private FunctionPlotter _plotter;

	private enum TickMode
	{
		PreTick,
		OnTick,
		PostTick
	}


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		TimeManager.OnPreTick += PreTick;

		TimeManager.OnTick += OnTick;

		TimeManager.OnPostTick += PostTick;
	}


	public override void OnStopNetwork()
	{
		base.OnStopNetwork();

		if (TimeManager != null)
		{
			TimeManager.OnPreTick -= PreTick;

			TimeManager.OnTick -= OnTick;

			TimeManager.OnPostTick -= PostTick;
		}
	}


	private void PreTick()
	{
		if (_tickMode == TickMode.PreTick)
		{
			TickPlotter((float)TimeManager.TickDelta);
		}
	}


	private void OnTick()
	{
		if (_tickMode == TickMode.OnTick)
		{
			TickPlotter((float)TimeManager.TickDelta);
		}
	}


	private void PostTick()
	{
		if (_tickMode == TickMode.PostTick)
		{
			TickPlotter((float)TimeManager.TickDelta);
		}
	}


	private void TickPlotter(float deltaTime)
	{
		if (gameObject.activeInHierarchy)
		{
			_plotter.ExternalTick(deltaTime);
		}
		
	}
}
