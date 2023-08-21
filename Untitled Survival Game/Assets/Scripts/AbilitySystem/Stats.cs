using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	public class Stats : NetworkBehaviour, IUIEventPublisher
	{
		public event Action<UIEventData> UIEvent;

		public event Action<StatKind> StatEmptied;

		[SerializeField]
		private List<StatInitialValue> _statInitialValues;


		private Dictionary<StatKind, ActorStat> _stats;

		private const float DEFAULT_MAX = 100f;


		#region NetworkCallbacks

		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			SetInitialValues(_statInitialValues);

			Actor actor = Actor.FindActor(gameObject);

			if (actor != null && actor.IsServer && actor.Inventory != null)
			{
				actor.Inventory.ItemEquipped += Inventory_ItemEquipped;
			}
		}


		public override void OnStartClient()
		{
			base.OnStartClient();

			// OnStartNetwork was too early to initialize client UI
			foreach (var stat in _stats)
			{
				UIEvent?.Invoke(stat.Value.GetUIData());
			}
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
			if (statKind != StatKind.None && _stats.TryGetValue(statKind, out ActorStat stat))
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

				if (stat.IsEmpty())
				{
					StatEmptied?.Invoke(statKind);
				}

				return stat.Value;
			}

			if (statKind != StatKind.None)
			{
				Debug.LogWarning($"Stats does not contain a stat with specified trait key: {statKind}");
			}

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
					// Set the max to the initial value unless the initial value is zero
					float max = (value.Value == 0f) ? DEFAULT_MAX : value.Value;

					ActorStat stat = new ActorStat(value.StatKind, 0f, max, value.Value);

					_stats[value.StatKind] = stat;

					UIEvent?.Invoke(stat.GetUIData());
				}
			}
		}


		private void Inventory_ItemEquipped(object sender, ItemEquippedArgs args)
		{
			// ItemEquippedArgs doesn't contain a PrevItem field and Inventory would require
			// extensive modification to allow adding it
			// for now let the abilitySet control Apply and Remove since it does maintain a reference to the prev item
			
			//RemoveItemStats(args.PrevItem.ItemSO);

			//ApplyItemStats(args.Item.ItemSO);
		}


		public void ApplyItemStats(ItemSO item)
		{
			foreach (StatInitialValue statIV in item.Stats)
			{
				if (statIV.StatKind != StatKind.None && _stats.TryGetValue(statIV.StatKind, out ActorStat stat))
				{
					float value = stat.Value + statIV.Value;
					SetStat(statIV.StatKind, value);
				}
			}
		}


		public void RemoveItemStats(ItemSO item)
		{
			foreach (StatInitialValue statIV in item.Stats)
			{
				if (statIV.StatKind != StatKind.None && _stats.TryGetValue(statIV.StatKind, out ActorStat stat))
				{
					float value = stat.Value - statIV.Value;
					SetStat(statIV.StatKind, value);
				}
			}
		}


		private void OnDestroy()
		{
			Actor actor = Actor.FindActor(gameObject);

			if (actor != null && actor.IsServer && actor.Inventory != null)
			{
				actor.Inventory.ItemEquipped -= Inventory_ItemEquipped;
			}
		}
	}
}
