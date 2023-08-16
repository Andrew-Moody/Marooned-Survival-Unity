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

			float damage = basicData.Value - target.Stats.GetStatValue(StatKind.Armor);

			damage = Mathf.Clamp(damage, 0, damage);

			float health = target.Stats.GetStatValue(StatKind.Health);

			health -= damage;

			target.Stats.SetStatValue(StatKind.Health, health);
		}
	}


	public class BasicOpData : OperationData
	{
		public float Value { get; set; }
	}
}
