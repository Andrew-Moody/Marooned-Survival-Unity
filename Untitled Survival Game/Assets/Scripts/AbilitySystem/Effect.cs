using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewEffect", menuName = "AbilitySystem/Effect")]
	public class Effect : ScriptableObject
	{
		public DurationType DurationType => _durationType;
		[SerializeField] private DurationType _durationType;

		public float Duration => _duration;
		[SerializeField] private float _duration;


		public StatModifier[] Modifiers => _modifiers;
		[SerializeField] private StatModifier[] _modifiers;


		public AbilityTrait[] Cues => _cues;
		[SerializeField] private AbilityTrait[] _cues;


		public AbilityTrait[] Traits => _traits;
		[SerializeField] private AbilityTrait[] _traits;


		public AbilityInputBinding[] AppliedAbilities => _appliedAbilities;
		[SerializeField] private AbilityInputBinding[] _appliedAbilities;
	}


	public enum DurationType
	{
		Instant,
		Infinite,
		Duration
	}
}
