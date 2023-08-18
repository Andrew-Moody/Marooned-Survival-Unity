using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "ToolDamage", menuName = "AbilitySystem/Operation/ToolDamage")]
	public class ToolDamage : StatOperation
	{
		public override void Apply(Actor source, Actor target, OperationData data)
		{
			BasicOpData basicData = data as BasicOpData;

			if (basicData == null)
			{
				return;
			}

			float armor = target.Stats.GetStatValue(StatKind.Armor);

			float hardness = target.Stats.GetStatValue(StatKind.Hardness);

			float damageReduction = Mathf.Max(armor, hardness);

			float damageUnclamped = basicData.Value - damageReduction;

			float damage = Mathf.Clamp(damageUnclamped, 0f, damageUnclamped);

			float health = target.Stats.GetStatValue(StatKind.Health);

			health -= damage;

			target.Stats.SetStatValue(StatKind.Health, health);

			Debug.LogWarning($"Value: {basicData.Value}, Armor: {armor}, Hardness: {hardness}, Damage: {damage}, DamageUnclamped: {damageUnclamped}, Health: {health}");
		}
	}
}
