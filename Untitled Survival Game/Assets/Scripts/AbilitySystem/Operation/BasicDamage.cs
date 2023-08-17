using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "BasicDamage", menuName = "AbilitySystem/Operation/BasicDamage")]
	public class BasicDamage : StatOperation
	{
		public override void Apply(Actor source, Actor target, OperationData data)
		{
			BasicOpData basicData = data as BasicOpData;

			if (basicData == null)
			{
				return;
			}

			float armor = target.Stats.GetStatValue(StatKind.Armor);

			float damageUnclamped = basicData.Value - armor;

			float damage = Mathf.Clamp(damageUnclamped, 0f, damageUnclamped);

			float health = target.Stats.GetStatValue(StatKind.Health);

			health -= damage;

			target.Stats.SetStatValue(StatKind.Health, health);

			Debug.LogWarning($"Value: {basicData.Value}, Armor: {armor}, Damage: {damage}, DamageUnclamped: {damageUnclamped}, Health: {health}");
		}
	}


	public class BasicOpData : OperationData
	{
		public float Value { get; set; }
	}
}
