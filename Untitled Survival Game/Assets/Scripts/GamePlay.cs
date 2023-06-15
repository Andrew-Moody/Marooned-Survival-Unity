using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : NetworkBehaviour
{
	public static GamePlay Instance;

	public event System.Action TimeChangedEvent;

	public int CurrentDay => _currentDay;
	public int CurrentHour => _currentHour;
	public float HourLength => _hourLength;
	public float NormalizedTimeInHour => _currentHour / _hourLength;


	[SerializeField]
	private float _hourLength;

	private int _currentDay;
	private int _currentHour;
	private float _timeInHour;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	void Update()
	{
		_timeInHour += Time.deltaTime;

		if (_timeInHour >= _hourLength)
		{
			_timeInHour = _timeInHour - _hourLength;
			_currentHour++;

			if (_currentHour > 23)
			{
				_currentHour = 0;
				_currentDay++;
			}


			Debug.LogError($"Current Time: {_currentDay} {_currentHour}");
			SyncTimeORPC(_currentDay, _currentHour, _timeInHour);
		}
	}


	public static GameObject GetRandomPlayer()
	{
		int index = Random.Range(0, GameManager.Instance.PlayerCount);

		return GameManager.Instance.GetPlayer(index);
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void SyncTimeORPC(int day, int hour, float timeInHour)
	{
		_currentDay = day;
		_currentHour = hour;
		_timeInHour = timeInHour;

		TimeChangedEvent?.Invoke();
	}
}
