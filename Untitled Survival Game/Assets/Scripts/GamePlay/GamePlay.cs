using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

public class GamePlay : NetworkBehaviour
{
	public static GamePlay Instance;

	public event System.Action TimeChangedEvent;

	public int CurrentDay => _currentDay;
	public int CurrentHour => _currentHour;
	public float HourLength => _hourLength;
	public float NormalizedTimeInHour => _currentHour / _hourLength;

	[SerializeField]
	private MobSpawnController _mobController;

	[SerializeField]
	private RitualAltar _ritualAltar;


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


	public override void OnStartNetwork()
	{
		base.OnStartNetwork();

		if (IsServer)
		{
			_mobController.enabled = true;

			_ritualAltar.RitualStartedEvent += OnRitualStarted;
		}
		else
		{
			enabled = false;
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


	private void OnRitualStarted()
	{
		Mob demon = MobManager.SpawnMob("Demon", _ritualAltar.transform.position, Quaternion.identity);

		demon.DeathFinished += OnDemonKilled;
	}


	private void OnDemonKilled(IActor actor, ActorEventData data)
	{
		ShowWinScreenORPC();
	}


	[ObserversRpc(RunLocally = true, BufferLast = true)]
	private void ShowWinScreenORPC()
	{
		PlayerInput.SetFPSMode(false);
		UIManager.ShowPanel("WinUI");
	}
}
