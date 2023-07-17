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


		[SerializeField] private StatModifier[] _modifiers;


		[SerializeField] private AbilityTag[] _cues;


	}


	public enum DurationType
	{
		Instant,
		Infinite,
		Duration
	}
}
