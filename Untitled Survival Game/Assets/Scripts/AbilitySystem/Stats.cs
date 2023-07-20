using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actor
{
	public class Stats : NetworkBehaviour, IUIEventPublisher
	{
		public event Action<UIEventData> UIEvent;


		[SerializeField]
		private List<StatInitialValue> _statInitialValues;


		private Dictionary<StatKind, ActorStat> _stats;


		#region NetworkCallbacks

		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			SetInitialValues(_statInitialValues);
		}

		#endregion


		public ActorStat GetStat(StatKind statKind)
		{
			if (_stats.TryGetValue(statKind, out ActorStat stat))
			{
				return new ActorStat(stat);
			}

			return null;
		}

		public float GetStatValue(StatKind statKind)
		{
			if (_stats.TryGetValue(statKind, out ActorStat stat))
			{
				return stat.Value;
			}

			return 0f;
		}


		[Server]
		public void SetStatValue(StatKind statKind, float value)
		{
			// SetStat may produce a clamped or modified result
			float statValue = SetStat(statKind, value);

			SetStatValueORPC(statKind, statValue);
		}

		[Server]
		public void SetStatBounds(StatKind statKind, float minValue, float maxValue)
		{
			SetBounds(statKind, minValue, maxValue);

			SetStatBoundsORPC(statKind, minValue, maxValue);
		}



		[ObserversRpc]
		private void SetStatValueORPC(StatKind statKind, float value)
		{
			SetStat(statKind, value);
		}


		[ObserversRpc]
		private void SetStatBoundsORPC(StatKind statKind, float minValue, float maxValue)
		{
			SetBounds(statKind, minValue, maxValue);
		}


		private float SetStat(StatKind statKind, float value)
		{
			if (_stats.TryGetValue(statKind, out ActorStat stat))
			{
				stat.SetStatClamped(value);

				UIEvent?.Invoke(stat.GetUIData());

				return stat.Value;
			}

			Debug.LogWarning("Stats does not contain a stat with specified trait key");

			return 0f;
		}


		private void SetBounds(StatKind statKind, float minValue, float maxValue)
		{
			if (_stats.TryGetValue(statKind, out ActorStat stat))
			{
				stat.SetBounds(minValue, maxValue);

				UIEvent?.Invoke(stat.GetUIData());
			}

			Debug.LogWarning("Stats does not contain a stat with specified trait key");
		}


		// Server and clients set the initial value to whatever they have stored in data
		// if the client has user modified data it wont impact others
		public void SetInitialValues(List<StatInitialValue> initialValues)
		{
			_stats = new Dictionary<StatKind, ActorStat>();

			foreach (StatInitialValue value in initialValues)
			{
				if (_stats.ContainsKey(value.StatKind))
				{
					Debug.LogWarning($"{gameObject} Initial stats contains multiple entries for trait {value.StatKind}");
				}
				else
				{
					ActorStat stat = new ActorStat();
					stat.SetBounds(0f, value.Value);
					stat.SetStatClamped(value.Value);

					_stats[value.StatKind] = stat;

					UIEventData data = new UIFloatChangeEventData()
					{
						TagString = value.StatKind.ToString(),
						Value = stat.Value,
						MinValue = stat.MinValue,
						MaxValue = stat.MaxValue
					};

					UIEvent?.Invoke(data);
				}
			}
		}
	}
}
